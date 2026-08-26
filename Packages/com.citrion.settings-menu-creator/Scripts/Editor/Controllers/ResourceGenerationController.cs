using CitrioN.Common;
using CitrioN.Common.Editor;
using System;
using System.Collections.Generic;
#if TEXT_MESH_PRO
using TMPro;
#endif
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  // TODO Check if events should be unsubscribed before subscribing
  // When and how often is setup called
  [CreateAssetMenu(fileName = "ResourceGenerationController_",
                   menuName = "CitrioN/Settings Menu Creator/Editor/ScriptableObjects/VisualTreeAsset/Controller/Resource Generation")]
  public class ResourceGenerationController : ScriptableVisualTreeAssetController
  {
    [SerializeField]
    protected ResourcesCreatorProfile profile;
    [SerializeField]
    protected VisualElement root;
    [SerializeField]
    [HideInInspector]
    private bool containerFoldoutValue_General = false;
    [SerializeField]
    [HideInInspector]
    private bool containerFoldoutValue_UGUI = false;
    [SerializeField]
    [HideInInspector]
    private bool containerFoldoutValue_UIT = false;

    private const string INFO_CONTAINER_CLASS = "container__info";
    private const string GENERAL_CONTAINER_CLASS = "container__general";
    private const string SETTINGS_CONTAINER_CLASS = "container__settings";
    private const string GENERATE_RESOURCES_BUTTON_CLASS = "button__generate-resources";

    private const string CREATE_FOLDERS_TOGGLE_CLASS = "toggle__create-folders";
    private const string CREATE_SCENE_TOGGLE_CLASS = "toggle__create-scene";
    private const string SOURCE_COLLECTION_OBJECT_FIELD_CLASS = "object-field__source-collection";
    private const string ADDITIONAL_SETTINGS_PROPERTY_FIELD_CLASS = "property-field__settings";
    private const string SETTINGS_SAVER_PROPERTY_FIELD_CLASS = "property-field__saver";

    private const string SAVER_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__saver";

    // UGUI
    private const string MENU_UGUI_PROPERTY_FIELD_CLASS = "property-field__ugui__menu";
    private const string MENU_UGUI_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__ugui__menu";
    private const string MENU_LAYOUT_UGUI_PROPERTY_FIELD_CLASS = "property-field__ugui__menu-layout";
    private const string MENU_LAYOUT_UGUI_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__ugui__menu-layout";
    private const string PROVIDERS_UGUI_PROPERTY_FIELD_CLASS = "property-field__ugui__providers";
    private const string PROVIDERS_UGUI_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__ugui__providers";
    private const string STYLE_PROFILE_UGUI_PROPERTY_FIELD_CLASS = "property-field__ugui__style-profile";
    private const string STYLE_PROFILE_UGUI_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__ugui__style-profile";

    // UI Toolkit
    private const string MENU_UIT_PROPERTY_FIELD_CLASS = "property-field__uit__menu";
    private const string MENU_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__menu";
    private const string PROVIDERS_UIT_PROPERTY_FIELD_CLASS = "property-field__uit__providers";
    private const string PROVIDERS_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__providers";
    private const string STYLE_PROFILE_UIT_PROPERTY_FIELD_CLASS = "property-field__uit__style-profile";
    private const string STYLE_PROFILE_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__style-profile";

    private const string DOCUMENTS_UIT_PROPERTY_FIELD_CLASS = "property-field__uit__documents";
    private const string LAYOUTS_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__layouts";
    private const string DOCUMENTS_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__documents";
    private const string STYLE_SHEETS_UIT_PROPERTY_FIELD_CLASS = "property-field__uit__style-sheets";
    private const string STYLE_SHEETS_UIT_SELECT_PRESET_BUTTON_CLASS = "button__select-preset__uit__style-sheets";

    private const string CREATE_PROFILE_BUTTON_CLASS = "button__create-profile";

    private const string PROFILE_FIELD_CLASS = "profile-field";

    protected bool HasProfile => Profile != null;

    public ResourcesCreatorProfile Profile
    {
      get => profile;
      set
      {
        bool setDirty = profile != value;
        profile = value;
        if (setDirty)
        {
          EditorUtility.SetDirty(this);
        }
      }
    }

    // profile-create-button
    //

    protected void UpdateFields()
    {
      if (root == null) { return; }

      var settingsContainer = root.Q(className: SETTINGS_CONTAINER_CLASS);
      settingsContainer.Show(HasProfile);

      var infoContainer = root.Q(className: INFO_CONTAINER_CLASS);
      infoContainer.Show(!HasProfile);

      if (Profile == null) { return; }

      var so = new SerializedObject(Profile);

      SetupBaseField<Toggle, bool>(so, CREATE_FOLDERS_TOGGLE_CLASS, "createAllFolders", "Create All Folders");
      SetupBaseField<Toggle, bool>(so, CREATE_SCENE_TOGGLE_CLASS, "createDedicatedScene", "Create Dedicated Scene");
      SetupObjectField<SettingsCollection>(so, SOURCE_COLLECTION_OBJECT_FIELD_CLASS, "sourceCollection", "Source Collection");
      SetupPropertyField(so, ADDITIONAL_SETTINGS_PROPERTY_FIELD_CLASS, "additionalSettings", "Additional Settings");
      SetupPropertyField(so, SETTINGS_SAVER_PROPERTY_FIELD_CLASS, "settingsSaver", "Settings Saver", 2);

      // Settings Menu UGUI
      SetupPropertyField(so, MENU_UGUI_PROPERTY_FIELD_CLASS, "settingsMenu_UGUI", "Settings Menu", 2);
      // Menu Layout
      SetupPropertyField(so, MENU_LAYOUT_UGUI_PROPERTY_FIELD_CLASS, "menuLayoutTemplate", "Menu Layout", 2);
      // Providers
      SetupPropertyField(so, PROVIDERS_UGUI_PROPERTY_FIELD_CLASS, "inputElementProviders_UGUI", "Provider Collection", 2);
      // Style Profile
      SetupPropertyField(so, STYLE_PROFILE_UGUI_PROPERTY_FIELD_CLASS, "styleProfile", "Style Profile", 2);

      // Settings Menu UI Toolkit
      SetupPropertyField(so, MENU_UIT_PROPERTY_FIELD_CLASS, "settingsMenu_UIT", "Settings Menu", 2);
      // Documents
      SetupPropertyField(so, DOCUMENTS_UIT_PROPERTY_FIELD_CLASS, "menuDocuments", "Documents", 2);
      // Style Sheets
      SetupPropertyField(so, STYLE_SHEETS_UIT_PROPERTY_FIELD_CLASS, "menuStyleSheets", "Style Sheets", 2);
      // Providers
      SetupPropertyField(so, PROVIDERS_UIT_PROPERTY_FIELD_CLASS, "inputElementProviders_UIT", "Provider Collection", 2);
      // Style Profile
      SetupPropertyField(so, STYLE_PROFILE_UIT_PROPERTY_FIELD_CLASS, "styleProfile_UIT", "Style Profile", 2);
    }

    protected void SetupBaseField<T1, T2>(SerializedObject so, string className, string propertyName, string label, int tooltipParentOffset = 1)
      where T1 : BaseField<T2>
    {
      var field = root.Q<T1>(className: className);
      if (field != null)
      {
        var property = so.FindProperty(propertyName);
        if (property != null)
        {
          field.BindProperty(property);
        }
        field.label = label;

        AddTooltipsAttribute.AddTooltip(so.targetObject.GetType(), field, propertyName, tooltipParentOffset);
      }
    }

    protected void SetupObjectField<T>(SerializedObject so, string className, string propertyName, string label, int tooltipParentOffset = 1)
      where T : UnityEngine.Object
    {
      var field = root.Q<ObjectField>(className: className);
      if (field != null)
      {
        field.objectType = typeof(T);
        var property = so.FindProperty(propertyName);
        if (property != null)
        {
          field.BindProperty(property);
        }
        field.label = label;

        AddTooltipsAttribute.AddTooltip(so.targetObject.GetType(), field, propertyName, tooltipParentOffset);
      }
    }

    protected void SetupPropertyField(SerializedObject so, string className, string propertyName, string label, int tooltipParentOffset = 1)
    {
      var field = root.Q<PropertyField>(className: className);
      if (field != null)
      {
        var property = so.FindProperty(propertyName);
        if (property != null)
        {
          field.BindProperty(property);
        }
        field.label = label;

        AddTooltipsAttribute.AddTooltip(so.targetObject.GetType(), field, propertyName, tooltipParentOffset);
      }
    }

    protected void SetupButton(string className, string label, Action onClick)
    {
      var button = root.Q<Button>(className: className);
      if (button != null)
      {
        button.clicked -= onClick;
        button.clicked += onClick;
        button.text = label;
      }
    }

    protected void SetupPresetButton(string className, Action onClick)
    {
      var button = root.Q<Button>(className: className);
      if (button != null)
      {
        button.clicked -= onClick;
        button.clicked += onClick;
        var height = button.layout.height;
        button.style.width = 20;
        button.text = null;

        // Update the image tint color if the light skin is used
        // so the preset icon is clearly visible
        if (!EditorUtilities.IsDarkSkin)
        {
          button.style.unityBackgroundImageTintColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.Button_Border_Color, true);
        }
        button.SetImage(EditorTextures.PRESET);
      }
    }

    public override void Setup(VisualElement root)
    {

      this.root = root;
      var profileField = root.Q<ObjectField>(className: PROFILE_FIELD_CLASS);

      if (profileField != null)
      {
        profileField.label = "Resources Creator Profile";
        profileField.objectType = typeof(ResourcesCreatorProfile);
        profileField.BindProperty(new SerializedObject(this).FindProperty("profile"));
        profileField.UnregisterValueChangedCallback(OnProfileChanged);
        profileField.RegisterValueChangedCallback(OnProfileChanged);//
      }

      UIToolkitStyleUtilities.AddToggleDescriptionsButton(root, root);
      //
      // TODO Called how often?
      SetupButton(CREATE_PROFILE_BUTTON_CLASS, "Create", CreateProfile);

      // General
      SetupPresetButton(SAVER_SELECT_PRESET_BUTTON_CLASS, ShowSaverPresetDropdown);

      // UGUI
      SetupPresetButton(MENU_UGUI_SELECT_PRESET_BUTTON_CLASS, ShowUguiMenuPresetDropdown);
      SetupPresetButton(MENU_LAYOUT_UGUI_SELECT_PRESET_BUTTON_CLASS, ShowMenuLayoutPresetDropdown);
      SetupPresetButton(PROVIDERS_UGUI_SELECT_PRESET_BUTTON_CLASS, ShowUguiProvidersPresetDropdown);
      SetupPresetButton(STYLE_PROFILE_UGUI_SELECT_PRESET_BUTTON_CLASS, ShowUguiStyleProfilePresetDropdown);

      // UI Toolkit
      SetupPresetButton(MENU_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitMenuPresetDropdown);
      SetupPresetButton(PROVIDERS_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitProvidersPresetDropdown);
      SetupPresetButton(LAYOUTS_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitLayoutsPresetDropdown);
      SetupPresetButton(STYLE_PROFILE_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitStyleProfilePresetDropdown);
      SetupPresetButton(DOCUMENTS_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitDocumentsPresetDropdown);
      SetupPresetButton(STYLE_SHEETS_UIT_SELECT_PRESET_BUTTON_CLASS, ShowUitStyleSheetsPresetDropdown);

      SetupButton(GENERATE_RESOURCES_BUTTON_CLASS, "Generate", GenerateResources);

      SerializedObject so = new SerializedObject(this);

      //// General Foldout
      //var foldout_General = root.Q<Foldout>(className: "container__general");
      //foldout_General.BindProperty(so.FindProperty(nameof(containerFoldoutValue_General)));
      ////foldout_General.value = containerFoldoutValue_General;
      ////foldout_General.RegisterValueChangedCallback(OnFoldoutValueChanged_General);

      //// UGUI Foldout
      //var foldout_UGUI = root.Q<Foldout>(className: "container__ugui");
      //foldout_UGUI.BindProperty(so.FindProperty(nameof(containerFoldoutValue_UGUI)));
      ////foldout_UGUI.value = containerFoldoutValue_UGUI;
      ////foldout_UGUI.RegisterValueChangedCallback(OnFoldoutValueChanged_UGUI);

      //// UI Toolkit Foldout
      //var foldout_UIT = root.Q<Foldout>(className: "container__uit");
      //foldout_UIT.BindProperty(so.FindProperty(nameof(containerFoldoutValue_UIT)));//
      ////foldout_UIT.BindProperty()
      ////foldout_UIT.value = containerFoldoutValue_UIT;
      ////foldout_UIT.RegisterValueChangedCallback(OnFoldoutValueChanged_UIT);

      UpdateFields();
    }

    private void OnFoldoutValueChanged_General(ChangeEvent<bool> evt)
    {
      containerFoldoutValue_General = evt.newValue;
    }

    private void OnFoldoutValueChanged_UGUI(ChangeEvent<bool> evt)
    {
      containerFoldoutValue_UGUI = evt.newValue;
    }

    private void OnFoldoutValueChanged_UIT(ChangeEvent<bool> evt)
    {
      containerFoldoutValue_UIT = evt.newValue;
    }

    private void GenerateResources()
    {
      if (Profile == null) { return; }

#if TEXT_MESH_PRO
      var settings = AssetDatabase.FindAssets("t:TMP_Settings");

      if (settings == null || settings.Length < 1)
      {
        DialogUtilities.DisplayDialog("TMP Essential Resources Required", "The 'Text Mesh Pro' essential resources are required. " +
          "Please import them first before proceeding with the menu resources generation.", "Import", () =>
        {
          AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
          AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
          SettingsMenuEditorUtility.ImportTextMeshProEssentialResources();
        });
        return;
      }
#endif

      ResourcesCreatorProfile.CreateResourcesFromProfile(Profile);
    }

    public void OnImportPackageCompleted(string packageName)
    {
      AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
      if (packageName != "TMP Essential Resources") { return; }

      var settings = AssetDatabase.FindAssets("t:TMP_Settings");

      if (settings == null || settings.Length < 1) { return; }

      GenerateResources();
    }

    // General
    private void ShowSaverPresetDropdown() => ShowPresetDropdown("Settings Saver", "Settings Saver");


    // UGUI
    private void ShowUguiMenuPresetDropdown() => ShowPresetDropdown("UGUI Settings Menu", "Settings Menu UGUI");

    private void ShowMenuLayoutPresetDropdown() => ShowPresetDropdown("UGUI Menu Layout", "Settings Menu Layout");

    private void ShowUguiProvidersPresetDropdown() => ShowPresetDropdown("UGUI Providers", "Provider Collection UGUI");

    private void ShowUguiStyleProfilePresetDropdown() => ShowPresetDropdown("UGUI Style Profile", "Style Profile UGUI");


    // UI Toolkit
    private void ShowUitMenuPresetDropdown() => ShowPresetDropdown("UI Toolkit Settings Menu", "Settings Menu UIT");

    private void ShowUitProvidersPresetDropdown() => ShowPresetDropdown("UI Toolkit Providers", "Provider Collection UIT");

    private void ShowUitStyleProfilePresetDropdown() => ShowPresetDropdown("UI Toolkit Style Profile", "Style Profile UIT");

    private void ShowUitLayoutsPresetDropdown() => ShowPresetDropdown("UI Toolkit Menu Layout", "Menu Layout UIT");

    private void ShowUitDocumentsPresetDropdown() => ShowPresetDropdown("UI Toolkit Documents", "Documents");

    private void ShowUitStyleSheetsPresetDropdown() => ShowPresetDropdown("UI Toolkit Style Sheets", "Style Sheets");


    private void OnProfileChanged(ChangeEvent<UnityEngine.Object> evt)
    {
      UpdateFields();
    }

    protected void ShowPresetDropdown(string header = null, string presetGroup = null)
    {
      bool hasGroup = !string.IsNullOrEmpty(presetGroup);
      List<GenericDropdownItemData<PresetDropdownData>> dropdownData = new List<GenericDropdownItemData<PresetDropdownData>>();
      var presetData = AssetUtilities.GetAllAssetsOfType<PresetDropdownData>();
      foreach (var data in presetData)
      {
        if (data.Preset == null) { continue; }
        if (data.Preset.CanBeAppliedTo(Profile))
        {
          // Skip the preset dropdown data if it is not part of the right group
          if (hasGroup && !data.Groups.Contains(presetGroup)) { continue; }

          dropdownData.AddIfNotContains(new GenericDropdownItemData<PresetDropdownData>(data, data.DisplayName, data.DropdownPath, data.Priority));
        }
      }
      GenericDropdown<PresetDropdownData>.Show(dropdownData, header, OnPresetSelectedNew, new Vector2(280, 260));
    }

    private void OnPresetSelectedNew(GenericDropdownItem<PresetDropdownData> item)
    {
      if (Profile == null || item == null || item.value == null) { return; }

      var preset = item.value.Preset;

      if (preset == null) { return; }

      preset.ApplyTo(Profile);
    }

    private void CreateProfile()
    {
      Profile = AssetUtilities.CreateScriptableObjectAssetWithSaveFilePanel<ResourcesCreatorProfile>();
      return;
      //var path = EditorUtility.SaveFilePanel("Select save path", "Assets", "ResourcesCreatorProfile_" + ".asset", "asset");

      //if (path.Length != 0)
      //{
      //  path = AssetUtilities.GetRelativePath(path);
      //  var instance = ScriptableObject.CreateInstance<ResourcesCreatorProfile>();
      //  PresetUtilities.ApplyPresets(instance);
      //  var newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}");
      //  AssetDatabase.CreateAsset(instance, newAssetPath);
      //  var newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);
      //  EditorUtility.SetDirty(newAsset);
      //  Profile = newAsset as ResourcesCreatorProfile;
      //  EditorUtilities.PingObject(Profile);
      //}
    }
  }
}