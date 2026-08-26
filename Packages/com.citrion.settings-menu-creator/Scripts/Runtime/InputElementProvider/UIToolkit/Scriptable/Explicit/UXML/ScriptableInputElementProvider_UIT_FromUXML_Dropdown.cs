using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_Dropdown_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Dropdown",
                   order = 121)]
  public class ScriptableInputElementProvider_UIT_FromUXML_Dropdown : ScriptableInputElementProvider_UIT_FromUXML_Generic<DropdownField, string>
  {
    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_Dropdown.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      return success && ProviderUtility_UIT_Dropdown.UpdateInputElement(elem, settingIdentifier, settings,
                                                                        labelText, values, initialize);
    }
  }
}
