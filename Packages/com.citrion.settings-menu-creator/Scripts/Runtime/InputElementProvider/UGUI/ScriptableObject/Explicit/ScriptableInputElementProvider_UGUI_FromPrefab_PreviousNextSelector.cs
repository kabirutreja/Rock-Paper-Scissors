using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UGUI_PreviousNextSelector_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UGUI/PreviousNextSelector",
                   order = 52)]
  public class ScriptableInputElementProvider_UGUI_FromPrefab_PreviousNextSelector : ScriptableInputElementProvider_UGUI_FromPrefab_Generic<string>
  {
    [SerializeField]
    [Tooltip("Whether the options should be possible to cycle through. " +
             "If enabled the next option from the last option will be the first option " +
             "and the previous option from the first option will be the last option.")]
    protected bool allowCycle = true;

    [SerializeField]
    [Tooltip("Whether the buttons should represent cycling availability. " +
             "If enabled when cycling is disabled the next button will be disabled when the last option is selected " +
             "and the previous button will be disabled if the first option is selected.")]
    protected bool representNoCycleOnButtons = false;

    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UGUI_PreviousNextSelector.InputFieldType;

    public override bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                                string labelText, SettingsCollection settings,
                                                List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);

      return success && ProviderUtility_UGUI_PreviousNextSelector.UpdateInputElement(elem, settingIdentifier, settings, values,
                                                                                     initialize, allowCycle, representNoCycleOnButtons);
    }
  }
}
