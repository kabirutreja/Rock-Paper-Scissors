using CitrioN.Common;
using CitrioN.Common.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public static class PreferencesAndPresetsUpdater
  {
    // TODO Should these be moved elsewhere?
    private const string packageId = "com.citrion.settings-menu-creator";
    private const string presetsFolderPath = "Packages/com.citrion.settings-menu-creator/Presets";

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptReload()
    {
      if (EditorApplication.isCompiling || EditorApplication.isUpdating)
      {
        EditorApplication.delayCall += OnScriptReload;
        return;
      }

      bool preferencesExisted = PreferencesUtility.GetOrCreateSettings<Preferences_SettingsMenuCreator_Templates>
        (PreferencesProvider_SettingsMenuCreator_Templates.fileDirectory,
         PreferencesProvider_SettingsMenuCreator_Templates.fileName, out var preferences);

      EditorApplication.delayCall += ()
        => UpdatePreferencesAndPresets(preferences, applyPresets: !preferencesExisted);
    }

    private static void UpdatePreferencesAndPresets(Preferences_SettingsMenuCreator_Templates preferences,
                                                    bool applyPresets)
    {
      if (!PackageUtilities.GetPackageVersion(packageId, out var packageVersion))
      {
        return;
      }

      string appliedVersion = preferences.AppliedPresetsVersion;

      if (appliedVersion == packageVersion)
      {
        return;
      }

      var version = appliedVersion.Split(".");
      if (version.Length >= 2)
      {
        var majorVersion = version[0];
        var minor = version[1];
        if (int.TryParse(minor, out var minorVersion))
        {
          if (majorVersion == "1" && minorVersion < 1)
          {
            DisplayUninstallDialog(appliedVersion);
            return;
          }
          if (majorVersion == "1" && minorVersion <= 2)
          {
            if (EditorUtility.DisplayDialog("Editor Restart Required", "The Settings Menu Creator needs an editor restart to apply changes from the recent update. " +
                                            "Do you want to restart now?", "Restart", "Later"))
            {
              EditorUtilities.RestartEditor();
            }
          }
        }
      }

      PresetUtilities.UpdatePresets(presetsFolderPath);

      if (applyPresets)
      {
        // Apply the default presets for the preferences object
        PresetUtilities.ApplyPresets(preferences);
      }

      // Update the applied version
      preferences.AppliedPresetsVersion = packageVersion;

      EditorUtility.SetDirty(preferences);
      AssetDatabase.SaveAssetIfDirty(preferences);

      if (!Application.isPlaying)
      {
        ManagerWindow_SettingsMenuCreator.ShowWindow_SettingsMenuCreator();
      }
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Uninstall Asset/Remove Settings Menu Creator Core")]
    private static void DisplayUninstallDialog()
    {
      DialogUtilities.DisplayDialog("Remove 'Settings Menu Creator' from project?",
        $"You can click the 'Uninstall' button to remove the core packages or do so later via " +
        $"Tools/CitrioN/Settings Menu Creator/Uninstall Asset/Remove Settings Menu Creator Core. " +
        $"This will not remove the addons so you may be left with error messages in the console until " +
        $"you either remove the addons or import the Settings Menu Creator again. " +
        $"For more control over what parts of the asset to remove you can use the uninstall tab (v1.1+) in " +
        $"the manager window at Tools/CitrioN/Settings Menu Creator/Uninstall. " +
        $"It is highly recommended to always create/have a backup of your project before proceeding with the removal.",
        "Uninstall", "Uninstall later", () => UninstallSettingsMenuCreatorAsset(false));
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Uninstall Asset/Remove Settings Menu Creator")]
    private static void DisplayUninstallDialogWithAddons()
    {
      DialogUtilities.DisplayDialog("Remove 'Settings Menu Creator' and addons from project?",
        $"You can click the 'Uninstall' button to remove the Settings Menu Creator and its addon " +
        $"packages or do so later via Tools/CitrioN/Settings Menu Creator/Uninstall Asset/Remove Settings Menu Creator. " +
        $"For more control over what parts of the asset to remove you can use the uninstall tab (v1.1+) " +
        $"in the manager window at Tools/CitrioN/Settings Menu Creator/Uninstall. " +
        $"It is highly recommended to always create/have a backup of your project before proceeding with the removal.",
        "Uninstall", "Uninstall later", () => UninstallSettingsMenuCreatorAsset(true));
    }

    private static void DisplayUninstallDialog(string appliedVersion)
    {
      DialogUtilities.DisplayDialog("The Settings Menu Creator requires a full reimport!",
        $"Your current version of the Settings Menu Creator ({appliedVersion}) requires a complete reimport " +
        $"of the asset to work correctly. This should only take a minute. You can click the 'Uninstall' button " +
        $"to remove all its packages or do so later via Tools/CitrioN/Settings Menu Creator/Uninstall Asset/Remove Settings Menu Creator Core. " +
        $"Any addons will remain in the project so you might see error messages upon removal. " +
        $"After the removal you can import the latest version again and the errors should no longer show up in the console. " +
        $"It is highly recommended to always create/have a backup of your project before proceeding with the removal and reimport.",
        "Uninstall", "Uninstall later", () => UninstallSettingsMenuCreatorAsset(false));
    }

    public static void UninstallSettingsMenuCreatorAsset(bool includeAddons)
    {
      List<string> packagesToRemove = new List<string>()
      {
        "com.citrion.common",
        "com.citrion.ui",
        "com.citrion.ui.images",
        "com.citrion.ui.components",
        "com.citrion.ui.panels",
        "com.citrion.styleprofile",
        "com.citrion.settings-menu-creator",
        "com.citrion.settings-menu-creator.pro"
      };

      if (includeAddons)
      {
        packagesToRemove.Add("com.citrion.settings-menu-creator.post-processing");
        packagesToRemove.Add("com.citrion.settings-menu-creator.srp");
        packagesToRemove.Add("com.citrion.settings-menu-creator.input");
      }

      var preferences = PreferencesProvider_SettingsMenuCreator_Templates.GetPreferences();
      if (preferences != null)
      {
        preferences.AppliedPresetsVersion = "0";
        EditorUtility.SetDirty(preferences);
        AssetDatabase.SaveAssetIfDirty(preferences);
      }

      PackageUtilities.RemovePackages(packagesToRemove);
    }
  }
}