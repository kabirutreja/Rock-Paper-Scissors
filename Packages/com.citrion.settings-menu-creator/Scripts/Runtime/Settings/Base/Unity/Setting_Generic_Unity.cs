using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Generic_Unity<T> : Setting_Generic<T>
  {
    // We don't store it internally so in the case of changing the
    // quality level it will load the value of the newly set level
    // from Unity instead of an internal value of this asset
    public override bool StoreValueInternally => false;

    //public override string EditorName => GetType().Name.Replace("Setting_", "").SplitCamelCase();

    //public override string RuntimeName => GetType().Name.Replace("Setting_", "").SplitCamelCase();


    [SerializeField]
    [Tooltip("Should the current Unity value be assigned as the default value at runtime?")]
    protected bool assignUnityValueAsDefault = false;

    public override void InitializeForRuntime(SettingsCollection settings)
    {
      base.InitializeForRuntime(settings);

      var values = GetCurrentValues(settings);
      if (values?.Count > 0 && values[0] is T actualValue)
      {
        if (assignUnityValueAsDefault)
        {
          defaultValue = actualValue;
        }
      }
    }
  }
}
