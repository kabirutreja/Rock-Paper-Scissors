using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  public enum TMP_Text_Variable_Float
  {
    Alpha,
    CharacterSpacing,
    CharacterWidthAdjustment,
    FontSize,
    FontSizeMin,
    FontSizeMax,
    LineSpacing,
    LineSpacingAdjustment,
    MappingUvLineOffset,
    OutlineWidth,
    ParagraphSpacing,
    WordSpacing,
    WordWrappingRatios
  }

  [HeaderInfo("\n\nAllows float modifications on a 'Text Mesh Pro' text component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (TMP Text - Float)")]
  public class StyleListener_Float_Text_TMP : StyleListener_Float
  {
    [Header("Text (TMP)")]
    [Space(10)]

    [SerializeField]
    [Tooltip("Text Mesh Pro text component reference for which to change the float variable.")]
    protected TextMeshProUGUI textComponent;

    [SerializeField]
    [Tooltip("The text material variable to modify.")]
    protected TMP_Text_Variable_Float variableToChange = TMP_Text_Variable_Float.FontSize;

    protected void Reset()
    {
      CacheTextComponent();
    }

    protected override void Awake()
    {
      base.Awake();
      CacheTextComponent();
    }

    private void CacheTextComponent()
    {
      if (textComponent == null)
      {
        textComponent = GetComponent<TextMeshProUGUI>();
      }
    }

    protected override void ApplyChange(float value)
    {
      base.ApplyChange(value);
      if (textComponent != null)
      {
        switch (variableToChange)
        {
          case TMP_Text_Variable_Float.Alpha:
            textComponent.alpha = value;
            break;
          case TMP_Text_Variable_Float.CharacterSpacing:
            textComponent.characterSpacing = value;
            break;
          case TMP_Text_Variable_Float.CharacterWidthAdjustment:
            textComponent.characterWidthAdjustment = value;
            break;
          case TMP_Text_Variable_Float.FontSize:
            textComponent.fontSize = value;
            break;
          case TMP_Text_Variable_Float.FontSizeMin:
            textComponent.fontSizeMin = value;
            break;
          case TMP_Text_Variable_Float.FontSizeMax:
            textComponent.fontSizeMax = value;
            break;
          case TMP_Text_Variable_Float.LineSpacing:
            textComponent.lineSpacing = value;
            break;
          case TMP_Text_Variable_Float.LineSpacingAdjustment:
            textComponent.lineSpacingAdjustment = value;
            break;
          case TMP_Text_Variable_Float.MappingUvLineOffset:
            textComponent.mappingUvLineOffset = value;
            break;
          case TMP_Text_Variable_Float.OutlineWidth:
            textComponent.outlineWidth = value;
            break;
          case TMP_Text_Variable_Float.ParagraphSpacing:
            textComponent.paragraphSpacing = value;
            break;
          case TMP_Text_Variable_Float.WordSpacing:
            textComponent.wordSpacing = value;
            break;
          case TMP_Text_Variable_Float.WordWrappingRatios:
            textComponent.wordWrappingRatios = value;
            break;
          default:
            break;
        }
      }
    }
  }
}