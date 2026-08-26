using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CanEditMultipleObjects]
  //#if !UNITY_2022_1_OR_NEWER
  [CustomEditor(typeof(ScriptableObject), editorForChildClasses: true, isFallback = true)]
  //#endif
  public class UIToolkitScriptableObjectEditor : UIToolkitInspectorWindowEditor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = base.CreateInspectorGUI();
      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);
      return root;
    }
  }
}