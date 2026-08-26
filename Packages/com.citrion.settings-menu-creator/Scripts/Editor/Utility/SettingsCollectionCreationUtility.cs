using CitrioN.Common;
using CitrioN.Common.Editor;
using CitrioN.StyleProfileSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_POST_PROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif

namespace CitrioN.SettingsMenuCreator.Editor
{
  public static class SettingsCollectionCreationUtility
  {
    private static Dictionary<Object, Object> duplicatedAssets = new Dictionary<Object, Object>();

    private static void Cleanup()
    {
      if (duplicatedAssets == null) { return; }
      foreach (var item in duplicatedAssets)
      {
        var duplicate = item.Value;

        if (duplicate is ThemeStyleSheet tss)
        {
          // Due to Unity importing a duplicated ThemeStyleSheet
          // with a warning if there is another ThemeStyleSheet
          // referenced we need to reimport the asset so the warning
          // has a chance to be removed
          var path = AssetDatabase.GetAssetPath(tss);
          var fullPath = FileUtility.GetFileDirectory(path) + "/" + tss.name + ".tss";
          var imports = tss.GetType().GetPrivateField("imports");
          var importsValue = imports.GetValue(tss);

          string importUrlString = "@import url";

          var enumerable = importsValue as IEnumerable;
          var elementType = enumerable.GetType().GetElementType();
          List<StyleSheet> styleSheets = new List<StyleSheet>();

          foreach (var entry in enumerable)
          {
            var styleSheetField = elementType.GetField("styleSheet");
            var styleSheet = styleSheetField.GetValue(entry);
            styleSheets.Add(styleSheet as StyleSheet);
          }

          try
          {
            // Read all lines from the document
            string[] lines = File.ReadAllLines(fullPath);
            List<int> importUrlLineIndices = new List<int>();

            // Modify lines starting with the specified substring
            for (int i = 0; i < lines.Length; i++)
            {
              if (lines[i].StartsWith(importUrlString))
              {
                importUrlLineIndices.Add(i);
              }
            }

            if (styleSheets.Count == importUrlLineIndices.Count)
            {
              for (int i = 0; i < styleSheets.Count; i++)
              {
                var styleSheet = styleSheets[i];
                var lineIndex = importUrlLineIndices[i];
                var assetPath = AssetDatabase.GetAssetPath(styleSheet);

                lines[lineIndex] = $"@import url(\"/{assetPath}\");";
              }
            }

            // Write the modified lines back to the document
            File.WriteAllLines(fullPath, lines);
          }
          catch (System.Exception e)
          {
            ConsoleLogger.LogError(e.Message);
          }

          RefreshThemeStyleSheet(tss);
          //AssetDatabase.ImportAsset(fullPath);
        }
      }
      duplicatedAssets.Clear();
    }

    [MenuItem("CONTEXT/ThemeStyleSheet/Refresh", priority = 110)]
    public static void RefreshThemeStyleSheet(MenuCommand command)
    {
      var tss = (ThemeStyleSheet)command.context;
      RefreshThemeStyleSheet(tss);
    }

    private static void RefreshThemeStyleSheet(ThemeStyleSheet tss)
    {
      if (tss == null) { return; }
      var type = typeof(ThemeStyleSheet);
      var flattenMethod = type.GetPrivateMethod("FlattenImportedStyleSheetsRecursive");
      var finalizeMethod = type.GetPrivateMethod("Finalize");
      flattenMethod?.Invoke(tss, new object[] { });
      finalizeMethod?.Invoke(tss, new object[] { });

      AssetDatabase.Refresh();
      EditorUtility.SetDirty(tss);
      AssetDatabase.SaveAssetIfDirty(tss);
    }

    [MenuItem("Assets/Create/CitrioN/Settings Menu Creator/Settings Collection/From Preferences", false, 1)]
    private static void MenuItem_CreateSettingsCollectionAndResourcesFromPreset()
    {
      CreateSettingsCollectionAndResources(null, true, true);
    }

    //[MenuItem("Assets/Create/CitrioN/Settings Menu Creator/Settings Collection/Duplicate SettingsCollection (incl. Resources)", false, 1)]
    //private static void MenuItem_DuplicateSelectedSettingsCollectionAndResourcesFromPreset()
    //{
    //  if (Selection.activeObject is SettingsCollection collection)
    //  {
    //    CreateSettingsCollectionAndResources(collection, true, true);
    //  }
    //  else
    //  {
    //    ConsoleLogger.LogWarning($"A '{nameof(SettingsCollection)}' needs to be selected to perform a duplication.");
    //  }
    //}

    [MenuItem("Assets/Create/CitrioN/Settings Menu Creator/Provider Collection/UGUI/From Preferences", false, 5)]
    private static void MenuItem_CreateInputElementProvidersAndResources_UGUI()
    {
      PreferencesUtility.GetOrCreateSettings<Preferences_SettingsMenuCreator_Templates>
        (PreferencesProvider_SettingsMenuCreator_Templates.fileDirectory,
         PreferencesProvider_SettingsMenuCreator_Templates.fileName, out var preferences);
      var provider = preferences != null ? preferences.InputElementProviders_UGUI : null;
      CreateInputElementProvidersAndResources_UGUI(provider);
    }

    [MenuItem("Assets/Create/CitrioN/Settings Menu Creator/Provider Collection/UI Toolkit/From Preferences", false, 6)]
    private static void MenuItem_CreateInputElementProvidersAndResources_UIT()
    {
      PreferencesUtility.GetOrCreateSettings<Preferences_SettingsMenuCreator_Templates>
        (PreferencesProvider_SettingsMenuCreator_Templates.fileDirectory,
         PreferencesProvider_SettingsMenuCreator_Templates.fileName, out var preferences);
      var provider = preferences != null ? preferences.InputElementProviders_UIT : null;
      CreateInputElementProvidersAndResources_UIT(provider);
    }

