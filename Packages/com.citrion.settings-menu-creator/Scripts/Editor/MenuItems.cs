using CitrioN.Common.Editor;
using UnityEditor;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public static class MenuItems
  {
    //[MenuItem("Tools/CitrioN/Settings Menu Creator/Scene/Add settings menu (UGUI)")]
    //private static void AddSettingsMenuToScene_UGUI()
    //{
    //  AddSettingsMenuToSceneInternal_UGUI(false);
    //}

    //[MenuItem("Tools/CitrioN/Settings Menu Creator/Scene/Add settings menu with layout template (UGUI)")]
    //private static void AddSettingsMenuWithLayoutToScene_UGUI()
    //{
    //  AddSettingsMenuToSceneInternal_UGUI(true);
    //}

    private static void AddSettingsMenuToSceneInternal_UGUI(bool assignLayout)
    {
      PreferencesUtility.GetOrCreateSettings<Preferences_SettingsMenuCreator_Templates>
        (PreferencesProvider_SettingsMenuCreator_Templates.fileDirectory,
         PreferencesProvider_SettingsMenuCreator_Templates.fileName, out var preferences);

      if (preferences == null) { return; }

      var menuTemplate = preferences.MenuTemplate_UGUI;
      if (menuTemplate == null) { return; }

      var menuInstance = PrefabUtility.InstantiatePrefab(menuTemplate.gameObject) as GameObject;

      EditorGUIUtility.PingObject(menuInstance);

      if (!assignLayout) { return; }

      var menuLayoutTemplate = preferences.MenuLayoutTemplate_UGUI;
      SettingsMenu_UGUI menuComponent = null;
      if (menuInstance != null)
      {
        menuComponent = menuInstance.GetComponent<SettingsMenu_UGUI>();
      }
      if (menuLayoutTemplate != null && menuComponent != null)
      {
        menuComponent.MenuTemplate = menuLayoutTemplate;
      }
    }
  }
}