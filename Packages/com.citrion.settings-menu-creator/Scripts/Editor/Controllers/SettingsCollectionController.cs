using CitrioN.Common;
using CitrioN.Common.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  // TODO Check if events should be unsubscribed before subscribing
  // When and how often is setup called
  [CreateAssetMenu(fileName = "SettingsCollectionController_",
                   menuName = "CitrioN/Settings Menu Creator/Editor/ScriptableObjects/VisualTreeAsset/Controller/Settings Collection")]
  public class SettingsCollectionController : ScriptableVisualTreeAssetController
  {
    [SerializeField]
    protected SettingsCollection settingsCollection;

    public SettingsCollectionEditorNew settingsCollectionEditor;

    public ListView settingsList;

    public SettingHolder selectedHolder;

    [SerializeField]
    protected VisualElement root;
    [SerializeField]
    [HideInInspector]
    private bool containerFoldoutValue_General = false;

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

    private const string CREATE_PROFILE_BUTTON_CLASS = "button__create-profile";

    private const string PROFILE_FIELD_CLASS = "profile-field";

    protected bool HasProfile => SettingsCollection != null;

    public string PathToListItem(int index) => $"settings.Array.data[{index}]";

    public SettingsCollection SettingsCollection
    {
      get => settingsCollection;
      set
      {
        bool setDirty = settingsCollection != value;
        settingsCollection = value;
        if (setDirty)
        {
          EditorUtility.SetDirty(this);
        }
      }
    }

    protected void UpdateFields()
    {
      if (root == null) { return; }

      var settingsContainer = root.Q(className: SETTINGS_CONTAINER_CLASS);
      settingsContainer.Show(HasProfile);

      var infoContainer = root.Q(className: INFO_CONTAINER_CLASS);
      infoContainer.Show(!HasProfile);
      SerializedObject so = null;

      if (SettingsCollection != null)
      {
        so = new SerializedObject(SettingsCollection);
      }

      //SetupBaseField<Toggle, bool>(so, CREATE_SCENE_TOGGLE_CLASS, "createDedicatedScene", "Create Dedicated Scene");
      //SetupObjectField<SettingsCollection>(so, SOURCE_COLLECTION_OBJECT_FIELD_CLASS, "sourceCollection", "Source Collection");
      //SetupPropertyField(so, ADDITIONAL_SETTINGS_PROPERTY_FIELD_CLASS, "additionalSettings", "Additional Settings");
      //SetupPropertyField(so, SETTINGS_SAVER_PROPERTY_FIELD_CLASS, "settingsSaver", "Settings Saver", 2);

      var settingsCollectionElement = root.Q(className: "container__settings-collection");
      //if (settingsCollectionEditor == null)
      //{
      //  settingsCollectionEditor = CreateInstance<SettingsCollectionEditor>();
      //}

      if (settingsCollectionElement != null)
      {
        settingsCollectionElement.RemoveAllChildren();
        if (so != null)
        {
          var inspectorElement = new InspectorElement(so);
          //var selectedSettingContainer = root.Q(className: "container__selected-setting");
          //var selectedSettingContainer2 = inspectorElement.Q(className: "container__selected-setting"); 
          //if (selectedSettingContainer2 != null)
          //{
          //  selectedSettingContainer.Add(selectedSettingContainer2);
          //}
          settingsCollectionElement.Add(inspectorElement);
          //settingsList = inspectorElement.Q<ListView>(className: SettingsCollectionEditorNew.LIST_CLASS);
          //if (settingsList != null)
          //{
          //  settingsList.onSelectionChange += OnSelectionChanged;
          //}
        }
      }
      //InspectorElement.FillDefaultInspector(settingsCollectionElement, so, );
    }

    private void SetupSelectedSettingDetails()
    {
      var index = settingsList.selectedIndex;
      var selectedSettingContainer = root.Q(className: "container__selected-setting");
      if (selectedSettingContainer != null)
      {
        selectedSettingContainer.RemoveAllChildren();
        if (index < 0 || index >= SettingsCollection.Settings.Count) { return; }
        selectedHolder = SettingsCollection.Settings[index];
        if (selectedHolder == null) { return; }
        var so = new SerializedObject(SettingsCollection);
        var settingsProperty = so.FindProperty(PathToListItem(index));
        var selectedSettingField = new PropertyField(settingsProperty);
        selectedSettingField.RegisterValueChangeCallback(OnSettingChanged);
        selectedSettingField.Bind(so);
        selectedSettingContainer.Add(selectedSettingField);

        //var label = new Label(SettingsCollection.Settings[index].Identifier);
        //selectedSettingContainer.Add(label);
      }
    }

    private void OnSettingChanged(SerializedPropertyChangeEvent evt)
    {
      //Debug.Log("Setting was changed!");
      settingsList.RefreshItem(settingsList.selectedIndex);
    }

    private void OnSelectionChanged(IEnumerable<object> enumerable)
    {
      //var list = enumerable.ToList();
      //var index = settingsList.selectedIndex;
      SetupSelectedSettingDetails();
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
      root.SetFlexGrow(1);
      var settingsCollectionField = root.Q<ObjectField>(className: PROFILE_FIELD_CLASS);

      if (settingsCollectionField != null)
      {
        settingsCollectionField.label = "Settings Collection";
        settingsCollectionField.objectType = typeof(SettingsCollection);
        settingsCollectionField.BindProperty(new SerializedObject(this).FindProperty("settingsCollection"));
        settingsCollectionField.UnregisterValueChangedCallback(OnSettingsCollectionChanged);
        settingsCollectionField.RegisterValueChangedCallback(OnSettingsCollectionChanged);
      }

      UIToolkitStyleUtilities.AddToggleDescriptionsButton(root, root);

      // General
      //SetupPresetButton(SAVER_SELECT_PRESET_BUTTON_CLASS, ShowSaverPresetDropdown);

      //SetupButton(GENERATE_RESOURCES_BUTTON_CLASS, "Generate", GenerateResources);

      SerializedObject so = new SerializedObject(this);

      // General Foldout
      //var foldout_General = root.Q<Foldout>(className: "container__general");
      //foldout_General.BindProperty(so.FindProperty(nameof(containerFoldoutValue_General)));

      UpdateFields();
    }

    private void OnFoldoutValueChanged_General(ChangeEvent<bool> evt)
    {
      containerFoldoutValue_General = evt.newValue;
    }

    //    private void GenerateResources()
    //    {
    //      if (Profile == null) { return; }

    //#if TEXT_MESH_PRO
    //      var settings = AssetDatabase.FindAssets("t:TMP_Settings");

    //      if (settings == null || settings.Length < 1)
    //      {
    //        DialogUtilities.DisplayDialog("TMP Essential Resources Required", "The 'Text Mesh Pro' essential resources are required. " +
    //          "Please import them first before proceeding with the menu resources generation.", "Import", () =>
    //          {
    //            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
    //            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
    //            SettingsMenuEditorUtility.ImportTextMeshProEssentialResources();
    //          });
    //        return;
    //      }
    //#endif

    //  ResourcesCreatorProfile.CreateResourcesFromProfile(Profile);
    //}

    // General
    private void ShowSaverPresetDropdown() => ShowPresetDropdown("Settings Saver", "Settings Saver");

    private void OnSettingsCollectionChanged(ChangeEvent<UnityEngine.Object> evt)
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
        if (data.Preset.CanBeAppliedTo(SettingsCollection))
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
      if (settingsCollection == null || item == null || item.value == null) { return; }

      var preset = item.value.Preset;

      if (preset == null) { return; }

      preset.ApplyTo(settingsCollection);
    }
  }
}