    [MenuItem("CONTEXT/InputElementProviders_UGUI/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateInputElementProvidersAndResources_UGUI(MenuCommand command)
    {
      var original = (InputElementProviderCollection_UGUI)command.context;
      CreateInputElementProvidersAndResources_UGUI(original, false);
    }

    [MenuItem("CONTEXT/InputElementProviders_UIT/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateInputElementProvidersAndResources_UIT(MenuCommand command)
    {
      var original = (InputElementProviderCollection_UIT)command.context;
      CreateInputElementProvidersAndResources_UIT(original, false);
    }

    #region Providers UGUI Duplicate Commands
    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Button/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Button(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Button)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Dropdown/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Dropdown(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Dropdown)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_PreviousNextSelector/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_PreviousNextSelector(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_PreviousNextSelector)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Float/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Float(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Float)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Integer/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Integer(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Slider_Integer)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Toggle/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Toggle(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Toggle)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    [MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab/Duplicate (incl. Resources)", false, 1)]
    private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab(MenuCommand command)
    {
      var original = (ScriptableInputElementProvider_UGUI_FromPrefab)command.context;
      CreateInputElementProviderAndResources_UGUI(original);
    }

    // TODO Enable once put back in
    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Field_Float/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Field_Float(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Field_Float)command.context;
    //  CreateInputElementProviderAndResources_UGUI(original);
    //}

    // TODO Enable once put back in
    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UGUI_FromPrefab_Field_Integer/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UGUI_ScriptableInputElementProvider_UGUI_FromPrefab_Field_Integer(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UGUI_FromPrefab_Field_Integer)command.context;
    //  CreateInputElementProviderAndResources_UGUI(original);
    //}
    #endregion

    #region Providers UIT Duplicate Commands
    // Disabled for the time being because UI Toolkit Provider duplication has issues

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_Button/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_Button(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_Button)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_Label/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_Label(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_Label)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_Dropdown/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_Dropdown(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_Dropdown)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_PreviousNextSelector/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_PreviousNextSelector(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_PreviousNextSelector)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_Slider_Float/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_Slider_Float(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_Slider_Float)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}

    //[MenuItem("CONTEXT/ScriptableInputElementProvider_UIT_FromUXML_Toggle/Duplicate (incl. Resources)", false, 1)]
    //private static void Command_DuplicateProviderAndResources_UIT_ScriptableInputElementProvider_UIT_FromUXML_Toggle(MenuCommand command)
    //{
    //  var original = (ScriptableInputElementProvider_UIT_FromUXML_Toggle)command.context;
    //  CreateInputElementProviderAndResources_UIT(original);
    //}
    #endregion

    public static void CreateSettingsCollectionAndResources
      (SettingsCollection original = null, bool usePresetsIfNull = true, bool usePreferences = true,
      StyleProfile styleProfile = null, GameObject layoutPrefab_UGUI = null,
      SettingsMenu_UGUI menuTemplate_UGUI = null, InputElementProviderCollection_UGUI inputElementProviders_UGUI = null,
      SettingsMenu_UIT menuTemplate_UIT = null, List<VisualTreeAsset> menuDocuments = null,
      List<StyleSheet> menuStyleSheets = null, StyleProfile styleProfile_UIT = null,
      InputElementProviderCollection_UIT inputElementProviders_UIT = null, bool createAllFolders = false,
      bool createDedicatedScene = false, SettingsSaver settingsSaver = null)
    {
      EditorSceneManager.SaveOpenScenes();

      string folder_UGUI = string.Empty;
      string folder_UGUI_Prefabs = string.Empty;
      string folder_UGUI_Providers = string.Empty;
      string menuFolder_UGUI = string.Empty;
      string inputElementsFolder_UGUI = string.Empty;

      string folder_UIT = string.Empty;
      string inputElementsFolder_UIT = string.Empty;
      string folder_UIT_UXML = string.Empty;
      string folder_UIT_USS = string.Empty;
      string folder_UIT_Providers = string.Empty;
      string menuFolder_UIT = string.Empty;
      string menuFolder_UIT_UXML = string.Empty;
      string menuFolder_UIT_USS = string.Empty;

      #region Fetch Preferences
      PreferencesUtility.GetOrCreateSettings<Preferences_SettingsMenuCreator_Templates>
        (PreferencesProvider_SettingsMenuCreator_Templates.fileDirectory,
        PreferencesProvider_SettingsMenuCreator_Templates.fileName, out var preferences);
      #endregion

      #region Sources (UGUI)
      #region Input Element Providers
      var providersSource_UGUI = inputElementProviders_UGUI;
      if (providersSource_UGUI == null && original != null)
      {
        providersSource_UGUI = original.InputElementProviders_UGUI;
      }
      if (providersSource_UGUI == null && usePreferences && preferences != null)
      {
        providersSource_UGUI = preferences.InputElementProviders_UGUI;
      }

      bool createInputElementProviders_UGUI = providersSource_UGUI != null;
      #endregion

      #region Style Profile
      if (styleProfile == null && usePreferences && preferences != null)
      {
        styleProfile = preferences.StyleProfile;
      }

      bool createStyleProfile = styleProfile != null;
      #endregion

      #region Menu Layout
      if (layoutPrefab_UGUI == null && usePreferences && preferences != null)
      {
        layoutPrefab_UGUI = preferences.MenuLayoutTemplate_UGUI;
      }

      bool createMenuLayoutPrefab_UGUI = layoutPrefab_UGUI != null;
      #endregion

      #region Menu Template
      if (menuTemplate_UGUI == null && usePreferences && preferences != null)
      {
        menuTemplate_UGUI = preferences.MenuTemplate_UGUI;
      }

      bool createMenuTemplate_UGUI = menuTemplate_UGUI != null;
      #endregion
      #endregion

      #region Sources (UI Toolkit)
      #region Input Element Providers
      var providersSource_UIT = inputElementProviders_UIT;
      if (providersSource_UIT == null && original != null)
      {
        providersSource_UIT = original.InputElementProviders_UIT;
      }
      if (providersSource_UIT == null && usePreferences && preferences != null)
      {
        providersSource_UIT = preferences.InputElementProviders_UIT;
      }

      bool createInputElementProviders_UIT = providersSource_UIT != null;
      #endregion

      #region Style Profile
      if (styleProfile_UIT == null && usePreferences && preferences != null)
      {
        styleProfile_UIT = preferences.StyleProfile_UIT;
      }

      bool createStyleProfile_UIT = styleProfile_UIT != null;
      #endregion

      #region Menu Template
      if (menuTemplate_UIT == null && usePreferences && preferences != null)
      {
        menuTemplate_UIT = preferences.MenuTemplate_UIT;
      }

      bool createMenuTemplate_UIT = menuTemplate_UIT != null;
      #endregion

      #region Menu Documents
      if ((menuDocuments == null || menuDocuments.Count == 0) && usePreferences && preferences != null)
      {
        menuDocuments = preferences.MenuDocuments_UIT;
      }

      bool createMenuDocuments = menuDocuments?.Count > 0;
      #endregion

      #region Menu Style Sheets
      if ((menuStyleSheets == null || menuStyleSheets.Count == 0) && usePreferences && preferences != null)
      {
        menuStyleSheets = preferences.MenuStyleSheets_UIT;
      }

      bool createMenuStyleSheets = menuStyleSheets?.Count > 0;
      #endregion
      #endregion

      #region Create Folders
      // Get the path to the currently active or selected folder
      string rootFolder = EditorUtilities.GetActiveFolderPath();

      // Create the root folder for the resources to create
      var rootFolderGuid = AssetDatabase.CreateFolder(rootFolder, "Settings");
      rootFolder = AssetDatabase.GUIDToAssetPath(rootFolderGuid);

      #region UGUI
      bool createInputElementsFolder_UGUI = createAllFolders || createInputElementProviders_UGUI;
      bool createMenuFolder_UGUI = createAllFolders || createMenuLayoutPrefab_UGUI ||
                                   createStyleProfile || createMenuTemplate_UGUI;
      bool createFolder_UGUI = createAllFolders || createInputElementsFolder_UGUI || createMenuFolder_UGUI;

      if (createFolder_UGUI)
      {
        // Create the root folder for the UGUI resources
        var folderGuid_UGUI = AssetDatabase.CreateFolder(rootFolder, "UGUI");
        folder_UGUI = AssetDatabase.GUIDToAssetPath(folderGuid_UGUI);
      }

      if (createInputElementsFolder_UGUI)
      {
        // Create the folder for the UGUI input element resources
        var inputElementsFolderGuid_UGUI = AssetDatabase.CreateFolder(folder_UGUI, "Elements");
        inputElementsFolder_UGUI = AssetDatabase.GUIDToAssetPath(inputElementsFolderGuid_UGUI);

        // Create the folder for the UGUI input element provider resources
        var folderGuid_UGUI_Providers = AssetDatabase.CreateFolder(inputElementsFolder_UGUI, "Providers");
        folder_UGUI_Providers = AssetDatabase.GUIDToAssetPath(folderGuid_UGUI_Providers);

        // Create the folder for the UGUI input element prefab resources
        var folderGuid_UGUI_Prefabs = AssetDatabase.CreateFolder(inputElementsFolder_UGUI, "Prefabs");
        folder_UGUI_Prefabs = AssetDatabase.GUIDToAssetPath(folderGuid_UGUI_Prefabs);
      }

      if (createMenuFolder_UGUI)
      {
        // Create the folder for the UGUI menu resources
        var menuFolderGuid_UGUI = AssetDatabase.CreateFolder(folder_UGUI, "Menu");
        menuFolder_UGUI = AssetDatabase.GUIDToAssetPath(menuFolderGuid_UGUI);
      }
      #endregion

      #region UI Toolkit
      bool createInputElementsFolder_UIT = createAllFolders || createInputElementProviders_UIT;
      bool createMenuFolder_UIT = createAllFolders || createStyleProfile_UIT || createMenuTemplate_UIT ||
                                  createMenuDocuments || createMenuStyleSheets;
      bool createFolder_UIT = createAllFolders || createInputElementsFolder_UIT || createMenuFolder_UIT;

      if (createFolder_UIT)
      {
        // Create the folder for the UIT resources
        var folderGuid_UIT = AssetDatabase.CreateFolder(rootFolder, "UI Toolkit");
        folder_UIT = AssetDatabase.GUIDToAssetPath(folderGuid_UIT);
      }

      if (createInputElementsFolder_UIT)
      {
        // Create the folder for the UIT input element resources
        var inputElementsFolderGuid_UIT = AssetDatabase.CreateFolder(folder_UIT, "Elements");
        inputElementsFolder_UIT = AssetDatabase.GUIDToAssetPath(inputElementsFolderGuid_UIT);

        // Create the folder for the UIT input element provider resources
        var folderGuid_UIT_Providers = AssetDatabase.CreateFolder(inputElementsFolder_UIT, "Providers");
        folder_UIT_Providers = AssetDatabase.GUIDToAssetPath(folderGuid_UIT_Providers);

        // Create the folder for the UIT UXML resources
        var folderGuid_UIT_UXML = AssetDatabase.CreateFolder(inputElementsFolder_UIT, "UXML");
        folder_UIT_UXML = AssetDatabase.GUIDToAssetPath(folderGuid_UIT_UXML);

        // Create the folder for the UIT USS resources
        var folderGuid_UIT_USS = AssetDatabase.CreateFolder(inputElementsFolder_UIT, "USS");
        folder_UIT_USS = AssetDatabase.GUIDToAssetPath(folderGuid_UIT_USS);
      }

      if (createMenuFolder_UIT)
      {
        // Create the folder for the UIT menu resources
        var menuFolderGuid_UIT = AssetDatabase.CreateFolder(folder_UIT, "Menu");
        menuFolder_UIT = AssetDatabase.GUIDToAssetPath(menuFolderGuid_UIT);

        // Create the folder for the UIT menu UXML resources
        if (createMenuDocuments)
        {
          var menuFolderGuid_UIT_UXML = AssetDatabase.CreateFolder(menuFolder_UIT, "UXML");
          menuFolder_UIT_UXML = AssetDatabase.GUIDToAssetPath(menuFolderGuid_UIT_UXML);
        }

        // Create the folder for the UIT menu USS resources
        if (createMenuStyleSheets)
        {
          var menuFolderGuid_UIT_USS = AssetDatabase.CreateFolder(menuFolder_UIT, "USS");
          menuFolder_UIT_USS = AssetDatabase.GUIDToAssetPath(menuFolderGuid_UIT_USS);
        }
      }
      #endregion
      #endregion

      // Create the settings collection and apply all specified project presets
      //var collection = CreateAssetWithPresets<SettingsCollection>(rootFolder, "SettingsCollection");
      var collection = CreateSettingsCollection(rootFolder, original, usePresetsIfNull);

      // Create the input element providers object for UGUI
      //var providers_UGUI = CreateAssetWithPresets<InputElementProviders_UGUI>(path, "InputElementProviders_UGUI");
      #region Create Input Element Providers (UGUI)
      if (createInputElementProviders_UGUI)
      {
        var providers_UGUI = CreateProviders_UGUI(inputElementsFolder_UGUI, folder_UGUI, providersSource_UGUI, usePresetsIfNull);
        collection.InputElementProviders_UGUI = providers_UGUI;
      }
      #endregion

      #region Create Input Element Providers (UI Toolkit)
      if (createInputElementProviders_UIT)
      {
        var providers_UIT = CreateProviders_UIT(inputElementsFolder_UIT, folder_UIT, providersSource_UIT, usePresetsIfNull);
        collection.InputElementProviders_UIT = providers_UIT;
      }
      #endregion

      #region Settings Saver
      settingsSaver = CreateSettingsSaver(rootFolder,
        settingsSaver != null ? settingsSaver :
        original != null ? original.SettingsSaver : null);
      collection.SettingsSaver = settingsSaver;
      #endregion

      #region Move Assets
      // Move UGUI assets
      if (createInputElementProviders_UGUI)
      {
        MoveAllAssetsOfType<GameObject>("t:prefab", folder_UGUI, folder_UGUI_Prefabs);
        MoveAllAssetsOfType<ScriptableInputElementProvider_UGUI>("t:ScriptableInputElementProvider_UGUI",
                                                                 folder_UGUI, folder_UGUI_Providers);
      }

      // Move UIT assets
      if (createInputElementProviders_UIT)
      {
        MoveAllAssetsOfType<VisualTreeAsset>("t:VisualTreeAsset", folder_UIT, folder_UIT_UXML);
        MoveAllAssetsOfType<StyleSheet>("t:stylesheet", folder_UIT, folder_UIT_USS);
        MoveAllAssetsOfType<ScriptableInputElementProvider_UIT>("t:ScriptableInputElementProvider_UIT",
                                                                folder_UIT, folder_UIT_Providers);
      }
      #endregion

      // Scene
      var previousActiveScene = EditorSceneManager.GetActiveScene();
      Scene scene = default;

      if (createDedicatedScene)
      {
        scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(scene);

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem),
          typeof(StandaloneInputModule), typeof(InputModuleValidator));
        eventSystemObject.transform.SetAsFirstSibling();

        GameObject lightObject = new GameObject("Directional Light", typeof(Light));
        lightObject.GetComponent<Light>().type = LightType.Directional;
        lightObject.transform.rotation = Quaternion.Euler(45, 0, 0);
        lightObject.transform.SetAsFirstSibling();

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.transform.SetAsFirstSibling();
        cameraObject.tag = "MainCamera";
      }

      #region Create Menu Resources

      #region UGUI
      GameObject menuLayoutPrefabVariant = null;

      if (createStyleProfile &&
          CreateScriptableObjectAsset<StyleProfile>(menuFolder_UGUI, styleProfile != null ?
            styleProfile.name : nameof(StyleProfile),
            styleProfile, out var duplicate, usePresetsIfNull))
      {

        styleProfile = duplicate as StyleProfile;
        collection.StyleProfile = styleProfile;
      }

      if (createMenuLayoutPrefab_UGUI && layoutPrefab_UGUI != null)
      {
        menuLayoutPrefabVariant = CreatePrefabVariant(layoutPrefab_UGUI, menuFolder_UGUI);
        collection.UguiLayoutPrefab = menuLayoutPrefabVariant;
        SettingsCollectionEditorNew.CreatePrefab(null, collection, createNewPrefabVariant: false);
      }

      if (createMenuTemplate_UGUI && menuTemplate_UGUI != null)
      {
        var menuPrefab = CreatePrefabVariant(menuTemplate_UGUI.gameObject, menuFolder_UGUI);
        SettingsMenu_UGUI settingsMenuComponent = null;

        if (menuPrefab != null)
        {
          settingsMenuComponent = menuPrefab.GetComponent<SettingsMenu_UGUI>();
        }

        if (settingsMenuComponent != null)
        {
          settingsMenuComponent.SettingsTemplate = collection;

          if (menuLayoutPrefabVariant != null)
          {
            settingsMenuComponent.MenuTemplate = menuLayoutPrefabVariant;
          }

          if (styleProfile != null &&
              menuPrefab.TryGetComponent<AssignStyleProfileToListenersInHierarchy>(out var styleProfileAssigner))
          {
            styleProfileAssigner.StyleProfile = styleProfile;
          }
        }

        var savedMenuPrefab = PrefabUtility.SavePrefabAsset(menuPrefab);
        EditorUtility.SetDirty(menuPrefab);

        if (createDedicatedScene)
        {
          if (savedMenuPrefab != null)
          {
            PrefabUtility.InstantiatePrefab(savedMenuPrefab);
          }
        }
      }

      if (menuLayoutPrefabVariant != null)
      {

        //if (CreateAsset<MenuWithInputElementsPrefabCreator>(menuFolder_UGUI, nameof(MenuWithInputElementsPrefabCreator),
        //    null, out var newAsset, false))
        //{
        //  var newMenuWithInputElementsPrefabCreator = newAsset as MenuWithInputElementsPrefabCreator;
        //  newMenuWithInputElementsPrefabCreator.MenuLayoutPrefab = menuLayoutPrefabVariant;
        //  newMenuWithInputElementsPrefabCreator.SettingsCollection = collection;
        //  newMenuWithInputElementsPrefabCreator.StyleProfile = styleProfile;
        //  EditorUtility.SetDirty(newMenuWithInputElementsPrefabCreator);

        //  var layoutPrefab = newMenuWithInputElementsPrefabCreator.CreatePrefab();
        //  //if (layoutPrefab != null)
        //  //{
        //  //  var layoutInstance = PrefabUtility.InstantiatePrefab(layoutPrefab) as GameObject;

        //  //  if (layoutInstance != null)
        //  //  {
        //  //    layoutInstance.AddComponent<RegisterStyleListenersInHierarchyInEditMode>();
        //  //    var applyStyleProfileScript = layoutInstance.AddComponent<ApplyStyleProfile>();
        //  //    applyStyleProfileScript.StyleProfile = styleProfile;
        //  //    PrefabUtility.ApplyPrefabInstance(layoutInstance, InteractionMode.AutomatedAction);
        //  //    GameObject.DestroyImmediate(layoutInstance);
        //  //  }
        //  //}
        //}
      }
      #endregion

      #region UI Toolkit

      if (createStyleProfile_UIT &&
          CreateScriptableObjectAsset<StyleProfile>(menuFolder_UIT, styleProfile_UIT != null ?
          styleProfile_UIT.name : nameof(StyleProfile),
          styleProfile_UIT, out var duplicate_StyleProfile_UIT, usePresetsIfNull))
      {

        styleProfile_UIT = duplicate_StyleProfile_UIT as StyleProfile;
      }

      List<VisualTreeAsset> menuTemplatesDuplicate = null;
      if (createMenuDocuments && menuDocuments?.Count > 0)
      {
        //var menuTemplatesDuplicate = DuplicateList(settingsMenuComponent.MenuTemplates, rootFolder);
        menuTemplatesDuplicate = DuplicateList(menuDocuments, menuFolder_UIT_UXML);
      }

      List<StyleSheet> styleSheetsDuplicate = null;
      if (createMenuStyleSheets && menuStyleSheets?.Count > 0 /*&& settingsMenuComponent.StyleSheets != null*/)
      {
        //var styleSheetsDuplicate = DuplicateList(settingsMenuComponent.StyleSheets, rootFolder);
        styleSheetsDuplicate = DuplicateList(menuStyleSheets, menuFolder_UIT_USS);
      }

      if (createMenuTemplate_UIT && menuTemplate_UIT != null)
      {
        var menuPrefab = CreatePrefabVariant(menuTemplate_UIT.gameObject, menuFolder_UIT);
        SettingsMenu_UIT settingsMenuComponent = null;

        if (menuPrefab != null)
        {
          settingsMenuComponent = menuPrefab.GetComponent<SettingsMenu_UIT>();
        }

        if (settingsMenuComponent != null)
        {
          settingsMenuComponent.SettingsTemplate = collection;
          //settingsMenuComponent.MenuTemplates.Clear();
          //settingsMenuComponent.StyleSheets.Clear();

          if (menuTemplatesDuplicate?.Count > 0)
          {
            settingsMenuComponent.MenuTemplates = menuTemplatesDuplicate;
          }

          if (styleSheetsDuplicate?.Count > 0)
          {
            settingsMenuComponent.StyleSheets = styleSheetsDuplicate;
          }

          //if (styleProfile_UIT != null &&
          //  menuPrefab.TryGetComponent<StyleProfileHierarchyUpdater_UIT>(out var styleProfileHierarchyUpdater))
          //{
          //  styleProfileHierarchyUpdater.StyleProfile = styleProfile_UIT;
          //}

          if (styleProfile_UIT != null && menuPrefab != null &&
              menuPrefab.TryGetComponent<AssignStyleProfileToListenersInHierarchy>(out var styleProfileAssigner))
          {
            styleProfileAssigner.StyleProfile = styleProfile_UIT;
          }
        }

        var savedMenuPrefab = PrefabUtility.SavePrefabAsset(menuPrefab);
        EditorUtility.SetDirty(menuPrefab);

        if (createDedicatedScene)
        {
          if (savedMenuPrefab != null)
          {
            PrefabUtility.InstantiatePrefab(savedMenuPrefab);
          }
        }
      }
      #endregion

      #endregion

      if (original != null)
      {
        if (original.AudioMixer != null)
        {
          collection.AudioMixer = original.AudioMixer;
        }

#if UNITY_POST_PROCESSING
        if (original.PostProcessProfile != null)
        {
          collection.PostProcessProfile = original.PostProcessProfile;

          if (createDedicatedScene)
          {
            GameObject volumeObject = new GameObject("Post Process Volume", typeof(PostProcessVolume));
            var volume = volumeObject.GetComponent<PostProcessVolume>();
            volume.sharedProfile = original.PostProcessProfile;
            volume.isGlobal = true;
            if (scene != null)
            {
              var rootGameObjects = scene.GetRootGameObjects();
              for (int i = 0; i < rootGameObjects.Length; i++)
              {
                var rootObject = rootGameObjects[i];
                var camera = rootObject.GetComponentInChildren<Camera>();
                if (camera != null)
                {
                  var layer = camera.gameObject.AddOrGetComponent<PostProcessLayer>();
                  layer.volumeLayer = LayerMask.GetMask("Default");
                }
              }
            }
          }
        }
#endif

#if UNITY_HDRP || UNITY_URP
        if (original.VolumeProfile != null)
        {
          collection.VolumeProfile = original.VolumeProfile;

          if (createDedicatedScene)
          {
            GameObject volumeObject = new GameObject("Global Volume", typeof(UnityEngine.Rendering.Volume));
            volumeObject.GetComponent<UnityEngine.Rendering.Volume>().sharedProfile = original.VolumeProfile;
          }
        }
#endif
      }

      EditorSceneManager.SetActiveScene(previousActiveScene);

      if (createDedicatedScene)
      {
        var sceneSavePath = rootFolder + "/SettingsMenu.unity";
        sceneSavePath = AssetDatabase.GenerateUniqueAssetPath(sceneSavePath);
        EditorSceneManager.SaveScene(scene, sceneSavePath);
        //EditorSceneManager.CloseScene(scene, false);
      }

      EditorUtility.SetDirty(collection);
      AssetDatabase.Refresh();
      EditorGUIUtility.PingObject(collection);
      Selection.activeObject = collection;
      ScheduleUtility.InvokeDelayedByFrames(() => Selection.activeObject = collection, 2);
      Cleanup();
    }

    public static void CreateInputElementProvidersAndResources_UGUI(InputElementProviderCollection_UGUI original = null, bool usePresetIfNull = true)
    {
      // Get the path to the currently active or selected folder
      string rootFolder = EditorUtilities.GetActiveFolderPath();

      // Create the root folder for the resources to create
      var rootFolderGuid = AssetDatabase.CreateFolder(rootFolder, "UGUI Providers");
      rootFolder = AssetDatabase.GUIDToAssetPath(rootFolderGuid);

      // Create the folder for the UGUI provider resources
      var folderGuid_UGUI_Providers = AssetDatabase.CreateFolder(rootFolder, "Providers");
      var folder_UGUI_Providers = AssetDatabase.GUIDToAssetPath(folderGuid_UGUI_Providers);

      // Create the folder for the UGUI prefab resources
      var folderGuid_UGUI_Prefabs = AssetDatabase.CreateFolder(rootFolder, "Prefabs");
      var folder_UGUI_Prefabs = AssetDatabase.GUIDToAssetPath(folderGuid_UGUI_Prefabs);

      // Create the input element providers object for UGUI
      var providers_UGUI = CreateProviders_UGUI(rootFolder, rootFolder, original, usePresetIfNull);

      // Move UGUI assets
      MoveAllAssetsOfType<GameObject>("t:prefab", rootFolder, folder_UGUI_Prefabs);
      MoveAllAssetsOfType<ScriptableInputElementProvider_UGUI>("t:ScriptableInputElementProvider_UGUI",
                                                               rootFolder, folder_UGUI_Providers);

      EditorUtility.SetDirty(providers_UGUI);
      AssetDatabase.Refresh();

      EditorGUIUtility.PingObject(providers_UGUI);

      Cleanup();
    }

    public static void CreateInputElementProvidersAndResources_UIT(InputElementProviderCollection_UIT original = null, bool usePresetIfNull = true)
    {
      // Get the path to the currently active or selected folder
      string rootFolder = EditorUtilities.GetActiveFolderPath();

      // Create the root folder for the resources to create
      var rootFolderGuid = AssetDatabase.CreateFolder(rootFolder, "UI Toolkit Providers");
      rootFolder = AssetDatabase.GUIDToAssetPath(rootFolderGuid);

      // Create the folder for the provider resources
      var folderGuid_Providers = AssetDatabase.CreateFolder(rootFolder, "Providers");
      var folder_Providers = AssetDatabase.GUIDToAssetPath(folderGuid_Providers);

      // Create the folder for the UXML resources
      var folderGuid_UXML = AssetDatabase.CreateFolder(rootFolder, "UXML");
      var folder_UXML = AssetDatabase.GUIDToAssetPath(folderGuid_UXML);

      // Create the folder for the USS resources
      var folderGuid_USS = AssetDatabase.CreateFolder(rootFolder, "USS");
      var folder_USS = AssetDatabase.GUIDToAssetPath(folderGuid_USS);

      // Create the input element providers object for UI Toolkit
      var providers_UIT = CreateProviders_UIT(rootFolder, rootFolder, original, usePresetIfNull);

      // Move UIT assets
      MoveAllAssetsOfType<VisualTreeAsset>("t:VisualTreeAsset", rootFolder, folder_UXML);
      MoveAllAssetsOfType<StyleSheet>("t:stylesheet", rootFolder, folder_USS);
      MoveAllAssetsOfType<ScriptableInputElementProvider_UIT>("t:ScriptableInputElementProvider_UIT",
                                                              rootFolder, folder_Providers);

      EditorUtility.SetDirty(providers_UIT);
      AssetDatabase.Refresh();

      EditorGUIUtility.PingObject(providers_UIT);

      Cleanup();
    }

    public static void CreateInputElementProviderAndResources_UGUI<T>(T original) where T : ScriptableInputElementProvider_UGUI
    {
      // Get the path to the currently active or selected folder
      string rootFolder = EditorUtilities.GetActiveFolderPath();

      // Create the root folder for the resources to create
      var rootFolderGuid = AssetDatabase.CreateFolder(rootFolder, typeof(T).Name);
      rootFolder = AssetDatabase.GUIDToAssetPath(rootFolderGuid);

      // Create the folder for prefab resources
      var folderGuid_Prefabs = AssetDatabase.CreateFolder(rootFolder, "Prefab");
      var folder_Prefabs = AssetDatabase.GUIDToAssetPath(folderGuid_Prefabs);

      var assetName = original != null ? original.name : typeof(T).Name;
      // Create the input element providers object for UGUI
      CreateScriptableObjectAsset<T>(rootFolder, assetName, original, out var provider);

      // Move prefabs
      MoveAllAssetsOfType<GameObject>("t:prefab", rootFolder, folder_Prefabs);

      EditorUtility.SetDirty(provider);
      AssetDatabase.Refresh();

      EditorGUIUtility.PingObject(provider);

      Cleanup();
    }

    public static void CreateInputElementProviderAndResources_UIT<T>(T original) where T : ScriptableInputElementProvider_UIT
    {
      // Get the path to the currently active or selected folder
      string rootFolder = EditorUtilities.GetActiveFolderPath();

      // Create the root folder for the resources to create
      var rootFolderGuid = AssetDatabase.CreateFolder(rootFolder, typeof(T).Name);
      rootFolder = AssetDatabase.GUIDToAssetPath(rootFolderGuid);

      // Create the folder for the UXML resources
      var folderGuid_UXML = AssetDatabase.CreateFolder(rootFolder, "UXML");
      var folder_UXML = AssetDatabase.GUIDToAssetPath(folderGuid_UXML);

      // Create the folder for the USS resources
      var folderGuid_USS = AssetDatabase.CreateFolder(rootFolder, "USS");
      var folder_USS = AssetDatabase.GUIDToAssetPath(folderGuid_USS);

      var assetName = original != null ? original.name : typeof(T).Name;
      // Create the input element providers object for UIT
      CreateScriptableObjectAsset<T>(rootFolder, assetName, original, out var provider);

      // Move assets
      MoveAllAssetsOfType<VisualTreeAsset>("t:VisualTreeAsset", rootFolder, folder_UXML);
      MoveAllAssetsOfType<StyleSheet>("t:stylesheet", rootFolder, folder_USS);

      EditorUtility.SetDirty(provider);
      AssetDatabase.Refresh();

      EditorGUIUtility.PingObject(provider);

      Cleanup();
    }

    private static GameObject CreatePrefabVariant(GameObject source, string folderPath)
    {
      if (source == null || string.IsNullOrEmpty(folderPath)) { return null; }

      // Check if the object was already duplicated in this duplication process
      if (source != null && duplicatedAssets != null && duplicatedAssets.TryGetValue(source, out var duplicatedAsset))
      {
        if (duplicatedAsset.GetType() == typeof(GameObject))
        {
          return duplicatedAsset as GameObject;
        }
      }

      GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
      if (instance == null) { return null; }

      var prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, $"{folderPath}/{source.name}.prefab");
      if (prefabVariant == null) { return null; }
      GameObject.DestroyImmediate(instance);
      EditorUtility.SetDirty(prefabVariant);
      duplicatedAssets?.AddOrUpdateDictionaryItem(source, prefabVariant);
      return prefabVariant;
    }

    private static SettingsSaver CreateSettingsSaver(string folder, SettingsSaver original)
    {
      if (string.IsNullOrEmpty(folder)) { return null; }

      SettingsSaver settingsSaver = null;
      if (original != null)
      {
        DuplicateAsset(original, folder, out settingsSaver);
      }
      if (settingsSaver == null)
      {
        if (CreateScriptableObjectAsset/*WithPresets*/<SettingsSaver_PlayerPrefs>(folder, "SettingsSaver_PlayerPrefs", null, out var duplicate))
        {
          settingsSaver = duplicate as SettingsSaver_PlayerPrefs;
        }
      }
      if (settingsSaver != null)
      {
        EditorUtility.SetDirty(settingsSaver);
      }
      return settingsSaver;
    }

    // TODO Add additional null checks for methods below

    public static void MoveAllAssetsOfType<T>(string searchString, string oldFolderPath, string newFolderPath)
      where T : UnityEngine.Object
    {
      var assetGuids = AssetDatabase.FindAssets(searchString, new string[] { oldFolderPath });
      foreach (var i in assetGuids)
      {
        var prefabPath = AssetDatabase.GUIDToAssetPath(i);
        var asset = AssetDatabase.LoadAssetAtPath<T>(prefabPath);
        var isMainAsset = AssetDatabase.IsMainAsset(asset);
        if (asset != null && isMainAsset)
        {
          var extension = Path.GetExtension(prefabPath);
          string newPath = $"{newFolderPath}/{asset.name}{extension}";
          AssetDatabase.MoveAsset(prefabPath, newPath);
        }
      }
    }

    public static SettingsCollection CreateSettingsCollection(string path, SettingsCollection original = null,
                                                              bool usePresetIfNull = true)
    {
      SettingsCollection collection = null;
      if (!(original != null && DuplicateAsset(original, path, out collection)))
      {
        if (CreateScriptableObjectAsset/*WithPresets*/<SettingsCollection>(path, "SettingsCollection", null, out var duplicate, usePresetIfNull))
        {
          collection = duplicate as SettingsCollection;
        }

        //else
        //{
        //  collection = ScriptableObject.CreateInstance<SettingsCollection>();
        //}
      }

      if (collection != null)
      {
        EditorUtility.SetDirty(collection);
      }
      return collection;
    }

    public static InputElementProviderCollection_UGUI CreateProviders_UGUI(string path, string resourcesFolder,
                                                                  InputElementProviderCollection_UGUI original = null,
                                                                  bool usePresetIfNull = true)
    {
      // Create the object to hold all provider references
      InputElementProviderCollection_UGUI providers_UGUI = null;
      if (!(original != null && DuplicateAsset(original, path, out providers_UGUI)))
      {
        // TODO Instead of using the preset use the one in the preferences?
        if (CreateScriptableObjectAsset/*WithPresets*/<InputElementProviderCollection_UGUI>(path, "InputElementProviders_UGUI", null, out var duplicate, usePresetIfNull))
        {
          providers_UGUI = duplicate as InputElementProviderCollection_UGUI;
        }
      }

      // Maps the original provider to the duplicate
      // Prevents the duplication of the name original provider twice
      // which would lead to unwanted duplication
      Dictionary<ScriptableInputElementProvider_UGUI, ScriptableInputElementProvider_UGUI> providerMapping =
        new Dictionary<ScriptableInputElementProvider_UGUI, ScriptableInputElementProvider_UGUI>();

      var newProviderRelations = new List<StringToGenericDataRelation<ScriptableInputElementProvider_UGUI>>();
      var relations = original != null ? original.GetRelations() : providers_UGUI.GetRelations();
      // Relation between the original and the duplicate
      // This dictionary is used to replace any Objects that were duplicated more than one time
      // with the first duplicate
      //Dictionary<Object, Object> duplicatedObjects = new Dictionary<Object, Object>();
      List<Object> objectsToRemove = new List<Object>();

      foreach (var r in relations)
      {
        var key = r.Item1;
        var provider = r.Item2;
        bool alreadyDuplicated = false;
        ScriptableInputElementProvider_UGUI newProvider = null;

        if (provider != null)
        {
          // Check if the provider was already duplicated
          if (providerMapping.ContainsKey(provider))
          {
            newProvider = providerMapping[provider];
            alreadyDuplicated = true;
          }
          else
          {
            // Create a new provider if the provider was not duplicated before
            if (CreateScriptableObjectAsset<ScriptableInputElementProvider_UGUI>(resourcesFolder, provider.name, provider, out var duplicate))
            {
              newProvider = duplicate as ScriptableInputElementProvider_UGUI;
            }
            providerMapping.Add(provider, newProvider);
          }
        }

        newProviderRelations.Add(new StringToGenericDataRelation<ScriptableInputElementProvider_UGUI>(key, newProvider));

        // Skip the cleanup of multiple duplicates of the same UnityEngine.Object
        if (alreadyDuplicated) { continue; }

        #region Cleanup multiple duplicates of the same UnityEngine.Object
        if (provider != null)
        {
          var fields = provider.GetType().GetSerializableFields();

          if (fields?.Count() > 0)
          {
            foreach (var field in fields)
            {
              var fieldType = field.FieldType;
              if (typeof(Object).IsAssignableFrom(fieldType))
              {
                // TODO Test
                //ConsoleLogger.Log($"{field.GetType()} for {field.Name}");

                var originalObj = field.GetValue(provider);
                if (originalObj == null) { continue; }
                var originalObject = originalObj as Object;

                var currentObj = field.GetValue(newProvider);
                if (currentObj == null) { continue; }
                var currentObject = currentObj as Object;

                if (duplicatedAssets != null && duplicatedAssets.TryGetValue(originalObject, out var duplicatedObject))
                {
                  // Handle the case that this object already has a dictionary entry meaning
                  // it has already been duplicated before and requires the current value
                  // to be replaced by the one in the dictionary

                  // Mark the object that was duplicated to be removed
                  objectsToRemove.AddIfNotContains(currentObject);

                  // Assign the object reference from the dictionary
                  field.SetValue(newProvider, duplicatedObject);
                }
                else
                {
                  // Add an entry for the object so future object references
                  // will be using this object instead of their own duplication
                  duplicatedAssets?.Add(originalObject, currentObject);
                }
              }
              else if (fieldType.IsListType() &&
                       typeof(Object).IsAssignableFrom(fieldType.GetGenericArguments()[0]))
              {
                // Original list
                var originalListValue = field.GetValue(provider);
                GetListAsArrayFromObject(originalListValue, out var elementType, out var originalList);

                // Duplicated list
                var newField = newProvider.GetType().GetSerializableField(field.Name);
                var duplicateListValue = newField.GetValue(newProvider);
                GetListAsArrayFromObject(duplicateListValue, out var newElementType, out var duplicatedList);

                if (elementType == null || newElementType == null)
                {
                  ConsoleLogger.LogWarning("List element type is null");
                  continue;
                }

                if (elementType != newElementType)
                {
                  ConsoleLogger.LogWarning($"List element types don't match!");
                  continue;
                }

                for (int i = 0; i < duplicatedList.Length; i++)
                {
                  var originalObj = originalList[i];
                  if (originalObj == null) { continue; }
                  var originalObject = originalObj as Object;
                  var currentObj = duplicatedList[i];
                  if (currentObj == null) { continue; }
                  var currentObject = currentObj as Object;

                  if (duplicatedAssets != null && duplicatedAssets.TryGetValue(originalObject, out var duplicatedObject))
                  {
                    // Handle the case that this object already has a dictionary entry meaning
                    // it has already been duplicated before and requires the current value
                    // to be replaced by the one in the dictionary

                    // Mark the object that was duplicated to be removed
                    objectsToRemove.AddIfNotContains(currentObject);

                    // Assign the object reference from the dictionary
                    duplicatedList[i] = duplicatedObject;
                  }
                  else
                  {
                    // Add an entry for the object so future object references
                    // will be using this object instead of their own duplication
                    duplicatedAssets?.Add(originalObject, currentObject);
                  }
                }

                // Cast back to the correct list
                var listConstructor = typeof(List<>).MakeGenericType(elementType).
                  GetConstructor(new System.Type[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
                var modifiedList = listConstructor.Invoke(new object[] { duplicatedList });

                // Assign the list back to the field
                newField.SetValue(newProvider, modifiedList);
              }
            }
          }
        }
        #endregion
      }

      providers_UGUI.SetRelations(newProviderRelations);
      //var newAssetPath = AssetDatabase.GetAssetPath(providers_UGUI);
      EditorUtility.SetDirty(providers_UGUI);
      //AssetDatabase.SaveAssetIfDirty(providers_UGUI);
      //AssetDatabase.ImportAsset(newAssetPath);
      //AssetDatabase.SaveAssetIfDirty(providers_UGUI);
      //AssetDatabase.LoadAssetAtPath(newAssetPath, providers_UGUI.GetType());
      //AssetDatabase.SaveAssetIfDirty(providers_UGUI);

      // TODO Does this need to be enabled again?
      //foreach (var obj in objectsToRemove)
      //{
      //  if (obj == null) { continue; }
      //  var objPath = AssetDatabase.GetAssetPath(obj);
      //  ConsoleLogger.Log($"Deleting object at path: {objPath}");
      //  AssetDatabase.DeleteAsset(objPath);
      //}

      return providers_UGUI;
    }

    private static void GetListAsArrayFromObject(object listObject,
      out System.Type elementType, out object[] elementsArray)
    {
      var listObjectType = listObject?.GetType();
      // Get the type of the list
      elementType = listObjectType?.GetGenericArguments()[0];
      // Get the "ToArray" method
      var toArrayMethod = listObjectType?.GetMethod("ToArray");
      // Cast the object to an array
      elementsArray = (object[])toArrayMethod?.Invoke(listObject, null);
    }

    public static InputElementProviderCollection_UIT CreateProviders_UIT(string path, string resourcesFolder,
                                                                InputElementProviderCollection_UIT original = null,
                                                                bool usePresetIfNull = true)
    {
      // Create the object to hold all provider references
      InputElementProviderCollection_UIT providers_UIT = null;
      if (!(original != null && DuplicateAsset(original, path, out providers_UIT)))
      {
        // TODO Instead of using the preset use the one in the preferences?
        if (CreateScriptableObjectAsset/*WithPresets*/<InputElementProviderCollection_UIT>(path, "InputElementProviders_UIT", null, out var duplicate, usePresetIfNull))
        {
          providers_UIT = duplicate as InputElementProviderCollection_UIT;
        }
      }

      // Maps the original provider to the duplicate
      // Prevents the duplication of the name original provider twice
      // which would lead to unwanted duplication
      Dictionary<ScriptableInputElementProvider_UIT, ScriptableInputElementProvider_UIT> providerMapping =
        new Dictionary<ScriptableInputElementProvider_UIT, ScriptableInputElementProvider_UIT>();

      var newProviderRelations = new List<StringToGenericDataRelation<ScriptableInputElementProvider_UIT>>();
      var relations = original != null ? original.GetRelations() : providers_UIT.GetRelations();

      // Relation between the original and the duplicate
      // This dictionary is used to replace any Objects that were duplicated more than one time
      // with the first duplicate
      //Dictionary<Object, Object> duplicatedObjects = new Dictionary<Object, Object>();
      List<Object> objectsToRemove = new List<Object>();

      foreach (var r in relations)
      {
        var key = r.Item1;
        var provider = r.Item2;
        bool alreadyDuplicated = false;
        ScriptableInputElementProvider_UIT newProvider = null;

        if (provider != null)
        {
          // Check if the provider was already duplicated
          if (providerMapping.ContainsKey(provider))
          {
            newProvider = providerMapping[provider];
            alreadyDuplicated = true;
          }
          else
          {
            // Create a new provider if the provider was not duplicated before
            if (CreateScriptableObjectAsset<ScriptableInputElementProvider_UIT>(resourcesFolder, provider.name, provider, out var newProviderObject))
            {
              newProvider = newProviderObject as ScriptableInputElementProvider_UIT;
            }
            providerMapping.Add(provider, newProvider);
          }
        }

        newProviderRelations.Add(new StringToGenericDataRelation<ScriptableInputElementProvider_UIT>(key, newProvider));

        // Skip the cleanup of multiple duplicates of the same UnityEngine.Object
        if (alreadyDuplicated) { continue; }

        #region Cleanup multiple duplicates of the same UnityEngine.Object
        //var fields = provider.GetType().GetSerializableFields();
        //foreach (var field in fields)
        //{
        //  var fieldType = field.FieldType;
        //  if (typeof(Object).IsAssignableFrom(fieldType))
        //  {
        //    // TODO Test
        //    //ConsoleLogger.Log($"{field.GetType()} for {field.Name}");

        //    var originalObj = field.GetValue(provider);
        //    if (original == null) { continue; }
        //    var originalObject = originalObj as Object;

        //    var currentObj = field.GetValue(newProvider);
        //    if (currentObj == null) { continue; }
        //    var currentObject = currentObj as Object;

        //    if (duplicatedAssets.TryGetValue(originalObject, out var duplicatedObject))
        //    {
        //      // Handle the case that this object already has a dictionary entry meaning
        //      // it has already been duplicated before and requires the current value
        //      // to be replaced by the one in the dictionary

        //      // Mark the object that was duplicated to be removed
        //      objectsToRemove.AddIfNotContains(currentObject);

        //      // Assign the object reference from the dictionary
        //      field.SetValue(newProvider, duplicatedObject);
        //    }
        //    else
        //    {
        //      // Add an entry for the object so future object references
        //      // will be using this object instead of their own duplication
        //      duplicatedAssets.Add(originalObject, currentObject);
        //    }
        //  }
        //  else if (fieldType.IsListType() &&
        //           typeof(Object).IsAssignableFrom(fieldType.GetGenericArguments()[0]))
        //  {
        //    // Original list
        //    var originalListValue = field.GetValue(provider);
        //    GetListAsArrayFromObject(originalListValue, out var elementType, out var originalList);

        //    // Duplicated list
        //    var newField = newProvider.GetType().GetSerializableField(field.Name);
        //    var duplicateListValue = newField.GetValue(newProvider);
        //    GetListAsArrayFromObject(duplicateListValue, out var newElementType, out var duplicatedList);

        //    if (elementType != newElementType)
        //    {
        //      ConsoleLogger.LogWarning($"List element types don't match!");
        //      continue;
        //    }

        //    for (int i = 0; i < duplicatedList.Length; i++)
        //    {
        //      var originalObj = originalList[i];
        //      if (original == null) { continue; }
        //      var originalObject = originalObj as Object;
        //      var currentObj = duplicatedList[i];
        //      if (currentObj == null) { continue; }
        //      var currentObject = currentObj as Object;

        //      if (duplicatedAssets.TryGetValue(originalObject, out var duplicatedObject))
        //      {
        //        // Handle the case that this object already has a dictionary entry meaning
        //        // it has already been duplicated before and requires the current value
        //        // to be replaced by the one in the dictionary

        //        // Mark the object that was duplicated to be removed
        //        objectsToRemove.AddIfNotContains(currentObject);

        //        // Assign the object reference from the dictionary
        //        duplicatedList[i] = duplicatedObject;
        //      }
        //      else
        //      {
        //        // Add an entry for the object so future object references
        //        // will be using this object instead of their own duplication
        //        duplicatedAssets.Add(originalObject, currentObject);
        //      }
        //    }

        //    // Cast back to the correct list
        //    var listConstructor = typeof(List<>).MakeGenericType(elementType).
        //      GetConstructor(new System.Type[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
        //    var modifiedList = listConstructor.Invoke(new object[] { duplicatedList });

        //    // Assign the list back to the field
        //    newField.SetValue(newProvider, modifiedList);
        //  }
        //}
        #endregion
      }

      providers_UIT.SetRelations(newProviderRelations);
      //var newAssetPath = AssetDatabase.GetAssetPath(providers_UIT);
      EditorUtility.SetDirty(providers_UIT);

      foreach (var obj in objectsToRemove)
      {
        if (obj == null) { continue; }
        var objPath = AssetDatabase.GetAssetPath(obj);
        ConsoleLogger.Log($"Deleting object at path: {objPath}", Common.LogType.Debug);
        AssetDatabase.DeleteAsset(objPath);
      }

      // OLD

      //foreach (var r in relations)
      //{
      //  var key = r.Item1;
      //  var value = r.Item2;
      //  ScriptableInputElementProvider_UIT newValue = null;

      //  if (value != null)
      //  {
      //    if (providerMapping.ContainsKey(value))
      //    {
      //      newValue = providerMapping[value];
      //    }
      //    else
      //    {
      //      newValue = CreateAsset(resourcesFolder, value.name, value);
      //      providerMapping.Add(value, newValue);
      //    }
      //  }

      //  newProviderRelations.Add(new StringToGenericDataRelation<ScriptableInputElementProvider_UIT>(key, newValue));
      //}

      //providers_UIT.SetRelations(newProviderRelations);
      //EditorUtility.SetDirty(providers_UIT);
      return providers_UIT;
    }

    //public static T CreateAssetWithPresets<T>(string path, string assetName) where T : ScriptableObject
    //{
    //  var instance = ScriptableObject.CreateInstance<T>();
    //  var presets = Preset.GetDefaultPresetsForObject(instance);
    //  foreach (var p in presets)
    //  {
    //    p.ApplyTo(instance);
    //  }

    //  path = AssetDatabase.GenerateUniqueAssetPath($"{path}/{assetName}.asset");
    //  AssetDatabase.CreateAsset(instance, path);
    //  //var guid = AssetDatabase.GUIDFromAssetPath(path);
    //  var asset = AssetDatabase.LoadAssetAtPath<T>(path);
    //  EditorUtility.SetDirty(asset);
    //  return asset;
    //}

    // TODO Refactor and cleanup
    public static bool CreateScriptableObjectAsset<T>(string path, string assetName, ScriptableObject source, out ScriptableObject newAsset, bool usePresetIfNull = true)
      where T : ScriptableObject
    {
      newAsset = null;
      string newAssetPath = string.Empty;

      // Check if the object was already duplicated in this duplication process
      if (source != null && duplicatedAssets != null && duplicatedAssets.TryGetValue(source, out var duplicatedAsset))
      {
        if (duplicatedAsset is T)
        {
          newAsset = duplicatedAsset as T;
          return true;
        }
      }

      if (typeof(StyleSheet).IsAssignableFrom(typeof(T)) || source is StyleSheet)
      {
        newAsset = null;
        return true;
      }

      bool applyPresets = source == null && usePresetIfNull;

      if (source != null/* && false*/)
      {
        string sourceAssetPath = AssetDatabase.GetAssetPath(source);
        newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{assetName}.asset");
        if (AssetDatabase.CopyAsset(sourceAssetPath, newAssetPath))
        {
          newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);

        }
      }

      //var instance = source == null ? ScriptableObject.CreateInstance<T>() : ScriptableObject.Instantiate(source);
      var instance = source == null ? ScriptableObject.CreateInstance<T>() : newAsset != null ? newAsset : ScriptableObject.Instantiate(source);
      var sb = new StringBuilder();
      sb.Append(typeof(T).Name);

      if (applyPresets)
      {
        var presets = Preset.GetDefaultPresetsForObject(instance);
        foreach (var p in presets)
        {
          p.ApplyTo(instance);
        }
      }

      if (/*instance is not Theme/StyleSheet && instance is not VisualTreeAsset*/true)
      {
        var fields = instance.GetType().GetSerializableFields().ToList();
        if (instance is ThemeStyleSheet themeStyleSheet)
        {
          var extraFields = typeof(StyleSheet).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
          foreach (var f in extraFields)
          {
            fields.AddIfNotContains(f);
          }
        }
        foreach (var field in fields)
        {
          var fieldType = field.FieldType;
          sb.AppendLine($"{field.Name}: {fieldType.Name}");
          var fieldValue = field.GetValue(instance);
          if (fieldValue == null) { continue; }

          if (typeof(GameObject).IsAssignableFrom(fieldType))
          {
            if (fieldValue is GameObject obj && obj != null)
            {
              // Check if the object is a prefab
              // TODO Use a different method
              if (!obj.IsSceneObject())
              {
                //GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(obj);
                //GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(obj, $"{path}/{obj.name}.prefab");

                var prefabAsset = CreatePrefabVariant(obj, path);
                //if (DuplicateAsset(obj, path, out var prefabAsset))
                {
                  field.SetValue(instance, prefabAsset);
                }
              }
            }
          }
          else if (fieldType == typeof(VisualTreeAsset) && fieldValue is VisualTreeAsset visualTreeAsset)
          {
            if (DuplicateAsset(visualTreeAsset, path, out var duplicate))
            {
              field.SetValue(instance, duplicate);
            }
          }
          else if (typeof(StyleSheet).IsAssignableFrom(fieldType) && fieldValue is StyleSheet styleSheet)
          {
            // TODO
            //if (fieldValue is ThemeStyleSheet themeStyleSheet)
            //{
            //  ConsoleLogger.Log("Is Theme");
            //}
            if (DuplicateAsset(styleSheet, path, out var duplicate))
            {
              field.SetValue(instance, duplicate);
            }
          }
          else if (fieldType.IsListType() || fieldType.IsArray)
          {
            if (fieldType.IsListType() && fieldType.IsGenericType)
            {
              var listType = fieldType.GetGenericTypeDefinition();
            }

            if (fieldType.IsArray)
            {
              var arrayType = fieldType.GetElementType();
              //var declaringType = field.DeclaringType;
              if (typeof(Object).IsAssignableFrom(arrayType))
              {
                //ConsoleLogger.Log("Element Type " + arrayType);
                //ConsoleLogger.Log("Declaring Type " + declaringType);

                var newList = DuplicateList(((Object[])fieldValue).ToList(), path);
                var newArray = newList.ToArray();
                field.SetValue(instance, newArray);
              }
            }


            if (fieldValue is List<GameObject> objList)
            {
              var newList = DuplicateList(objList, path);
              field.SetValue(instance, newList);
            }

            if (fieldValue is List<StyleSheet> list1)
            //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
            {
              var newList = DuplicateList(list1, path);
              field.SetValue(instance, newList);
            }

            else if (fieldValue is List<VisualTreeAsset> list2)
            //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
            {
              var newList = DuplicateList(list2, path);
              field.SetValue(instance, newList);
            }
          }
          else
          {
            //ConsoleLogger.Log("Type:" + fieldType.Name);
          }
        }
      }
      //else
      //{
      //  ConsoleLogger.Log("Is TSS");
      //}

      //ConsoleLogger.Log(sb.ToString());
      if (newAsset == null && instance != null)
      {
        newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{assetName}.asset");
        AssetDatabase.CreateAsset(instance, newAssetPath);
        //var guid = AssetDatabase.GUIDFromAssetPath(path);
      }
      newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);

      if (newAsset != null && source != null)
      {
        duplicatedAssets?.AddOrUpdateDictionaryItem(source, newAsset);
      }

      EditorUtility.SetDirty(newAsset);
      return newAsset != null;
    }

    public static bool CreateAsset<T>(string path, string assetName,
      ScriptableObject source, out ScriptableObject newAsset, bool usePresetIfNull = true)
      where T : ScriptableObject
    {
      // Check if the object was already duplicated in this duplication process
      if (source != null && duplicatedAssets != null && duplicatedAssets.TryGetValue(source, out var duplicatedAsset))
      {
        if (duplicatedAsset is T)
        {
          newAsset = duplicatedAsset as T;
          return true;
        }
      }

      newAsset = null;
      string newAssetPath = string.Empty;

      if (typeof(StyleSheet) == (typeof(T)) || source is StyleSheet)
      {
        newAsset = null;
        return true;
      }

      if (source != null)
      {
        string sourceAssetPath = AssetDatabase.GetAssetPath(source);
        newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{assetName}.asset");
        if (AssetDatabase.CopyAsset(sourceAssetPath, newAssetPath))
        {
          newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);
        }
      }

      //var instance = source == null ? ScriptableObject.CreateInstance<T>() : ScriptableObject.Instantiate(source);
      var instance = source == null ? ScriptableObject.CreateInstance<T>() : newAsset != null ? newAsset : ScriptableObject.Instantiate(source);
      var sb = new StringBuilder();
      sb.Append(typeof(T).Name);

      bool applyPresets = source == null && usePresetIfNull;
      if (applyPresets)
      {
        var presets = Preset.GetDefaultPresetsForObject(instance);
        foreach (var p in presets)
        {
          p.ApplyTo(instance);
        }
      }

      if (/*instance is not Theme/StyleSheet && instance is not VisualTreeAsset*/true)
      {
        // TODO Should this get all private and serializable fields?
        var fields = instance.GetType().GetSerializableFields().ToList();
        if (instance is ThemeStyleSheet themeStyleSheet)
        {
          var privateFields = typeof(StyleSheet).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
          foreach (var f in privateFields)
          {
            fields.AddIfNotContains(f);
          }
        }

        bool updateFields = true;
        bool isStyleSheet = typeof(StyleSheet) == typeof(T) || (source != null && source.GetType() == typeof(StyleSheet));
        updateFields = !isStyleSheet;

        if (updateFields)
        {
          foreach (var field in fields)
          {
            var fieldType = field.FieldType;
            sb.AppendLine($"{field.Name}: {fieldType.Name}");
            var fieldValue = field.GetValue(instance);
            if (fieldValue == null) { continue; }

            // GameObject
            if (typeof(GameObject).IsAssignableFrom(fieldType))
            {
              if (fieldValue is GameObject obj && obj != null)
              {
                // TODO Use a different method
                // Check if the object is a prefab
                if (!obj.IsSceneObject())
                {
                  var prefabAsset = CreatePrefabVariant(obj, path);
                  //if (DuplicateAsset(obj, path, out var prefabAsset))
                  {
                    field.SetValue(instance, prefabAsset);
                  }
                }
              }
            }
            // UXML
            else if (fieldType == typeof(VisualTreeAsset) && fieldValue is VisualTreeAsset visualTreeAsset)
            {
              if (DuplicateAsset(visualTreeAsset, path, out var duplicate))
              {
                field.SetValue(instance, duplicate);
              }
            }
            // USS
            else if (typeof(StyleSheet).IsAssignableFrom(fieldType) && fieldValue is StyleSheet styleSheet)
            {
              if (DuplicateAsset(styleSheet, path, out var duplicate))
              {
                field.SetValue(instance, duplicate);
              }
            }
            // List
            else if (fieldType.IsListType())
            {
              // TODO Make this more generic to support any kind of list?

              //var listValue = fieldValue as IList;
              //if (fieldType.IsGenericType)
              //{
              //  var listType = fieldType.GetGenericTypeDefinition();
              //}

              //if (fieldType.IsGenericType)
              //{
              //  var elementType = fieldType.GetGenericTypeDefinition();
              //  if (typeof(Object).IsAssignableFrom(elementType))
              //  {
              //    //var objectList = fieldValue as List<Object>;
              //    var objectList = (IList)fieldValue;
              //    //var newList = DuplicateList(objectList, path);
              //    field.SetValue(instance, newList);
              //  }
              //}

              if (fieldValue is List<GameObject> objList)
              {
                var newList = DuplicateList(objList, path);
                field.SetValue(instance, newList);
              }

              else if (fieldValue is List<StyleSheet> list1)
              //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
              {
                var newList = DuplicateList(list1, path);
                field.SetValue(instance, newList);
              }

              else if (fieldValue is List<VisualTreeAsset> list2)
              //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
              {
                var newList = DuplicateList(list2, path);
                field.SetValue(instance, newList);
              }
            }
            // Array
            else if (fieldType.IsArray)
            {
              // TODO Check if this works

              var arrayType = fieldType.GetElementType();

              //var declaringType = field.DeclaringType;
              if (typeof(Object).IsAssignableFrom(arrayType))
              {
                //ConsoleLogger.Log("Element Type " + arrayType);
                //ConsoleLogger.Log("Declaring Type " + declaringType);

                var newList = DuplicateList(((Object[])fieldValue).ToList(), path);
                var newArray = newList.ToArray();
                field.SetValue(instance, newArray);
              }
            }
            else
            {
              // Unsupported fieldType
              //ConsoleLogger.Log("Type:" + fieldType.Name);
            }
          }
        }
      }

      //ConsoleLogger.Log(sb.ToString());
      if (newAsset == null && instance != null)
      {
        newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{assetName}.asset");
        AssetDatabase.CreateAsset(instance, newAssetPath);
      }
      newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);
      EditorUtility.SetDirty(newAsset);

