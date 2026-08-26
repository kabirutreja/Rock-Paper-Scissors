using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class PropertyDrawerFromVisualTreeAsset : PropertyDrawer
  {
    public virtual string UxmlPath => $"Packages/com.citrion.common/UI Toolkit/UXML/PropertyDrawers/{GetType().Name}.uxml";

    public virtual string StyleSheetPath => $"Packages/com.citrion.common/UI Toolkit/USS/PropertyDrawers/{GetType().Name}";

    protected SerializedProperty property;
    protected SerializedObject serializedObject;

    protected VisualElement root;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      this.property = property;
      serializedObject = property.serializedObject;

      return UIToolkitEditorExtensions.CreateVisualElementFromTemplate(UxmlPath, StyleSheetPath, GetType().Name);
    }
  }
}