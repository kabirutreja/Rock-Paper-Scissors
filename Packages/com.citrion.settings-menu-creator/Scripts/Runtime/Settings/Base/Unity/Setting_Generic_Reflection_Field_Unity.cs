using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [ExcludeFromMenuSelection]
  [System.Serializable]
  public abstract class Setting_Generic_Reflection_Field_Unity<T1, T2> : Setting_Generic_Reflection_Field<T1, T2>
  {
    [SerializeField]
    [Tooltip("Should the current Unity value be assigned as the default value at runtime?")]
    protected bool assignUnityValueAsDefault = false;

    public override void InitializeForRuntime(SettingsCollection settings)
    {
      base.InitializeForRuntime(settings);

      var values = GetCurrentValues(settings);
      if (values?.Count > 0 && values[0] is T1 actualValue)
      {
        if (assignUnityValueAsDefault)
        {
          defaultValue = actualValue;
        }
      }
    }
  }
}

