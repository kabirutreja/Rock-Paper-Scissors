using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.UI.UIToolkit
{
  [HeaderInfo("\n\nNested Panel:\nPanel that is using a 'UI Element' as its root. " +
              "Useful if only a part of a 'UI Document' should serve as the panel.")]
  public class UIT_NestedPanel : UIT_Panel
  {
    [Header("Nested Panel")]
    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("The name of the visual element to serve as the root of the panel.")]
    protected string rootElementName = string.Empty;

#if UNITY_2021_1_OR_NEWER || LEGACY_UI_TOOLKIT
    protected override VisualElement GetRoot()
    {
      var documentRoot = base.GetRoot();
      if (string.IsNullOrEmpty(rootElementName)) { return documentRoot; }
      var panelRoot = documentRoot.Q<VisualElement>(rootElementName);
      if (panelRoot == null)
      {
        ConsoleLogger.LogWarning($"Unable to find panel root element with name {rootElementName} for {this.name}");
        return documentRoot;
      }
      return panelRoot;
    }
#endif
  }
}