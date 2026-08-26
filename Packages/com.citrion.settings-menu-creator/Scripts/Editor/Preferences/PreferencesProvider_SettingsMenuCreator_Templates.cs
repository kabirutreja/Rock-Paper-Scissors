using CitrioN.Common;
using CitrioN.Common.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  static class PreferencesProvider_SettingsMenuCreator_Templates
  {
    public const string fileDirectory = "Assets/CitrioN/SMC/Preferences/";
    public const string fileName = "Preferences_SettingsMenuCreator_Templates.asset";
    public const string LABEL_TITLE_CLASS_NAME = "label__title";

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Refresh Preferences")]
    public static void RefreshPreferences()
    {
      var settings = PreferencesUtility.GetSerializedSettings<Preferences_SettingsMenuCreator_Templates>
                                                                 (fileDirectory, fileName);
      var preferences = settings.targetObject as Preferences_SettingsMenuCreator_Templates;
      if (preferences != null)
      {
        //ConsoleLogger.Log("Refresh");
        preferences.Refresh();
      }
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
      var provider = new SettingsProvider("Preferences/Settings Menu Creator/Templates", SettingsScope.User)
      {
        label = "Templates",

        // Called when the user clicks on the settings item in the settings window
        activateHandler = (searchContext, root) =>
        {
          var settings = PreferencesUtility.GetSerializedSettings<Preferences_SettingsMenuCreator_Templates>
                                                                 (fileDirectory, fileName);

          string styleSheetPath = $"Packages/com.citrion.settings-menu-creator/UI Toolkit/USS/Editors/" +
                                  $"{typeof(Preferences_SettingsMenuCreator_Templates).Name}";
          // TODO Move this into some common script
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

          // If any elements are added to the root the OnGUI function
          // will not be called and UI Toolkit will be used for drawing instead
          var titleLabel = new Label("Settings Menu Creator");
          titleLabel.AddToClassList(LABEL_TITLE_CLASS_NAME);
          root.Add(titleLabel);
          root.Add(new InspectorElement(settings));
          root.Bind(settings);
        },
        keywords = new HashSet<string>(new[] { "Settings", "Menu", "Creator" })
      };

      return provider;
    }

    public static Preferences_SettingsMenuCreator_Templates GetPreferences()
    {
      var settings = PreferencesUtility.GetSerializedSettings<Preferences_SettingsMenuCreator_Templates>
                                                           (fileDirectory, fileName);
      return settings.targetObject as Preferences_SettingsMenuCreator_Templates;
    }
  }
}