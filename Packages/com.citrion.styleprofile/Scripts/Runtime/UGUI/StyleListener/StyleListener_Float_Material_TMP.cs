using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  public enum TMP_MaterialVariable_Float
  {
    OutlineWidth,
    OutlineSoftness,
    UnderlayOffsetX,
    UnderlayOffsetY,
    UnderlayDilate,
    UnderlaySoftness,
    //FaceDilate
  }

  [HeaderInfo("\n\nChanges a material's float variable of a 'Text Mesh Pro' text component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (TMP Material - Float)")]
  public class StyleListener_Float_Material_TMP : StyleListener_Float
  {
    protected const string TMP_UNDERLAY_KEYWORD = "UNDERLAY_ON";

    [Header("Material Float")]

    [SerializeField]
    [Tooltip("'Text Mesh Pro' text component reference for which to change the material's float variable.")]
    protected TextMeshProUGUI textElement;

    [SerializeField]
    [Tooltip("The material variable to modify.")]
    protected TMP_MaterialVariable_Float materialVariable = TMP_MaterialVariable_Float.OutlineWidth;

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

    protected override void ApplyChange(float value)
    {
      var variableName = GetMaterialVariableName();
      base.ApplyChange(value);
      if (textElement != null && !string.IsNullOrEmpty(variableName))
      {
        ScheduleUtility.InvokeDelayedByFrames(() =>
        {
          var material = textElement.fontSharedMaterial;
          if (material == null) { return; }
          EnableRequiredKeywords();
          material.SetFloat(variableName, value);
          textElement.fontSharedMaterial = new Material(material);
          //textElement.SetMaterialDirty();
        });
      }
    }

    protected virtual void EnableRequiredKeywords()
    {
      if (textElement == null) { return; }
      var material = textElement.fontSharedMaterial;
      if (material == null) { return; }

      string keyword = string.Empty;

      switch (materialVariable)
      {
        case TMP_MaterialVariable_Float.OutlineWidth:
          break;
        case TMP_MaterialVariable_Float.OutlineSoftness:
          break;
        case TMP_MaterialVariable_Float.UnderlayOffsetX:
          keyword = TMP_UNDERLAY_KEYWORD; break;
        case TMP_MaterialVariable_Float.UnderlayOffsetY:
          keyword = TMP_UNDERLAY_KEYWORD; break;
        case TMP_MaterialVariable_Float.UnderlayDilate:
          keyword = TMP_UNDERLAY_KEYWORD; break;
        case TMP_MaterialVariable_Float.UnderlaySoftness:
          keyword = TMP_UNDERLAY_KEYWORD; break;
        default:
          break;
      }

      if (string.IsNullOrEmpty(keyword)) { return; }
      material.EnableKeyword(keyword);
    }

    protected string GetMaterialVariableName()
    {
      switch (materialVariable)
      {
        case TMP_MaterialVariable_Float.OutlineWidth:
          return "_OutlineWidth";
        case TMP_MaterialVariable_Float.OutlineSoftness:
          return "_OutlineSoftness";
        case TMP_MaterialVariable_Float.UnderlayOffsetX:
          return "_UnderlayOffsetX";
        case TMP_MaterialVariable_Float.UnderlayOffsetY:
          return "_UnderlayOffsetY";
        case TMP_MaterialVariable_Float.UnderlayDilate:
          return "_UnderlayDilate";
        case TMP_MaterialVariable_Float.UnderlaySoftness:
          return "_UnderlaySoftness";
        default:
          break;
      }
      return string.Empty;
    }
  }
}