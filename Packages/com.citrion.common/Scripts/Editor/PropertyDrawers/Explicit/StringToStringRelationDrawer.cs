using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CustomPropertyDrawer(typeof(StringToStringRelation))]
  public class StringToStringRelationDrawer : PropertyDrawerFromTemplateBase
  {
    protected const string KeyPropertyFieldClass = "property-field__value";

    protected override string PropertyFieldClass => $"property-field__key";

    public override string UxmlPath
      => $"Packages/com.citrion.common/UI Toolkit/UXML/PropertyDrawers/{GetType().Name}.uxml";

    public override string StyleSheetPath
      => $"Packages/com.citrion.common/UI Toolkit/USS/PropertyDrawers/{GetType().Name}";

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      var keyProperty = property.FindPropertyRelative("key");
      UIToolkitEditorExtensions.SetupPropertyField(keyProperty, root, PropertyFieldClass);

      var valueProperty = property.FindPropertyRelative("value");
      UIToolkitEditorExtensions.SetupPropertyField(valueProperty, root, KeyPropertyFieldClass);
    }
  }
}