      if (source != null && newAsset != null)
      {
        duplicatedAssets?.AddOrUpdateDictionaryItem(source, newAsset);
      }

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      return newAsset != null;
    }

    private static void DuplicateFields(object instance, string path)
    {
      if (/*instance is not Theme/StyleSheet && instance is not VisualTreeAsset*/true)
      {
        var fields = instance.GetType().GetSerializableFields().ToList();
        if (instance is ThemeStyleSheet themeStyleSheet)
        {
          var extraFields = typeof(StyleSheet).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
          foreach (var f in extraFields)
          {
            fields.AddIfNotContains(f);
          }
        }
        foreach (var field in fields)
        {
          var fieldType = field.FieldType;
          var fieldValue = field.GetValue(instance);
          if (fieldValue == null) { continue; }

          if (typeof(GameObject).IsAssignableFrom(fieldType))
          {
            if (fieldValue is GameObject obj && obj != null)
            {
              // Check if the object is a prefab
              // TODO Use a different method
              if (!obj.IsSceneObject())
              {
                //GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(obj);
                //GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(obj, $"{path}/{obj.name}.prefab");

                var prefabAsset = CreatePrefabVariant(obj, path);
                //if (DuplicateAsset(obj, path, out var prefabAsset))
                {
                  field.SetValue(instance, prefabAsset);
                }
              }
            }
          }
          else if (fieldType == typeof(VisualTreeAsset) && fieldValue is VisualTreeAsset visualTreeAsset)
          {
            if (DuplicateAsset(visualTreeAsset, path, out var duplicate))
            {
              field.SetValue(instance, duplicate);
            }
          }
          else if (fieldType == typeof(StyleSheet) && fieldValue is StyleSheet styleSheet)
          {
            // TODO
            //if (fieldValue is ThemeStyleSheet themeStyleSheet)
            //{
            //  ConsoleLogger.Log("Is Theme");
            //}
            if (DuplicateAsset(styleSheet, path, out var duplicate))
            {
              field.SetValue(instance, duplicate);
            }
          }
          else if (fieldType.IsListType() || fieldType.IsArray)
          {
            if (fieldType.IsListType() && fieldType.IsGenericType)
            {
              var listType = fieldType.GetGenericTypeDefinition();
            }

            if (fieldType.IsArray)
            {
              var arrayType = fieldType.GetElementType();
              //var declaringType = field.DeclaringType;
              if (typeof(Object).IsAssignableFrom(arrayType))
              {
                //ConsoleLogger.Log("Element Type " + arrayType);
                //ConsoleLogger.Log("Declaring Type " + declaringType);

                var newList = DuplicateList(((Object[])fieldValue).ToList(), path);
                var newArray = newList.ToArray();
                field.SetValue(instance, newArray);
              }
            }


            if (fieldValue is List<GameObject> objList)
            {
              var newList = DuplicateList(objList, path);
              field.SetValue(instance, newList);
            }

            if (fieldValue is List<StyleSheet> list1)
            //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
            {
              var newList = DuplicateList(list1, path);
              field.SetValue(instance, newList);
            }

            else if (fieldValue is List<VisualTreeAsset> list2)
            //if (typeof(IList).IsAssignableFrom(fieldType) && fieldType != typeof(string))
            {
              var newList = DuplicateList(list2, path);
              field.SetValue(instance, newList);
            }
          }
          else
          {
            //ConsoleLogger.Log("Type:" + fieldType.Name);
          }
        }
      }
    }

