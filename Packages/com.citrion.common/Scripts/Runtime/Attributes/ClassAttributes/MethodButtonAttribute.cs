using UnityEngine.UIElements;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
  public class MethodButtonAttribute : ClassAttribute
  {
    protected string methodName;

    protected string label;

    protected string tooltip;

    protected string buttonClassName;

    protected bool attachToFooter = false;

    public string MethodName { get => methodName; set => methodName = value; }

    public string Label { get => label; set => label = value; }

    public string Tooltip { get => tooltip; set => tooltip = value; }

    public string ButtonClassName { get => buttonClassName; set => buttonClassName = value; }

    public bool AttachToFooter { get => attachToFooter; set => attachToFooter = value; }

    public MethodButtonAttribute() { }

    public MethodButtonAttribute(string methodName, string label, string tooltip, string buttonClassName)
    {
      this.methodName = methodName;
      this.label = label;
      this.tooltip = tooltip;
      this.buttonClassName = buttonClassName;
    }

    public MethodButtonAttribute(string methodName, string label, string tooltip, string buttonClassName, bool attachToFooter)
      : this(methodName, label, tooltip, buttonClassName)
    {
      this.attachToFooter = attachToFooter;
    }

#if UNITY_EDITOR
    public override void UpdateHierarchy(VisualElement root, SerializedObject serializedObject)
    {
      if (root == null) { return; }
      AddButton(root, serializedObject);
    }

    private void AddButton(VisualElement root, SerializedObject serializedObject)
    {
      VisualElement parent = null;
      if (attachToFooter)
      {
        parent = UIToolkitStyleUtilities.AddFooterToolbar(root, true);
      }
      else
      {
        parent = UIToolkitStyleUtilities.AddHeaderToolbar(root, true);
      }
      UIToolkitStyleUtilities.AddMethodButton(root, parent, ClickAction(serializedObject), ButtonClassName, Label, Tooltip);
    }

    public Action ClickAction(SerializedObject serializedObject)
    {
      if (serializedObject == null) { return null; }

      var method = serializedObject.targetObject.GetType().GetMethod(MethodName);
      Action action = null;
      action += () => method.Invoke(serializedObject.targetObject, null);
      return action;
    }
#endif
  }
}