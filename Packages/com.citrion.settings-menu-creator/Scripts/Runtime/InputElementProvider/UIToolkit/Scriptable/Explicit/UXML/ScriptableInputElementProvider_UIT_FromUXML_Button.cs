using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_Button_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Button",
                   order = 125)]
  public class ScriptableInputElementProvider_UIT_FromUXML_Button : ScriptableInputElementProvider_UIT_FromUXML<Button>
  {
    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_Button.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      return success && ProviderUtility_UIT_Button.UpdateInputElement(elem, settingIdentifier, settings,
                                                                      labelText, values, initialize);
    }
  }
}
