using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomEditor(typeof(Preferences_SettingsMenuCreator_Templates))]
  public class Preferences_SettingsMenuCreator_Templates_Editor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();

      // Add the default inspector
      InspectorElement.FillDefaultInspector(root, serializedObject, this);

#if !UNITY_2022_1_OR_NEWER
      // We need to manually insert the headers here
      // because Unity does not draw them in the default inspector in Unity 2021
      Common.UIToolkitUtilities.InsertElementBefore(root, "PropertyField:menuTemplate", Common.UIToolkitUtilities.CreateHeaderLabel("UGUI"));
      Common.UIToolkitUtilities.InsertElementBefore(root, "PropertyField:menuTemplateUIToolkit", Common.UIToolkitUtilities.CreateHeaderLabel("UI Toolkit"));
#endif

      // Add some spacing
      var spacer = new VisualElement();
      spacer.style.height = 10;
      root.Add(spacer);

      // Create and add a button for refreshing the preferences
      var refreshButton = new Button(RefreshPreferences);
      refreshButton.text = "Refresh";
      refreshButton.style.height = 30;
      root.Add(refreshButton);

      return root;
    }

    private void RefreshPreferences()
    {
      var targetObject = serializedObject.targetObject;
      if (targetObject != null && targetObject is Preferences_SettingsMenuCreator_Templates script)
      {
        script.Refresh();
      }
    }
  }
}