using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UGUI_Dropdown_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UGUI/Dropdown",
                   order = 51)]
  public class ScriptableInputElementProvider_UGUI_FromPrefab_Dropdown : ScriptableInputElementProvider_UGUI_FromPrefab_Generic<string>
  {
    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UGUI_Dropdown.InputFieldType;

    public override bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      bool success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);

      return success && ProviderUtility_UGUI_Dropdown.UpdateInputElement(elem, settingIdentifier,
                                                                         settings, values, initialize);
    }
  }
}
