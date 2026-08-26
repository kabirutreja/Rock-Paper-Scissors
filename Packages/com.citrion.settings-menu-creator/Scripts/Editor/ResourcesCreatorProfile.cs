using CitrioN.Common;
using CitrioN.StyleProfileSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CreateAssetMenu(fileName = "ResourcesCreatorProfile_",
                   menuName = "CitrioN/Settings Menu Creator/Resources Creator Profile")]
  public class ResourcesCreatorProfile : ScriptableObject
  {
    [SerializeField]
    [Tooltip("Whether the entire folder structure should be created even if some folders may end up being empty. " +
             "This is useful if you don't generate all files but want to use the suggested folder structure so you can later " +
             "add your files to those folders. You can always change the folder structure in any way you see fit for your project. " +
             "If enabled there will for example be UI Toolkit related folders even if no UI Toolkit files were generated.")]
    protected bool createAllFolders = false;
    [SerializeField]
    [Tooltip("Whether a functional scene containing the settings menu prefab should be created. The scene will contain " +
             "all relevant objects such as a camera, light, event system and of course the settings menu. If you generate resources for both " +
             "UGUI and UI Toolkit you should manually delete one of the two menus in your scene after the resource generation.")]
    protected bool createDedicatedScene = true;

    [SerializeField]
    [Tooltip("A 'SettingsCollection' to duplicate (including all its references). " +
             "This is useful if you already have a collection with certain providers, " +
             "layout etc and you want to use that as a starting point for a new menu. " +
             "This could also be a demo scene SettingsCollection you like to use for your own settings menu.\n\n" +
             "Note:\n" +
             "The settings menu prefab itself will not be duplicated as it is not referenced on a SettingsCollection!")]
    protected SettingsCollection sourceCollection;
    [SerializeField]
    [Tooltip("A list of SettingsCollections. All settings in those collections will be added to the generated " +
             "SettingsCollection in the order they appear in the list.")]
    protected List<SettingsCollection> additionalSettings = new List<SettingsCollection>();

    [SerializeField]
    [Tooltip("The SettingsSaver to duplicate and use for saving and loading setting values.")]
    protected SettingsSaver settingsSaver;

    //[Header("UGUI")]
    [SerializeField]
    [Tooltip("The settings menu prefab to create a variant of. Will be the default one in almost all cases.")]
    protected SettingsMenu_UGUI settingsMenu_UGUI;
    [SerializeField]
    [Tooltip("The settings menu layout prefab to create a variant of. This is the prefab containing your GameObject hierarchy that defines your layout. " +
             "This can for example be a basic or a more complex tab menu layout.")]
    protected GameObject menuLayoutTemplate;
    [SerializeField]
    [Tooltip("The StylePofile to duplicate. The style profile allows easy customization of various aspects of the menu such as " +
             "font colors or sizes, background colors and much more.")]
    protected StyleProfile styleProfile;
    [SerializeField]
    [Tooltip("The input element providers to duplicate. This is a collection of providers that handle the creation and initialization " +
             "of input elements of a menu such as dropdowns, sliders etc.")]
    InputElementProviderCollection_UGUI inputElementProviders_UGUI;

    //[Header("UI Toolkit")]
    [SerializeField]
    [Tooltip("The settings menu prefab to create a variant of. You have to use the one with style listeners for the style profile to have an effect.")]
    protected SettingsMenu_UIT settingsMenu_UIT;
    [SerializeField]
    [Tooltip("The UXML files to duplicate.")]
    protected List<VisualTreeAsset> menuDocuments = new List<VisualTreeAsset>();
    [SerializeField]
    [Tooltip("The USS files to duplicate.")]
    protected List<StyleSheet> menuStyleSheets = new List<StyleSheet>();
    [SerializeField]
    [Tooltip("The StylePofile to duplicate. The style profile allows easy customization of various aspects of the menu such as " +
             "font colors or sizes, background colors and much more.")]
    protected StyleProfile styleProfile_UIT;
    [SerializeField]
    [Tooltip("The input element providers to duplicate. This is a collection of providers that handle the creation and initialization " +
             "of input elements of a menu such as dropdowns, sliders etc.")]
    InputElementProviderCollection_UIT inputElementProviders_UIT;

    [MenuItem("CONTEXT/ResourcesCreatorProfile/Create Resources", false, 1)]
    private static void Command_CreateResourcesFromProfile(MenuCommand command)
    {
      var profile = (ResourcesCreatorProfile)command.context;
      CreateResourcesFromProfile(profile);
    }

    public static void CreateResourcesFromProfile(ResourcesCreatorProfile profile)
    {
      if (profile == null) { return; }

      var collection = profile.sourceCollection != null ?
        profile.sourceCollection : CreateInstance<SettingsCollection>();

      if (profile.additionalSettings != null)
      {
        foreach (var c in profile.additionalSettings)
        {
          if (c == null) { continue; }
          for (int i = 0; i < c.Settings.Count; i++)
          {
            var setting = c.Settings[i];
            var duplicate = SettingHolder.GetCopy(setting);
            if (duplicate == null)
            {
              ConsoleLogger.LogWarning("SettingHolder duplicate is null");
            }
            else
            {
              collection.Settings.Add(duplicate);
            }
          }
        }
      }

      //// Assign UGUI providers
      //if (profile.inputElementProviders_UGUI != null)
      //{
      //  collection.InputElementProviders_UGUI = profile.inputElementProviders_UGUI;
      //}

      //// Assign UI Toolkit providers
      //if (profile.inputElementProviders_UIT != null)
      //{
      //  collection.InputElementProviders_UIT = profile.inputElementProviders_UIT;
      //}

      SettingsSaver settingsSaver = profile.settingsSaver;

      if (settingsSaver == null && collection != null)
      {
        settingsSaver = collection.SettingsSaver;
      }

      #region UGUI
      StyleProfile styleProfile = profile.styleProfile;
      GameObject menuLayoutTemplate = profile.menuLayoutTemplate;
      InputElementProviderCollection_UGUI inputElementProviders_UGUI = profile.inputElementProviders_UGUI;

      if (profile.settingsMenu_UGUI != null)
      {
        var menu_UGUI = profile.settingsMenu_UGUI.GetComponent<SettingsMenu_UGUI>();

        if (menu_UGUI != null)
        {
          // Style Profile
          if (styleProfile == null)
          {
            var assigner = menu_UGUI.GetComponent<AssignStyleProfileToListenersInHierarchy>();
            if (assigner != null)
            {
              styleProfile = assigner.StyleProfile;
            }
          }

          // Menu Layout Prefab
          if (menuLayoutTemplate == null)
          {
            menuLayoutTemplate = menu_UGUI.MenuTemplate;
          }
        }

        if (collection != null)
        {
          //Input Element Providers
          if (inputElementProviders_UGUI == null)
          {
            inputElementProviders_UGUI = collection.InputElementProviders_UGUI;
          }
        }
      }
      #endregion

      #region UI Toolkit
      List<VisualTreeAsset> menuDocuments = profile.menuDocuments;
      List<StyleSheet> menuStyleSheets = profile.menuStyleSheets;
      StyleProfile styleProfile_UIT = profile.styleProfile_UIT;
      InputElementProviderCollection_UIT inputElementProviders_UIT = profile.inputElementProviders_UIT;

      if (profile.settingsMenu_UIT != null)
      {
        var menu_UIT = profile.settingsMenu_UIT.GetComponent<SettingsMenu_UIT>();

        if (menu_UIT != null)
        {
          // Menu Documents
          if (menuDocuments == null || menuDocuments.Count < 1)
          {
            menuDocuments = menu_UIT.MenuTemplates;
          }

          // Menu Style Sheets
          if (menuStyleSheets == null || menuStyleSheets.Count < 1)
          {
            menuStyleSheets = menu_UIT.StyleSheets;
          }
        }

        // Style Profile
        if (styleProfile_UIT == null)
        {
          var assigner = menu_UIT.GetComponent<AssignStyleProfileToListenersInHierarchy>();
          if (assigner != null)
          {
            styleProfile_UIT = assigner.StyleProfile;
          }
        }
      }

      if (collection != null)
      {
        //Input Element Providers
        if (inputElementProviders_UIT == null)
        {
          inputElementProviders_UIT = collection.InputElementProviders_UIT;
        }
      }
      #endregion

      // Select the profile so the resources will be generated in the same folder
      Selection.activeObject = profile;

      SettingsCollectionCreationUtility.CreateSettingsCollectionAndResources
        (collection, false, false,
        styleProfile, menuLayoutTemplate,
        profile.settingsMenu_UGUI, inputElementProviders_UGUI,
        profile.settingsMenu_UIT,
        menuDocuments, menuStyleSheets, styleProfile_UIT,
        inputElementProviders_UIT, profile.createAllFolders, profile.createDedicatedScene,
        settingsSaver);
    }
  }
}