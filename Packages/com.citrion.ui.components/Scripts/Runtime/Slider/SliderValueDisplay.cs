using CitrioN.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the display of a 'Slider' component's value.")]
  [SkipObfuscationRename]
  public class SliderValueDisplay : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The slider for which to display the value for.")]
    protected Slider slider;

    [SerializeField]
    [Tooltip("The text element in which to show the slider value.")]
    protected TextMeshProUGUI textComponent;

    [SerializeField]
    [Tooltip("Whether the slider value should be displayed as an integer.")]
    protected bool forceToInt = false;

    [SerializeField]
    [Range(0, 3)]
    [Tooltip("The amount of decimal numbers to display.")]
    protected int decimals = 1;

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
      string valueString = forceToInt ? Mathf.FloorToInt(value).ToString() :
                           value.Truncate(decimals).ToString();
      if (textComponent != null)
      {
        textComponent.SetText(valueString);
      }
    }

    [SkipObfuscationRename]
    public void UpdateDisplay()
    {
      if (slider == null) { return; }
      UpdateDisplay(slider.value);
    }
  }
}