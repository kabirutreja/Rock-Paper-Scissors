using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CustomPropertyDrawer(typeof(UnityPackageObjectAttribute))]
  public class UnityPackageObjectAttributeDrawer : PropertyDrawerFromTemplateBase
  {
    PropertyField propertyField = null;

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      base.SetupVisualElements(property, root);

      propertyField = root.Q<PropertyField>(className: PropertyFieldClass);

      propertyField?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnValueChanged);
    }

    private void OnValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
      var asset = evt.newValue;

      if (asset == null) { return; }

      var assetPath = AssetDatabase.GetAssetPath(asset);
      if (!Path.HasExtension(assetPath) ||
           Path.GetExtension(assetPath) != ".unitypackage")
      {
        var objectField = propertyField.Q<ObjectField>();

        if (objectField != null)
        {
          objectField.value = null;

          //EditorApplication.delayCall += () =>
          //{
          //  var so = property.serializedObject;
          //  var obj = so.targetObject;
          //  so.ApplyModifiedProperties();
          //  so.Update();
          //  EditorUtility.SetDirty(obj);
          //};
        }
      }
    }
  }
}
