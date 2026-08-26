using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  // TODO Rename this to PropertyDrawerFromTemplateWithPropertyField
  public abstract class PropertyDrawerFromTemplateBase : PropertyDrawerFromTemplate
  {
    protected virtual string PropertyFieldClass => $"property-field";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      var root = base.CreatePropertyGUI(property);

      Setup(property, root);

      return root;
    }

    private VisualElement GetDrawerRootElement(VisualElement elem)
    {
      VisualElement parent = elem;

      do
      {
        parent = parent.parent?.parent;
      } while (parent != null && parent.ClassListContains(RootClass));

      return parent != null ? parent : elem;
    }

    protected virtual void Setup(SerializedProperty property, VisualElement root)
    {
      UpdateFields(property, GetDrawerRootElement(root));
    }

    private void UpdateFields(SerializedProperty property, VisualElement root)
    {
      SetupVisualElements(property, root);
    }

    protected virtual void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      UIToolkitEditorExtensions.SetupPropertyField(property, root, PropertyFieldClass);
    }
  }
}