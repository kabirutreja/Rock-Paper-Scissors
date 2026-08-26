using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UGUI_Toggle_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UGUI/Toggle",
                   order = 53)]
  public class ScriptableInputElementProvider_UGUI_FromPrefab_Toggle : ScriptableInputElementProvider_UGUI_FromPrefab_Generic<bool>
  {
    [SerializeField]
    [Tooltip("The type of the toggle transition.")]
    protected Toggle.ToggleTransition transition = Toggle.ToggleTransition.None;

    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UGUI_Toggle.InputFieldType;

    public override bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                                string labelText, SettingsCollection settings,
                                                List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);

      return success && ProviderUtility_UGUI_Toggle.UpdateInputElement(elem, settingIdentifier, settings,
                                                                       values, initialize, transition);
    }
  }
}
