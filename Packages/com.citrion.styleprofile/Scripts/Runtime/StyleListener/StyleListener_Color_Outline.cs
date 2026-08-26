using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.StyleProfileSystem
{
  [HeaderInfo("\n\nChanges the color of the referenced Outline component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (Outline - Color)")]
  public class StyleListener_Color_Outline : StyleListener_Color
  {
    [Header("Image Color")]

    [SerializeField]
    [Tooltip("Outline component reference for which to change the color.")]
    protected Outline outline;

    [SerializeField]
    protected bool keepAlpha = false;

    [Range(-360f, 360f)]
    [SerializeField]
    [Tooltip("Hue offset to apply to the color.")]
    protected int hueOffset = 0;

    [Range(-1f, 1f)]
    [SerializeField]
    [Tooltip("Saturation offset to apply to the color.")]
    protected float saturationOffset = 0f;

    [Range(-1f, 1f)]
    [SerializeField]
    [Tooltip("Value offset to apply to the color.")]
    protected float valueOffset = 0f;

    protected void Reset()
    {
      CacheOutline();
    }

    protected override void OnEnable()
    {
      base.OnEnable();

      // TODO fetch the actual color
    }

    protected override void Awake()
    {
      base.Awake();
      CacheOutline();
    }

    private void CacheOutline()
    {
      if (outline == null)
      {
        outline = GetComponent<Outline>();
      }
    }

    protected override void ApplyChange(Color color)
    {
      base.ApplyChange(color);
      if (outline != null)
      {
        if (hueOffset != 0 || saturationOffset != 0f || valueOffset != 0f)
        {
          Color.RGBToHSV(color, out var hue, out var saturation, out var value);

          if (hueOffset != 0)
          {
            hue = Mathf.Clamp01(hue + (hueOffset / 360f));
          }

          if (saturationOffset != 0f)
          {
            saturation = Mathf.Clamp01(saturation + saturationOffset);
          }

          if (valueOffset != 0f)
          {
            value = Mathf.Clamp01(value + valueOffset);
          }

          color = Color.HSVToRGB(hue, saturation, value);
        }

        if (keepAlpha)
        {
          color.a = outline.effectColor.a;
        }

        outline.effectColor = color;
      }
    }
  }
}