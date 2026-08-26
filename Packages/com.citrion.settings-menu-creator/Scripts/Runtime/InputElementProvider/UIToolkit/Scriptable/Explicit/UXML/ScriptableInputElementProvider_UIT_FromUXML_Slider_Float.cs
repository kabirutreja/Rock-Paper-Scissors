using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UIT_FromUXML_Slider_Float_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Slider Float",
                   order = 124)]
  public class ScriptableInputElementProvider_UIT_FromUXML_Slider_Float : ScriptableInputElementProvider_UIT_FromUXML_Generic<Slider, float>
  {
    [SerializeField]
    [Tooltip("Whether an input field should be shown next to the slider.")]
    protected bool showInputField = false;
    [SerializeField]
    [Tooltip("Whether the slider should be inverted.")]
    protected bool inverted = false;
    [SerializeField]
    [Tooltip("The direction of the slider.")]
    protected SliderDirection direction = SliderDirection.Horizontal;

    public override Type GetInputFieldType(SettingsCollection settings)
      => ProviderUtility_UIT_Slider_Float.InputFieldType;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      //if (!IsCorrectInputElement(elem, settings)) { return false; }

      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      return success && ProviderUtility_UIT_Slider_Float.UpdateInputElement(elem, settingIdentifier, settings,
                                                                            labelText, values, initialize,
                                                                            minSliderValue: 0, maxSliderValue: 1,
                                                                            showInputField, inverted, direction);
    }
  }
}
