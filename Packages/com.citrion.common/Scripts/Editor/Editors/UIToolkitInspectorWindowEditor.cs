using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  /// <summary>
  /// Required for Unity 2021 because the inspector window
  /// will otherwise be drawn with the old system (imgui).
  /// This script ensures UI Toolkit is used for drawing
  /// which consequently allows property drawers that are
  /// using UI Toolkit to work.
  /// </summary>
  [CanEditMultipleObjects]
  public class UIToolkitInspectorWindowEditor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();
      InspectorElement.FillDefaultInspector(root, serializedObject, this);
      return root;
    }
  }
}