    private static bool DuplicateAsset<T>(T original, string saveDirectoryPath, out T duplicate)
      where T : UnityEngine.Object
    {
      // Check if the object was already duplicated in this duplication process
      if (original != null && duplicatedAssets != null && duplicatedAssets.TryGetValue(original, out var duplicatedAsset))
      {
        if (duplicatedAsset is T)
        {
          duplicate = duplicatedAsset as T;
          return true;
        }
      }

      duplicate = null;
      string newAssetPath = string.Empty;

      if (AssetDatabase.Contains(original))
      {
        var sourceAssetPath = AssetDatabase.GetAssetPath(original);
        var extension = Path.GetExtension(sourceAssetPath);
        newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{saveDirectoryPath}/{original.name}{extension}");
        //newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{saveDirectoryPath}/{original.name}_Copy{extension}");

        if (/*original is not ThemeStyleSheet && !typeof(ThemeStyleSheet).IsAssignableFrom(typeof(T)) &&*/
            //original is not StyleSheet && !typeof(StyleSheet).IsAssignableFrom(typeof(T)) &&
            original is not VisualTreeAsset && !typeof(VisualTreeAsset).IsAssignableFrom(typeof(T)) &&
            original is ScriptableObject so)
        {
          if (CreateScriptableObjectAsset<ScriptableObject>(saveDirectoryPath, so.name, so,
            out var newAsset, usePresetIfNull: false /*TODO*/))
          {
            duplicate = newAsset as T;
            if (original != null && duplicate != null)
            {
              duplicatedAssets?.AddOrUpdateDictionaryItem(original, duplicate);
              return true;
            }
          }
        }

        try
        {
          if (AssetDatabase.CopyAsset(sourceAssetPath, newAssetPath))
          {
            duplicate = AssetDatabase.LoadAssetAtPath<T>(newAssetPath);
            if (original != null && duplicate != null)
            {
              duplicatedAssets?.AddOrUpdateDictionaryItem(original, duplicate);
              #region TSS
              if (true && duplicate is ThemeStyleSheet themeStyleSheet)
              {
                var originalTheme = original as ThemeStyleSheet;
                //ConsoleLogger.Log("Is TSS !!!");
                var type = themeStyleSheet.GetType();
                var methods = type.GetPrivateMethods();
                var publicMethods = type.GetMethods();
                var serializableFields = type.GetSerializableFields();
                var fields = type.GetPrivateFields();
                var flattenMethod = type.GetPrivateMethod("FlattenImportedStyleSheetsRecursive");
                var markDirtyMethod = type.GetPrivateMethod("MarkDirty");
                var finalizeMethod = type.GetPrivateMethod("Finalize");
                var importedWithWarningsMethod = type.GetPrivateMethod("set_importedWithWarnings");
                var imports = themeStyleSheet.GetType().GetPrivateField("imports");
                //var importsValue = imports.GetValue(themeStyleSheet);

                var importsValue = imports.GetValue(originalTheme);
                if (importsValue.GetType().IsArray)
                {
                  var enumerable = importsValue as IEnumerable;
                  var elementType = enumerable.GetType().GetElementType();
                  System.Type listType = typeof(List<>).MakeGenericType(elementType);
                  var list = System.Activator.CreateInstance(listType) as IList;

                  foreach (var item in enumerable)
                  {
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    foreach (var f in elementType.GetFields())
                    {
                      if (f.GetValue(item) is UnityEngine.Object o)
                      {
                        if (DuplicateAsset(o, saveDirectoryPath, out var duplicateObject))
                        {
                          p.Add(f.Name, duplicateObject);
                          f.SetValue(item, duplicateObject);
                        }
                        else
                        {
                          p.Add(f.Name, o);
                          ConsoleLogger.LogWarning($"Unable to duplicate {o.name}");
                        }
                      }
                      else
                      {
                        p.Add(f.Name, f.GetValue(item));
                      }
                    }

                    list.Add(item);
                    //var result = typeof(Enumerable).GetMethod("Cast")
                    //             .MakeGenericMethod(itemType)
                    //             .Invoke(null, new object[] { enumerable });

                    //var newInstance = FormatterServices.GetUninitializedObject(itemType);
                    //var t = newInstance.GetType();
                    //foreach (var pair in p)
                    //{
                    //  var field = t.GetPrivateField(pair.Key);
                    //  field?.SetValue(newInstance, pair.Value);
                    //}

                    //var newItemInstance = System.Activator.CreateInstance(itemType, p.ToArray());
                    //DuplicateFields(item, saveDirectoryPath);

                    //var importStructFields = item.GetType().GetSerializableFields();
                    //foreach (var f in importStructFields)
                    //{
                    //  ConsoleLogger.Log($"Field: {f.Name}");
                    //}
                    //ConsoleLogger.Log(item.ToString());
                  }

                  //var elementType = importsValue.GetType().GetElementType();

                  var listObjectType = list.GetType();
                  // Get the type of the list
                  elementType = listObjectType.GetGenericArguments()[0];
                  // Get the "ToArray" method
                  var toArrayMethod = listObjectType.GetMethod("ToArray");
                  // Cast the object to an array
                  var elementsArray = /*(object[])*/toArrayMethod.Invoke(list, null);

                  imports.SetValue(themeStyleSheet, elementsArray);

                  //
                  //finalizeMethod?.Invoke(themeStyleSheet, new object[] { });

                  //ScheduleUtility.InvokeNextFrame(() =>
                  //{
                  //  flattenMethod.Invoke(themeStyleSheet, new object[] { });
                  //  //importedWithWarningsMethod?.Invoke(themeStyleSheet, new object[] { false });
                  //});
                  flattenMethod.Invoke(themeStyleSheet, new object[] { });
                  //markDirtyMethod?.Invoke(themeStyleSheet, new object[] { });
                  //finalizeMethod?.Invoke(themeStyleSheet, new object[] { });
                  duplicate = AssetDatabase.LoadAssetAtPath<T>(newAssetPath);

                  //string text = File.ReadAllText(newAssetPath, Encoding.UTF8);
                  //string[] lines = File.ReadAllLines(newAssetPath, Encoding.UTF8);

                  //EditorUtility.SetDirty(themeStyleSheet);
                }


                //var properties = themeStyleSheet.GetType().GetPrivateProperties();
                //var styleSheetsInfo = themeStyleSheet.GetType().GetPrivateProperty("flattenedRecursiveImports");
                //var styleSheets = styleSheetsInfo.GetValue(duplicate);
                //List<StyleSheet> styleSheetsList = (List<StyleSheet>)styleSheets;
                //styleSheetsList.Add(null);


                //var styleSheets = themeStyleSheet.GetType().GetPrivateProperty("m_FlattenedImportedStyleSheets");

                //var importsField = themeStyleSheet.GetType().GetPrivateField("imports");
                //var importsValue = importsField.GetValue(themeStyleSheet);
                //if (importsValue != null)
                //{
                //  ConsoleLogger.Log($"importsValue: {importsValue.GetType()}");
                //  StyleSheet.ImportStruct[] arrayValue = importsValue as StyleSheet.ImportStruct[];
                //}
              }
              #endregion
              //EditorUtility.SetDirty(duplicate);
              //AssetDatabase.SaveAssets();
              //AssetDatabase.Refresh();
              if (duplicate != null && original != null)
              {
                duplicatedAssets?.AddOrUpdateDictionaryItem(original, duplicate);
              }

              return true;
            }
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
          }
        }
        catch (System.Exception e)
        {
          ConsoleLogger.LogError(e);
          throw;
        }

        return false;
      }

      if (original is GameObject obj)
      {
        duplicate = CreatePrefabVariant(obj, saveDirectoryPath) as T;
        EditorUtility.SetDirty(duplicate);
        return true;
      }

      var fileName = string.IsNullOrEmpty(original.name) ? original.GetType().Name : original.name;
      newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{saveDirectoryPath}/{fileName}.asset");
      //newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{saveDirectoryPath}/{fileName}_Copy.asset");

      AssetDatabase.CreateAsset(original, newAssetPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      duplicate = AssetDatabase.LoadAssetAtPath<T>(newAssetPath);

      if (duplicate != null && original != null)
      {
        duplicatedAssets?.AddOrUpdateDictionaryItem(original, duplicate);
      }

      //if (duplicate is ThemeStyleSheet tss)
      //{
      //  ScheduleUtility.InvokeNextFrame(() => AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tss)));
      //}
      return true;
    }

    public static List<T> DuplicateList<T>(List<T> original, string path) where T : UnityEngine.Object
    {
      var duplicate = new List<T>(original);

      for (int i = 0; i < original.Count; i++)
      {
        var item = original[i];
        if (item == null) { continue; }

        if (typeof(T) == typeof(GameObject) && !((item as GameObject).IsSceneObject()))
        {
          var prefabAsset = CreatePrefabVariant(item as GameObject, path);
          duplicate[i] = prefabAsset as T;
        }

        else if (DuplicateAsset(item, path, out var innerDuplicate))
        {
          duplicate[i] = innerDuplicate;
        }
      }
      return duplicate;
    }
  }
}