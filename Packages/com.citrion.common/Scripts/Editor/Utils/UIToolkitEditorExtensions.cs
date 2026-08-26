using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public static class UIToolkitEditorExtensions
  {
    public const string DEBUG_CLASS = "debug";

    public static Foldout GetPropertyFieldFoldout(this PropertyField field)
    {
      if (field == null) { return null; }
      var childs = field.Children();
      if (childs != null && childs.Count() > 0)
      {
        var firstChild = childs.First();
        if (firstChild != null && firstChild is Foldout foldout)
        {
          return foldout;
        }
      }
      return null;
    }

    public static void SetPropertyFieldFoldoutValue(this PropertyField field, bool value)
    {
      var foldout = GetPropertyFieldFoldout(field);
      if (foldout != null)
      {
        foldout.value = value;
      }
    }

    public static void OpenPropertyFieldFoldout(this PropertyField field)
      => field?.SetPropertyFieldFoldoutValue(true);

    public static void ClosePropertyFieldFoldout(this PropertyField field)
      => field?.SetPropertyFieldFoldoutValue(false);

    public static VisualElement CreateVisualElementFromTemplate
      (string UxmlPath, string styleSheetPath = "", string rootClass = "")
    {
      var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);

      var root = template != null ? template.CloneTree() : new VisualElement();
      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{styleSheetPath}.uss");
      if (styleSheet != null)
      {
        root.AddStyleSheets(styleSheet);
      }
      else
      {
        styleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>($"{styleSheetPath}.tss");
        if (styleSheet != null)
        {
          root.AddStyleSheets(styleSheet);
        }
      }

      if (!string.IsNullOrEmpty(rootClass))
      {
        root.AddToClassList(rootClass);
      }
      return root;
    }

    public static VisualElement CreateVisualElementFromTemplate(VisualTreeAsset template)
    {
      var root = template != null ? template.CloneTree() : new VisualElement();
      return root;
    }

    public static PropertyField SetupPropertyField(SerializedProperty property, VisualElement root, string className)
    {
      if (property == null)
      {
        ConsoleLogger.LogWarning($"Property for {className} is null!");
        return null;
      }

      var propertyField = root.Q<PropertyField>(className: className);

      if (propertyField == null)
      {
        propertyField = new PropertyField();
        propertyField.AddToClassList(className);
        root.Add(propertyField);
      }

      try
      {
        propertyField.BindProperty(property);
      }
      catch (System.Exception)
      {
        //ConsoleLogger.Log(e);

        // TODO Could potentially be removed if Unity bug is fixed
        // This is here because when changing the UnitySetting Unity occasionally fails to properly update the
        // options field.
        // The error is: System.ObjectDisposedException:
        // SerializedProperty settings.Array.data[0].setting.unitySetting.options has disappeared!
        // The actual options list seems to be internally updated though so binding the property on the next cycle
        // seems to be a workaround fix for this issue

        // Old
        //EditorApplication.delayCall += () => propertyField.BindProperty(property);

        // New
        EditorApplication.delayCall += () =>
        {
          if (propertyField != null && property != null)
          {
            propertyField.BindProperty(property);
          }
        };

        //var serializedObject = property.serializedObject;
        //serializedObject.Update();
        //serializedObject.ApplyModifiedProperties();
        //serializedObject.SetIsDifferentCacheDirty();
        //throw;
      }
      //propertyField.OpenPropertyFieldFoldout();

      return propertyField;
    }

    public static HelpBox AddHelpBoxWithDebugClass(this VisualElement elem,
           string text, bool createIfNoText = false, HelpBoxMessageType messageType = HelpBoxMessageType.None)
    {
      if (elem == null || string.IsNullOrEmpty(text)) { return null; }
      if (!createIfNoText && string.IsNullOrEmpty(text)) { return null; }
      var helpBox = new HelpBox(text, messageType);
      helpBox.AddToClassList(DEBUG_CLASS);
      elem.Add(helpBox);
      return helpBox;
    }

    public static HelpBox AddHelpBoxWithDebugClass(this VisualElement elem, StringToStringRelationProfile mapping,
       string key, bool createIfNoText = false, HelpBoxMessageType messageType = HelpBoxMessageType.None)
    {
      string text = null;
      if (mapping != null) { text = mapping.GetValue(key); }
      return elem.AddHelpBoxWithDebugClass(text, createIfNoText, messageType);
    }
  }
}