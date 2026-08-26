using CitrioN.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomEditor(typeof(SettingsSaver_PlayerPrefs))]
  public class SettingsSaver_PlayerPrefsEditor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();

      // Add the default inspector
      InspectorElement.FillDefaultInspector(root, serializedObject, this);
      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);

      // Add some spacing
      var spacer = new VisualElement();
      spacer.style.height = 10;
      root.Add(spacer);

      var removePlayerPrefsSettingsKeyButton = new Button(RemovePlayerPrefsSettingsKey);
      removePlayerPrefsSettingsKeyButton.text = "Remove Player Prefs Settings Key";
      root.Add(removePlayerPrefsSettingsKeyButton);

      return root;
    }

    private void RemovePlayerPrefsSettingsKey()
    {
      var targetObject = serializedObject.targetObject;
      if (targetObject != null && targetObject is SettingsSaver_PlayerPrefs saver)
      {
        saver.RemovePlayerPrefsSettingsKey();
      }
    }
  }
}