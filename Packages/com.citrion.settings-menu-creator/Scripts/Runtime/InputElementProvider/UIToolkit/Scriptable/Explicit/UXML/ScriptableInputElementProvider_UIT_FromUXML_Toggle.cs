using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_Toggle_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Toggle",
                   order = 123)]
  public class ScriptableInputElementProvider_UIT_FromUXML_Toggle : ScriptableInputElementProvider_UIT_FromUXML_Generic<Toggle, bool>
  {
    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_Toggle.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      return success && ProviderUtility_UIT_Toggle.UpdateInputElement(elem, settingIdentifier, settings,
                                                                      labelText, values, initialize);
    }
  }
}
