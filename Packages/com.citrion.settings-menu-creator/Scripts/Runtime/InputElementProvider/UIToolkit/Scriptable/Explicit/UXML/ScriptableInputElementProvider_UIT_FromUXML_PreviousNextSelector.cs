using CitrioN.UI.UIToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_PrevNextSelector_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Previous Next Selector",
                   order = 122)]
  public class ScriptableInputElementProvider_UIT_FromUXML_PreviousNextSelector : ScriptableInputElementProvider_UIT_FromUXML_Generic<PreviousNextSelector, string>
  {
    [SerializeField]
    [Tooltip("Whether the options should be possible to cycle through. " +
             "If enabled the next option from the last option will be the first option " +
             "and the previous option from the first option will be the last option.")]
    protected bool allowCycle = false;

    [SerializeField]
    [Tooltip("Whether the buttons should represent cycling availability. " +
             "If enabled when cycling is disabled the next button will be disabled when the last option is selected " +
             "and the previous button will be disabled if the first option is selected.")]
    protected bool representNoCycleOnButtons = true;

    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_PreviousNextSelector.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);

      return success && ProviderUtility_UIT_PreviousNextSelector.UpdateInputElement(elem, settingIdentifier, settings,
                                                                                    labelText, values, initialize,
                                                                                    allowCycle, representNoCycleOnButtons);
    }
  }
}
