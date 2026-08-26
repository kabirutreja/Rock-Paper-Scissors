using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.Common
{
  [System.AttributeUsage(System.AttributeTargets.Class)]
  public abstract class ClassAttribute : System.Attribute
  {
#if UNITY_EDITOR
    public abstract void UpdateHierarchy(VisualElement root, UnityEditor.SerializedObject serializedObject);

    public static void ApplyClassAttributesToHierarchy(VisualElement root, SerializedObject serializedObject)
    {
      if (root == null) { return; }

      var obj = serializedObject?.targetObject;
      if (obj == null) { return; }

      // We reverse the order so child attributes will be executed last.
      // This allows attributes in child classes to affect attributes of their bases classes.
      var classAttributes = obj.GetType().GetCustomAttributes<ClassAttribute>().Reverse();
      if (classAttributes != null)
      {
        foreach (var classAttribute in classAttributes)
        {
          classAttribute?.UpdateHierarchy(root, serializedObject);
        }
      }
      return;
    }
#endif
  }
}