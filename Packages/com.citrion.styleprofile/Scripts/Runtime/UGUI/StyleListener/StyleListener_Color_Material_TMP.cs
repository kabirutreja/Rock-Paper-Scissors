using CitrioN.Common;
using TMPro;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  public enum TMP_MaterialVariable_Color
  {
    Outline,
    Underlay,
    Face
  }

  [HeaderInfo("\n\nChanges a material's color variable of a 'Text Mesh Pro' text component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (TMP Material - Color)")]
  public class StyleListener_Color_Material_TMP : StyleListener_Color
  {
    [Header("Material Color")]

    [SerializeField]
    [Tooltip("If the alpha of the 'Text Mesh Pro' text component should be kept. " +
             "When disabled the alpha from the style change will be applied. " +
             "Useful to retain the alpha while still changing the color itself.")]
    protected bool keepAlpha = true;

    [SerializeField]
    [Tooltip("'Text Mesh Pro' text component reference for which to change the material's color variable.")]
    protected TextMeshProUGUI textElement;

    [SerializeField]
    [Tooltip("The material variable to modify.")]
    protected TMP_MaterialVariable_Color materialVariable = TMP_MaterialVariable_Color.Outline;

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
      var variableName = GetMaterialVariableName();
      base.ApplyChange(color);
      if (textElement != null && !string.IsNullOrEmpty(variableName))
      {
        ScheduleUtility.InvokeDelayedByFrames(() =>
        {
          var material = textElement.fontSharedMaterial;
          //var material = textElement.fontMaterial;
          if (material == null) { return; }
          if (keepAlpha)
          {
            //color.a = material.GetColor(processedKey).a;
            color.a = material.GetColor(variableName).a;
          }
          //material.SetColor(processedKey, color);
          material.SetColor(variableName, color);

          // For some reason we need to create and assign a new material
          // Not sure why this is required for a shared material?!
          textElement.fontSharedMaterial = new Material(material);
          //textElement.fontMaterial = new Material(material);
        });
      }
    }

    protected string GetMaterialVariableName()
    {
      switch (materialVariable)
      {
        case TMP_MaterialVariable_Color.Face:
          return "_FaceColor";
        case TMP_MaterialVariable_Color.Outline:
          return "_OutlineColor";
        case TMP_MaterialVariable_Color.Underlay:
          return "_UnderlayColor";
        default:
          break;
      }
      return string.Empty;
    }
  }
}