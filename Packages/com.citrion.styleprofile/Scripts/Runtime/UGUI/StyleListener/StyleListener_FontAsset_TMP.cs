using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  [HeaderInfo("\n\nListens to a style change of type 'TMP_FontAsset'." +
              "\n\nChanges the text font of a 'Text Mesh Pro' text component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (TMP Font Asset)")]
  public class StyleListener_FontAsset_TMP : GenericStyleListener<TMP_FontAsset>
  {
    [Header("Font")]

    [SerializeField]
    [Tooltip("'Text Mesh Pro' text component reference for which to change the font.")]
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

    protected override void ApplyChange(TMP_FontAsset fontAsset)
    {
      base.ApplyChange(fontAsset);
      if (fontAsset != null && textElement != null)
      {
        textElement.font = fontAsset;
      }
    }
  }
}