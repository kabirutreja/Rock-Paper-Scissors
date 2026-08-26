using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
  public class HideScriptFieldAttribute : ClassAttribute
  {
    public HideScriptFieldAttribute() { }

#if UNITY_EDITOR
    public override void UpdateHierarchy(VisualElement root, SerializedObject serializedObject)
    {
      if (root == null || serializedObject == null) { return; }

      var scriptField = root.Q<PropertyField>(UIToolkitStyleUtilities.SCRIPT_FIELD_NAME);
      scriptField?.Show(false);
    }
#endif
  }
}