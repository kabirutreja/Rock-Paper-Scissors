using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the display of a 'StepSlider' component's value.")]
  [SkipObfuscationRename]
  public class StepSliderValueDisplay : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The step slider for which to display the value for.")]
    protected StepSlider slider;

    [SerializeField]
    [Tooltip("The text element in which to show the slider value.")]
    protected TextMeshProUGUI textComponent;

    private void Awake()
    {
      if (slider != null)
      {
        slider.onValueChanged.AddListener(UpdateDisplay);
      }
    }

    private void OnDestroy()
    {
      if (slider != null)
      {
        slider.onValueChanged.RemoveListener(UpdateDisplay);
      }
    }

    private void OnEnable()
    {
      // Update the value display a frame later
      // to ensure that any value changes have been applied
      this.InvokeDelayedByFrames(UpdateDisplay);
    }

    private void UpdateDisplay(float value)
    {
      if (textComponent == null || slider == null) { return; }
      textComponent.SetText(slider.GetValueString());
    }

    [SkipObfuscationRename]
    public void UpdateDisplay()
    {
      if (slider == null) { return; }
      UpdateDisplay(slider.value);
    }
  }
}