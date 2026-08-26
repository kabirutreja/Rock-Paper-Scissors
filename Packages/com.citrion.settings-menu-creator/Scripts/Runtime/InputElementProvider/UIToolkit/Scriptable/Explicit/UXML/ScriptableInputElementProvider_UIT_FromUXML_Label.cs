using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_Label_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Label",
                   order = 126)]
  public class ScriptableInputElementProvider_UIT_FromUXML_Label : ScriptableInputElementProvider_UIT_FromUXML<Label>
  {
    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_Label.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      return success && ProviderUtility_UIT_Label.UpdateInputElement(elem, settings, labelText);
    }
  }
}
