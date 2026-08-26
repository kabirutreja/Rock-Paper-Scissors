using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CustomPropertyDrawer(typeof(StringToGenericDataRelation<>))]
  public class StringToGenericDataRelationDrawer : PropertyDrawerFromTemplateBase
  {
    protected const string KeyPropertyFieldClass = "property-field__value";

    protected override string PropertyFieldClass => "property-field__key";

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      var keyProperty = property.FindPropertyRelative("key");
      var keyPropertyField = UIToolkitEditorExtensions.SetupPropertyField(keyProperty, root, PropertyFieldClass);

      var valueProperty = property.FindPropertyRelative("value");
      var valuePropertyField = UIToolkitEditorExtensions.SetupPropertyField(valueProperty, root, KeyPropertyFieldClass);
    }
  }
}