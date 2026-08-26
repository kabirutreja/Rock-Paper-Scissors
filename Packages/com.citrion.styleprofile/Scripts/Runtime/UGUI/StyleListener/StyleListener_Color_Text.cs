using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  [HeaderInfo("\n\nChanges to text color of a 'Text Mesh Pro' text component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (TMP Text - Color)")]
  public class StyleListener_Color_Text : StyleListener_Color
  {
    [Header("Text Color")]

    [SerializeField]
    [Tooltip("If the alpha of the 'Text Mesh Pro' text component should be kept. " +
             "When disabled the alpha from the style change will be applied. " +
             "Useful to retain the alpha while still changing the color itself.")]
    protected bool keepAlpha = true;

    [SerializeField]
    [Tooltip("'Text Mesh Pro' text component reference for which to change the text color.")]
    protected TextMeshProUGUI textElement;

    private void Reset()
    {
      CacheTextElement();
    }

    protected override void Awake()
    {
      base.Awake();
      CacheTextElement();
    }

    private void CacheTextElement()
    {
      if (textElement == null)
      {
        textElement = GetComponent<TextMeshProUGUI>();
      }
    }

    protected override void ApplyChange(Color color)
    {
      base.ApplyChange(color);
      if (textElement != null)
      {
        if (keepAlpha)
        {
          color.a = textElement.color.a;
        }
        textElement.color = color;
      }
    }
  }
}