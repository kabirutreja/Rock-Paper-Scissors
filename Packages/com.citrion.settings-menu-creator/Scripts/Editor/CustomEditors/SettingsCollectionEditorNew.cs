using CitrioN.Common;
using CitrioN.Common.Editor;
using CitrioN.StyleProfileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  //[InitializeOnLoad]
  //public static class SuppressExceptions
  //{
  //  static SuppressExceptions()
  //  {
  //    Application.logMessageReceivedThreaded += HandleLog;
  //  }

  //  private static void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
  //  {
  //    if (type == UnityEngine.LogType.Exception && logString.StartsWith("ObjectDisposedException: SerializedProperty settings.Array.data"))
  //    {
  //      // Suppress the log by doing nothing here
  //      Debug.unityLogger.logEnabled = false;
  //      ScheduleUtility.InvokeDelayedByFrames(() => { Debug.unityLogger.logEnabled = true; }, 3);
  //      return;
  //    }

  //    // For all other logs, use the default handler
  //    //Debug.unityLogger.Log(type, logString);
  //  }
  //}

  // TODO Cleanup entire script!
  [CanEditMultipleObjects]
  [CustomEditor(typeof(SettingsCollection))]
  public class SettingsCollectionEditorNew : EditorFromVisualTreeAsset_SettingsMenu
  {
    #region Fields
    protected SettingsCollection collection;

    protected ListView settingsList;
    protected VisualElement root;
    protected PropertyField propertyField;
    protected VisualElement selectedSettingContainer;

    protected VisualElement multiSelectContainer;
    protected Toggle applyImmediatelyToggle;
    protected Toggle overrideIdentifierWhenCopiedToggle;
    protected TextField parentIdentifierTextField;

    protected int boundIndex = -1;
    protected bool isUnity2022OrNewer = false;

    protected Toggle sharedApplyImmediatelyToggle;
    protected TextField inputElementParentClassTextField;

    protected const string ADVANCED_FOLDOUT_TEXT = "Extra";

    public const string LIST_CLASS = "settings-list";
    protected const string ITEM_FOLDOUT_CLASS = "item-foldout";
    protected const string ITEM_PROPERTY_CLASS = "item-property";
    protected const string ITEM_ROOT_CLASS = "item-root";

    protected const string ADVANCED_FOLDOUT_CLASS = "foldout__advanced-settings";
    protected const string APPLY_IMMEDIATELY_MODE_PROPERTY_FIELD_CLASS = "property-field__apply-immediately-mode";
    protected const string IDENTIFIER_PROPERTY_FIELD_CLASS = "property-field__identifier";
    protected const string SETTINGS_SAVER_PROPERTY_FIELD_CLASS = "property-field__settings-saver";
    protected const string INPUT_ELEMENT_TEMPLATES_UGUI_FIELD_CLASS = "property-field__input-element-templates_ugui";
    protected const string INPUT_ELEMENT_TEMPLATES_UIT_FIELD_CLASS = "property-field__input-element-templates_uitoolkit";
    protected const string POST_PROCESS_PROFILE_FIELD_CLASS = "property-field__post-process-profile";
    protected const string VOLUME_PROFILE_FIELD_CLASS = "property-field__volume-profile";
    protected const string AUDIO_MIXER_FIELD_CLASS = "property-field__audio-mixer";
    protected const string LOCALIZER_FIELD_CLASS = "property-field__localizer";
    protected const string LAYOUT_PREFAB_FIELD_CLASS = "property-field__layout-prefab";
    protected const string STYLE_PROFILE_FIELD_CLASS = "property-field__style-profile";
    protected const string PROVIDER_COLLECTION_OVERRIDE_FIELD_CLASS = "property-field__provider-collection-override";
    protected const string ADD_STYLE_PROFILE_EDITOR_LISTENER_FIELD_CLASS = "toggle__add-style-profile-editor-listener";
    protected const string ADD_APPLY_STYLE_PROFILE_FIELD_CLASS = "toggle__add-apply-style-profile";

    protected const string APPLY_IMMEDIATELY_TOGGLE_CLASS = "toggle__apply-immediately";
    protected const string OVERRIDE_IDENTIFIER_WHEN_COPIED_TOGGLE_CLASS = "toggle__override-identifier-when-copied";
    protected const string PARENT_IDENTIFIER_FIELD_CLASS = "field__parent-identifier";

    protected const string REFRESH_LIST_BUTTON_CLASS = "button__refresh-list-view";
    protected const string REBUILD_LIST_BUTTON_CLASS = "button__rebuild-list-view";
    protected const string SELECT_ALL_BUTTON_CLASS = "button__select-all";
    protected const string DESELECT_ALL_BUTTON_CLASS = "button__deselect-all";
    protected const string CLEAR_ALL_BUTTON_CLASS = "button__clear-all";
    protected const string OPEN_IN_EDITOR_BUTTON_CLASS = "button__open-in-editor";
    protected const string OPEN_LAYOUT_PREFAB_BUTTON_CLASS = "button__open-layout-prefab";
    protected const string CREATE_PREFAB_BUTTON_CLASS = "button__create-prefab";
    protected const string UPDATE_PREFAB_BUTTON_CLASS = "button__update-prefab";
    protected const string ADD_ELEMENTS_BUTTON_CLASS = "button__add-elements";
    protected const string ADD_SELECTED_ELEMENTS_BUTTON_CLASS = "button__add-selected-elements";
    protected const string INITIALIZE_ELEMENTS_BUTTON_CLASS = "button__initialize-elements";
    protected const string MATCH_ORDER_ELEMENTS_BUTTON_CLASS = "button__match-order-elements";
    protected const string MATCH_ORDER_SELECTED_ELEMENTS_BUTTON_CLASS = "button__match-order-selected_elements";
    protected const string MATCH_ORDER_LIST_BUTTON_CLASS = "button__match-order-list";
    protected const string DESTROY_ELEMENTS_BUTTON_CLASS = "button__destroy-elements";
    protected const string DESTROY_SELECTED_ELEMENTS_BUTTON_CLASS = "button__destroy-selected-elements";
    protected const string SELECT_ELEMENTS_BUTTON_CLASS = "button__select-elements";
    protected const string SELECT_SELECTED_ELEMENTS_BUTTON_CLASS = "button__select-selected-elements";
    protected const string SHOW_ALL_SETTING_OBJECTS_BUTTON_CLASS = "button__show-setting-objects";
    protected const string ADD_STYLEPROFILE_SCRIPTS_BUTTON_CLASS = "button__add-styleprofile-scripts";
    protected const string REMOVE_STYLEPROFILE_SCRIPTS_BUTTON_CLASS = "button__remove-styleprofile-scripts";

    protected const string SELECT_ALL_EVENT_NAME = "SettingsCollection Select All";
    protected const string DESELECT_ALL_EVENT_NAME = "SettingsCollection Deselect All";

    protected const string DUPLICATE_SELECTED_EVENT_NAME = "SettingsCollection Duplicate Selected";

    protected const string EXPAND_ALL_EVENT_NAME = "SettingsCollection Expand All";
    protected const string EXPAND_SELECTED_EVENT_NAME = "SettingsCollection Expand Selected";
    protected const string COLLAPSE_ALL_EVENT_NAME = "SettingsCollection Collapse All";
    protected const string COLLAPSE_SELECTED_EVENT_NAME = "SettingsCollection Collapse Selected";

    protected const string SELECT_TAB_ALL_EVENT_NAME = "SettingsCollection Select Tab All";
    protected const string SELECT_TAB_SELECTED_EVENT_NAME = "SettingsCollection Select Tab Selected";
    #endregion

    public string PathToListItem(int index) => $"settings.Array.data[{index}]";

    public override VisualElement CreateInspectorGUI()
    {
      //var root = new VisualElement();
      root = base.CreateInspectorGUI();

      collection = serializedObject.targetObject as SettingsCollection;

      Undo.undoRedoPerformed += OnUndoPerformed;
      //Undo.postprocessModifications += MyPostprocessModificationsCallback;

      propertyField = null;
      selectedSettingContainer = root.Q(className: "container__selected-setting");
      multiSelectContainer = root.Q(className: "container__multi-setting-selection");

#if UNITY_2022_1_OR_NEWER
      isUnity2022OrNewer = true;
#endif

      InitSettingsList(root);

      SetupExtraSettings(root);

      SetupButtons(root);

      RegisterEventListeners();

      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);

      return root;
    }

    private void OnDestroy()
    {
      UnregisterEventListeners();
    }

    private void RegisterEventListeners()
    {
      GlobalEventHandler.AddEventListener<SettingsCollection>(
      SettingsCollection.SETTINGS_COLLECTION_CHANGED_EVENT_NAME, OnSettingsCollectionReset);

      GlobalEventHandler.AddEventListener<SettingsCollection>(SELECT_ALL_EVENT_NAME, OnSelectAll);
      GlobalEventHandler.AddEventListener<SettingsCollection>(DESELECT_ALL_EVENT_NAME, OnDeselectAll);

      GlobalEventHandler.AddEventListener<SettingsCollection>(DUPLICATE_SELECTED_EVENT_NAME, OnDuplicateSelected);

      GlobalEventHandler.AddEventListener<SettingsCollection>(EXPAND_ALL_EVENT_NAME, OnExpandAll);
      GlobalEventHandler.AddEventListener<SettingsCollection>(EXPAND_SELECTED_EVENT_NAME, OnExpandSelected);
      GlobalEventHandler.AddEventListener<SettingsCollection>(COLLAPSE_ALL_EVENT_NAME, OnCollapseAll);
      GlobalEventHandler.AddEventListener<SettingsCollection>(COLLAPSE_SELECTED_EVENT_NAME, OnCollapseSelected);

      GlobalEventHandler.AddEventListener<SettingsCollection, int>(SELECT_TAB_ALL_EVENT_NAME, OnSelectTabAll);
      GlobalEventHandler.AddEventListener<SettingsCollection, int>(SELECT_TAB_SELECTED_EVENT_NAME, OnSelectTabSelected);

      GlobalEventHandler.AddEventListener<bool>("Sync ApplyImmediately", SyncSelected_ApplyImmediately);
      GlobalEventHandler.AddEventListener<bool>("Sync OverrideIdentifierWhenCopied", SyncSelected_OverrideIdentifierWhenCopied);
      GlobalEventHandler.AddEventListener<string>("Sync InputElementProviderParent",
                                                  SyncSelected_InputElementParentClass);
    }

    private void UnregisterEventListeners()
    {
      GlobalEventHandler.RemoveEventListener<SettingsCollection>(
        SettingsCollection.SETTINGS_COLLECTION_CHANGED_EVENT_NAME, OnSettingsCollectionReset);

      GlobalEventHandler.RemoveEventListener<SettingsCollection>(SELECT_ALL_EVENT_NAME, OnSelectAll);
      GlobalEventHandler.RemoveEventListener<SettingsCollection>(DESELECT_ALL_EVENT_NAME, OnDeselectAll);

      GlobalEventHandler.RemoveEventListener<SettingsCollection>(DUPLICATE_SELECTED_EVENT_NAME, OnDuplicateSelected);

      GlobalEventHandler.RemoveEventListener<SettingsCollection>(EXPAND_ALL_EVENT_NAME, OnExpandAll);
      GlobalEventHandler.RemoveEventListener<SettingsCollection>(EXPAND_SELECTED_EVENT_NAME, OnExpandSelected);
      GlobalEventHandler.RemoveEventListener<SettingsCollection>(COLLAPSE_ALL_EVENT_NAME, OnCollapseAll);
      GlobalEventHandler.RemoveEventListener<SettingsCollection>(COLLAPSE_SELECTED_EVENT_NAME, OnCollapseSelected);

      GlobalEventHandler.RemoveEventListener<SettingsCollection, int>(SELECT_TAB_ALL_EVENT_NAME, OnSelectTabAll);
      GlobalEventHandler.RemoveEventListener<SettingsCollection, int>(SELECT_TAB_SELECTED_EVENT_NAME, OnSelectTabSelected);

      GlobalEventHandler.RemoveEventListener<bool>("Sync ApplyImmediately", SyncSelected_ApplyImmediately);
      GlobalEventHandler.RemoveEventListener<bool>("Sync OverrideIdentifierWhenCopied", SyncSelected_OverrideIdentifierWhenCopied);
      GlobalEventHandler.RemoveEventListener<string>("Sync InputElementProviderParent",
                                                     SyncSelected_InputElementParentClass);
    }

    private void InvokeOnSelectedElements(Action<VisualElement> action)
    {
      if (settingsList?.itemsSource == null) { return; }
      var indices = settingsList.selectedIndices.ToList();
      InvokeOnElements(indices, action);
    }

    private void InvokeOnSelectedItems(Action<SettingHolder> action)
    {
      if (settingsList?.itemsSource == null) { return; }
      var indices = settingsList.selectedIndices.ToList();
      InvokeOnItems(indices, action);
    }

    private void InvokeOnElements(List<int> indices, Action<VisualElement> action)
    {
      var settingsCount = settingsList.itemsSource.Count;

      foreach (var i in indices)
      {
        if (i >= 0 && i < settingsCount)
        {
#if UNITY_2021_1_OR_NEWER
          var elem = settingsList.GetRootElementForIndex(i);
#else
          var elem = settingsList.ElementAt(i);
#endif
          action?.Invoke(elem);
        }
      }
    }

    private void InvokeOnItems(List<int> indices, Action<SettingHolder> action)
    {
      var settingsCount = settingsList.itemsSource.Count;
      var items = settingsList.itemsSource;

      foreach (var i in indices)
      {
        if (i >= 0 && i < settingsCount)
        {
          var elem = items[i];
          if (elem is SettingHolder holder)
          {
            action?.Invoke(holder);
          }
        }
      }
    }

    private void InvokeOnAllElements(Action<VisualElement> action)
    {
      if (settingsList?.itemsSource == null) { return; }

      for (int i = 0; i < settingsList.itemsSource.Count; i++)
      {
#if UNITY_2021_1_OR_NEWER
        var elem = settingsList.GetRootElementForIndex(i);
#else
        var elem = settingsList.ElementAt(i);
#endif
        action?.Invoke(elem);
      }
    }

    private void InvokeOnAllItems(Action<SettingHolder> action)
    {
      if (settingsList?.itemsSource == null) { return; }
      var items = settingsList.itemsSource;

      for (int i = 0; i < settingsList.itemsSource.Count; i++)
      {
        var elem = items[i];
        if (elem is SettingHolder holder)
        {
          action?.Invoke(holder);
        }
      }
    }

    private void SelectTabForElement(VisualElement elem, int index)
    {
      var tabMenu = elem?.Q<TabMenu>();
      tabMenu?.SelectTab(index);
    }

    private void ExpandElement(VisualElement elem, bool expand)
    {
      var foldout = elem?.Q<Foldout>();
      if (foldout != null)
      {
        foldout.value = expand;
      }
    }

    private void OnSelectTabSelected(SettingsCollection collection, int index)
    {
      if (collection != this.collection) { return; }

      InvokeOnSelectedElements((elem) => { SelectTabForElement(elem, index); });
    }

    private void OnSelectTabAll(SettingsCollection collection, int index)
    {
      if (collection != this.collection) { return; }

      InvokeOnAllElements((elem) => { SelectTabForElement(elem, index); });
    }

    private void OnExpandSelected(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        ExpandSelected();
      }
    }

    private void OnExpandAll(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        ExpandAll();
      }
    }

    private void OnCollapseAll(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        CollapseAll();
      }
    }

    private void OnCollapseSelected(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        CollapseSelected();
      }
    }

    private void OnSelectAll(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        SelectAll();
      }
    }

    private void OnDeselectAll(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        DeselectAll();
      }
    }

    private void OnSettingsCollectionReset(SettingsCollection settings)
    {
      if (settings != collection) { return; }
      RebuildList();
    }

    private void SelectAll()
    {
      if (settingsList?.itemsSource == null) { return; }
      var indices = Enumerable.Range(0, settingsList.itemsSource.Count).ToList();
      settingsList.SetSelection(indices);
    }

    public void DeselectAll() => settingsList?.ClearSelection();

    private void ExpandAll() => InvokeOnAllElements((elem) => { ExpandElement(elem, true); });

    private void ExpandSelected() => InvokeOnSelectedElements((elem) => { ExpandElement(elem, true); });

    private void CollapseAll() => InvokeOnAllElements((elem) => { ExpandElement(elem, false); });

    private void CollapseSelected() => InvokeOnSelectedElements((elem) => { ExpandElement(elem, false); });

    private void OnDuplicateSelected(SettingsCollection collection)
    {
      if (collection == this.collection)
      {
        DuplicateSelected();
      }
    }

    private void DuplicateSelected()
    {
      if (settingsList?.itemsSource == null || collection?.Settings == null) { return; }

      var selectedIndex = settingsList.selectedIndex;

      var selectedIndices = settingsList.selectedIndices;

      foreach (var index in selectedIndices)
      {
        selectedIndex = index;

        if (selectedIndex >= 0 && selectedIndex < settingsList.itemsSource.Count)
        {
          var holder = collection.Settings[selectedIndex];
          var holderDuplicate = SettingHolder.GetCopy(holder);

          if (holderDuplicate == null)
          {
            ConsoleLogger.LogWarning("SettingHolder duplicate is null");
          }
          else
          {
            collection.Settings.Add(holderDuplicate);
            EditorUtility.SetDirty(collection);
          }
        }
      }

      RebuildList();
    }

    private void SetupButtons(VisualElement root)
    {
      var rebuildButton = UIToolkitExtensions.SetupVisualElement<Button>(root, REBUILD_LIST_BUTTON_CLASS);
      rebuildButton.tooltip = "Rebuilds/Refreshs the settings list.\n" +
                              "There are some Unity bugs that may require a manual refresh of the list view.";
      rebuildButton.clicked += OnRebuildButtonClicked;
    }

    protected void SetupExtraSettings(VisualElement root)
    {
      var foldout = root.Q<Foldout>(className: ADVANCED_FOLDOUT_CLASS);
      if (foldout == null)
      {
        foldout = new Foldout() { value = false };
        foldout.AddToClassList(ADVANCED_FOLDOUT_CLASS);
        root.Add(foldout);
      }

      //foldout.text = ADVANCED_FOLDOUT_TEXT;

      //var applyImmediatelyModeProperty = serializedObject.FindProperty("applyImmediatelyMode");
      //UIToolkitEditorExtensions.SetupPropertyField(applyImmediatelyModeProperty, root,
      //  APPLY_IMMEDIATELY_MODE_PROPERTY_FIELD_CLASS);

      var identifierProperty = serializedObject.FindProperty("identifier");
      UIToolkitEditorExtensions.SetupPropertyField(identifierProperty, root,
        IDENTIFIER_PROPERTY_FIELD_CLASS);

      var settingsSaverProperty = serializedObject.FindProperty("settingsSaver");
      UIToolkitEditorExtensions.SetupPropertyField(settingsSaverProperty, root,
        SETTINGS_SAVER_PROPERTY_FIELD_CLASS);

      var inputElementTemplatesProperty_UGUI = serializedObject.FindProperty("inputTemplatesUGUI");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementTemplatesProperty_UGUI, root,
        INPUT_ELEMENT_TEMPLATES_UGUI_FIELD_CLASS);

      var inputElementTemplatesProperty_UIT = serializedObject.FindProperty("inputTemplatesUIToolkit");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementTemplatesProperty_UIT, root,
        INPUT_ELEMENT_TEMPLATES_UIT_FIELD_CLASS);

      var audioMixerProperty = serializedObject.FindProperty("audioMixer");
      UIToolkitEditorExtensions.SetupPropertyField(audioMixerProperty, root,
        AUDIO_MIXER_FIELD_CLASS);

      var localizerProperty = serializedObject.FindProperty("localizationProvider");
      UIToolkitEditorExtensions.SetupPropertyField(localizerProperty, root,
        LOCALIZER_FIELD_CLASS);

      #region UI (UGUI)
      var editorSerializedObject = new SerializedObject(this);

      var layoutPrefabProperty = serializedObject.FindProperty("uguiLayoutPrefab");
      var layoutPrefabPropertyField = UIToolkitEditorExtensions.SetupPropertyField(layoutPrefabProperty, root,
        LAYOUT_PREFAB_FIELD_CLASS);

      // TODO Finish description
      string uiFoldoutTooltip = "This section allows you to create or update a layout prefab to be used for your settings menu. " +
        "You can populate it with the necessary UI elements for the current settings of this SettingsCollection. Additionally you can " +
        "add support for live StyleProfile changes in edit mode.\n\nBe aware the when chosing a prepopulated prefab any changes to the " +
        "order of the settings in the list will not be updated and automatic navigation setup will not work correctly. " +
        "If you don't need keyboard/controller navigation you can safely remove the 'Navigation Setter' script on your SettingsMenu_UGUI prefab " +
        "and not worry about navigation allowing you to freely move around your UI elements without having to match the order in the list of settings.";
      AddTooltipsAttribute.CreateTooltip(layoutPrefabPropertyField.parent, layoutPrefabPropertyField, uiFoldoutTooltip, true);

      var styleProfileProperty = serializedObject.FindProperty("styleProfile");
      UIToolkitEditorExtensions.SetupPropertyField(styleProfileProperty, root,
        STYLE_PROFILE_FIELD_CLASS);

      var providerCollectionOverrideProperty = serializedObject.FindProperty("providerCollectionOverride");
      UIToolkitEditorExtensions.SetupPropertyField(providerCollectionOverrideProperty, root,
        PROVIDER_COLLECTION_OVERRIDE_FIELD_CLASS);

      var addEditModeStyleProfileListeningToggle = root.Q<Toggle>(className: ADD_STYLE_PROFILE_EDITOR_LISTENER_FIELD_CLASS);
      if (addEditModeStyleProfileListeningToggle != null)
      {
        addEditModeStyleProfileListeningToggle.label = "addEditModeStyleProfileListening".SplitCamelCase().ToUpperFirstCharacter();
        addEditModeStyleProfileListeningToggle.tooltip = AddTooltipsAttribute.GetTooltipForProperty(typeof(SettingsCollection), "addEditModeStyleProfileListening");
        addEditModeStyleProfileListeningToggle.SetValueWithoutNotify(collection.AddEditModeStyleProfileListening);
        addEditModeStyleProfileListeningToggle.RegisterValueChangedCallback((evt) => { collection.AddEditModeStyleProfileListening = evt.newValue; });
        AddTooltipsAttribute.AddTooltip(typeof(SettingsCollection), addEditModeStyleProfileListeningToggle, "addEditModeStyleProfileListening");
      }

      var addApplyStyleProfileScriptToggle = root.Q<Toggle>(className: ADD_APPLY_STYLE_PROFILE_FIELD_CLASS);
      if (addApplyStyleProfileScriptToggle != null)
      {
        addApplyStyleProfileScriptToggle.label = "addApplyStyleProfileScript".SplitCamelCase().ToUpperFirstCharacter();
        addApplyStyleProfileScriptToggle.tooltip = AddTooltipsAttribute.GetTooltipForProperty(typeof(SettingsCollection), "addApplyStyleProfileScript");
        addApplyStyleProfileScriptToggle.SetValueWithoutNotify(collection.AddApplyStyleProfileScript);
        addApplyStyleProfileScriptToggle.RegisterValueChangedCallback((evt) => { collection.AddApplyStyleProfileScript = evt.newValue; });
        AddTooltipsAttribute.AddTooltip(typeof(SettingsCollection), addApplyStyleProfileScriptToggle, "addApplyStyleProfileScript");
      }

      var openInEditorButton = root.Q<Button>(className: OPEN_IN_EDITOR_BUTTON_CLASS);
      if (openInEditorButton != null)
      {
        openInEditorButton.clicked += () => OpenInEditor();
        openInEditorButton.tooltip = "Opens the dedicated editor window for the SettingsCollection.";
      }

      var openLayoutPrefabButton = root.Q<Button>(className: OPEN_LAYOUT_PREFAB_BUTTON_CLASS);
      if (openLayoutPrefabButton != null)
      {
        openLayoutPrefabButton.clicked += () => OpenPrefab();
        openLayoutPrefabButton.tooltip = "Opens the referenced layout prefab.";
      }

      var createPrefabButton = root.Q<Button>(className: CREATE_PREFAB_BUTTON_CLASS);
      if (createPrefabButton != null)
      {
        createPrefabButton.clicked += () => CreatePrefab(settingsList, collection, true);
        createPrefabButton.tooltip = "Creates a new prefab variant from the referenced prefab with the specified options. " +
          "Adds UI elements for all settings in the list which don't have a corresponding UI element yet. " +
          "Adds StyleProfile related scripts if enabled.";

      }

      var updatePrefabButton = root.Q<Button>(className: UPDATE_PREFAB_BUTTON_CLASS);
      if (updatePrefabButton != null)
      {
        updatePrefabButton.clicked += () => CreatePrefab(settingsList, collection, false);
        updatePrefabButton.tooltip = "Updates the prefab with the specified options. " +
          "Adds UI elements for all settings in the list which don't have a corresponding UI element yet. " +
          "Adds StyleProfile related scripts if enabled.";
      }

      var addUIElementsButton = root.Q<Button>(className: ADD_ELEMENTS_BUTTON_CLASS);
      if (addUIElementsButton != null)
      {
        addUIElementsButton.clicked += () => AddAndReorderElements(settingsList, collection, collection?.UguiLayoutPrefab, false, true, false, true);
        addUIElementsButton.tooltip = "Adds UI elements to the prefab for all settings in the list which don't have a corresponding UI element yet.";
      }

      var addSelectedUIElementsButton = root.Q<Button>(className: ADD_SELECTED_ELEMENTS_BUTTON_CLASS);
      if (addSelectedUIElementsButton != null)
      {
        addSelectedUIElementsButton.clicked += () => AddAndReorderElements(settingsList, collection, collection?.UguiLayoutPrefab, true, true, false, true);
        addSelectedUIElementsButton.tooltip = "Adds UI elements to the prefab for the currently selected settings " +
                                              "in the list which don't have a corresponding UI element yet.";
      }

      var initializeUIElementsButton = root.Q<Button>(className: INITIALIZE_ELEMENTS_BUTTON_CLASS);
      if (initializeUIElementsButton != null)
      {
        initializeUIElementsButton.clicked += () => AddAndReorderElements(settingsList, collection, collection.UguiLayoutPrefab, false, false, true, false);
        initializeUIElementsButton.tooltip = "Initializes/Updates all UI elements in the prefab which correspond to a setting in the list. " +
                                             "This includes updating its label and data such as the values for a dropdown.";
      }

      var matchSettingsOrderButton = root.Q<Button>(className: MATCH_ORDER_LIST_BUTTON_CLASS);
      if (matchSettingsOrderButton != null)
      {
        matchSettingsOrderButton.clicked += () => MatchSettingsOrderToElementOrder();
        matchSettingsOrderButton.tooltip = "Updates the order of settings in the list to match the order of UI elements in the prefab.";
      }

      var matchElementsOrderButton = root.Q<Button>(className: MATCH_ORDER_ELEMENTS_BUTTON_CLASS);
      if (matchElementsOrderButton != null)
      {
        if (!isUnity2022OrNewer) { matchElementsOrderButton.SetText($"{matchElementsOrderButton.text} (Has limits in 2021)"); }
        matchElementsOrderButton.clicked += () => MatchElementsOrderToListOrder(selected: false, true);
        matchElementsOrderButton.tooltip = "Updates the order of UI elements on the prefab to match the order " +
                                           "of settings in the list as close as possible.\n\n" +
                                           "In Unity 2021 it is not possible to reorder objects in the hierarchy on a prefab in which " +
                                           "the objects were not originally added as an override.";
      }

      var matchSelectedElementsOrderButton = root.Q<Button>(className: MATCH_ORDER_SELECTED_ELEMENTS_BUTTON_CLASS);
      if (matchSelectedElementsOrderButton != null)
      {
        if (!isUnity2022OrNewer) { matchSelectedElementsOrderButton.SetText($"{matchSelectedElementsOrderButton.text} (Has limits in 2021)"); }
        matchSelectedElementsOrderButton.clicked += () => MatchElementsOrderToListOrder(selected: true, true);
        matchSelectedElementsOrderButton.tooltip = "Updates the order of UI elements on the prefab to match the order " +
                                                   "of currently selected settings in the list as close as possible.\n\n" +
                                                   "In Unity 2021 it is not possible to reorder objects in the hierarchy on a prefab in which " +
                                                   "the objects were not originally added as an override.";
      }

      var destroyUIElementsButton = root.Q<Button>(className: DESTROY_ELEMENTS_BUTTON_CLASS);
      if (destroyUIElementsButton != null)
      {
        if (!isUnity2022OrNewer) { destroyUIElementsButton.SetText($"{destroyUIElementsButton.text} (Limited in 2021)"); }
        destroyUIElementsButton.clicked += () => DestroyUIElementsForSettings(false);
        destroyUIElementsButton.tooltip = "Destroys all UI elements in the prefab that correspond to a setting in this collection.\n\n" +
                                          "In Unity 2021 it is not possible to destroy objects on a prefab in which the objects were not originally added as an override.";
      }

      var destroySelectedUIElementsButton = root.Q<Button>(className: DESTROY_SELECTED_ELEMENTS_BUTTON_CLASS);
      if (destroySelectedUIElementsButton != null)
      {
        if (!isUnity2022OrNewer) { destroySelectedUIElementsButton.SetText($"{destroySelectedUIElementsButton.text} (Has limits in 2021)"); }
        destroySelectedUIElementsButton.clicked += () => DestroyUIElementsForSettings(true);
        destroySelectedUIElementsButton.tooltip = "Destroys all UI elements in the prefab that correspond to a currently selected setting in this collection.\n\n" +
                                                  "In Unity 2021 it is not possible to destroy objects on a prefab in which the objects were not originally added as an override.";
      }

      var selectUIElementsButton = root.Q<Button>(className: SELECT_ELEMENTS_BUTTON_CLASS);
      if (selectUIElementsButton != null)
      {
        selectUIElementsButton.clicked += () => SelectElementsInPrefab(false);
        selectUIElementsButton.tooltip = "Selects all UI elements in the prefab that correspond to a setting in this collection.";
      }

      var selectSelectdUIElementsButton = root.Q<Button>(className: SELECT_SELECTED_ELEMENTS_BUTTON_CLASS);
      if (selectSelectdUIElementsButton != null)
      {
        selectSelectdUIElementsButton.clicked += () => SelectElementsInPrefab(true);
        selectSelectdUIElementsButton.tooltip = "Selects all UI elements in the prefab that correspond to a currently selected setting in this collection.";
      }

      var showSettingObjectsButton = root.Q<Button>(className: SHOW_ALL_SETTING_OBJECTS_BUTTON_CLASS);
      if (showSettingObjectsButton != null)
      {
        showSettingObjectsButton.clicked += () => ApplyHierarchyFilter("t:SettingObject");
        showSettingObjectsButton.tooltip = "Shows all GameObject's with the SettingsObject script attached in the prefab.";
      }

      var addStyleProfileScriptsButton = root.Q<Button>(className: ADD_STYLEPROFILE_SCRIPTS_BUTTON_CLASS);
      if (addStyleProfileScriptsButton != null)
      {
        addStyleProfileScriptsButton.clicked += () => AddRemoveStyleProfileScripts(collection?.UguiLayoutPrefab, true);
        addStyleProfileScriptsButton.tooltip = "Adds the StyleProfile related scripts to the prefab.";
      }

      var removeStyleProfileScriptsButton = root.Q<Button>(className: REMOVE_STYLEPROFILE_SCRIPTS_BUTTON_CLASS);
      if (removeStyleProfileScriptsButton != null)
      {
        removeStyleProfileScriptsButton.clicked += () => AddRemoveStyleProfileScripts(collection?.UguiLayoutPrefab, false);
        removeStyleProfileScriptsButton.tooltip = "Removes the StyleProfile related scripts from the prefab.";
      }
      #endregion

      var postProcessProfileProperty = serializedObject.FindProperty("postProcessProfile");
      if (postProcessProfileProperty != null)
      {
        UIToolkitEditorExtensions.SetupPropertyField(postProcessProfileProperty, root,
          POST_PROCESS_PROFILE_FIELD_CLASS);
      }

      var volumeProfileProperty = serializedObject.FindProperty("volumeProfile");
      if (volumeProfileProperty != null)
      {
        UIToolkitEditorExtensions.SetupPropertyField(volumeProfileProperty, root,
          VOLUME_PROFILE_FIELD_CLASS);
      }

      var dropArea = root.Q(className: "drop-area");
      if (dropArea != null)
      {
        dropArea.tooltip = "You can drop other SettingCollections here to " +
                           "append all their settings to this one.\n\n" +
                           "You can find some presets in the project by searching for " +
                           "'t:settingscollection preset_' in the packages.";

        Action<VisualElement, object> onDragEnter = (v, o) =>
        {
          if (o is SettingsCollection)
          {
            v.AddToClassList("drag__hover--valid");
          }
          else { v.AddToClassList("drag__hover--invalid"); }
        };
        Action<VisualElement, object> onDragLeave = (v, o) =>
        {
          v.RemoveFromClassList("drag__hover--valid");
          v.RemoveFromClassList("drag__hover--invalid");
        };
        Action<VisualElement, object> onDragPerform = (v, o) =>
        {
          if (o is SettingsCollection collection)
          {
            if (collection != this.collection)
            {
              AddSettingsFromCollection(collection);
            }
          }
        };
        new EditorDragAndDropManipulator(dropArea, typeof(SettingsCollection),
                                         onDragEnter, onDragLeave, onDragPerform);
      }
    }

    private void AddSettingsFromCollection(SettingsCollection collection)
    {
      Undo.RecordObject(serializedObject.targetObject, "Added settings from collection");
      for (int i = 0; i < collection.Settings.Count; i++)
      {
        var setting = collection.Settings[i];
        var duplicate = SettingHolder.GetCopy(setting);
        if (duplicate == null)
        {
          ConsoleLogger.LogWarning("SettingHolder duplicate is null");
        }
        else
        {
          this.collection.Settings.Add(duplicate);
        }
      }
      EditorUtility.SetDirty(serializedObject.targetObject);
      RebuildList();
    }

    private void SyncSelected_ApplyImmediately(bool applyImmediately)
    {
      Undo.RecordObject(serializedObject.targetObject, "Sync apply immediately");
      InvokeOnSelectedItems((h) => h.ApplyImmediately = applyImmediately);
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void SyncSelected_OverrideIdentifierWhenCopied(bool overrideIdentifier)
    {
      Undo.RecordObject(serializedObject.targetObject, "Sync override identifier");
      InvokeOnSelectedItems((h) => h.OverrideIdentifierWhenCopied = overrideIdentifier);
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void SyncSelected_InputElementParentClass(string parentClass)
    {
      if (string.IsNullOrEmpty(parentClass)) { return; };

      Undo.RecordObject(serializedObject.targetObject, "Sync parent class");
      InvokeOnSelectedItems((h) =>
      {
        var providerSettings = h?.InputElementProviderSettings;
        if (providerSettings != null)
        {
          providerSettings.ParentIdentifier = parentClass;
        }
      });
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void OnRefreshButtonClicked()
    {
      serializedObject.Update();
      RefreshItems();
    }

    private void OnRebuildButtonClicked() => RebuildList();

    [ContextMenu("Rebuild List View")]
    private void RebuildList()
    {
      serializedObject.Update();
      serializedObject.ApplyModifiedProperties();
#if UNITY_2021_1_OR_NEWER
      settingsList?.Rebuild();
#else
      RefreshItems();
#endif
    }

    private void InitSettingsList(VisualElement root)
    {
      settingsList = root.Q<ListView>(className: LIST_CLASS);
      if (settingsList == null)
      {
        settingsList = new ListView();
        settingsList.AddToClassList(LIST_CLASS);
        root.Add(settingsList);
      }
      // TODO Only apply the following if the settings list was created via code and
      // was not in the uxml beforehand because then those settings should be used
#if UNITY_2021_1_OR_NEWER
      settingsList.headerTitle = "List";
      settingsList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
      settingsList.reorderMode = ListViewReorderMode.Animated;
      settingsList.fixedItemHeight = 100;
      settingsList.showAddRemoveFooter = true;
#else
      settingsList.itemHeight = 100;
#endif
      settingsList.reorderable = true;
      settingsList.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
      settingsList.itemsSource = collection.Settings;
      settingsList.showBorder = true;
      settingsList.selectionType = SelectionType.Multiple;

      settingsList.makeItem = MakeSettingsListItem;
      settingsList.bindItem = BindSettingsListItem;


#if UNITY_2021_1_OR_NEWER
      settingsList.SetAddRemoveButtonAction(true, OnSettingsListAddButtonClicked);
      settingsList.SetAddRemoveButtonAction(false, OnSettingsListRemoveButtonClicked);
      //CreateAndAttachAdditionalFooterButton(settingsList, "Q", OnSettingsListAddQualitySettingButtonClicked);

      settingsList.itemIndexChanged += OnItemIndexChanged;
#endif

#if UNITY_2022_1_OR_NEWER
      settingsList.selectionChanged += OnSelectionChanged;
#else
      settingsList.onSelectionChange += OnSelectionChanged;
#endif

      AddTooltipsAttribute.AddTooltip(typeof(SettingsCollection), settingsList, "settings");

      //ScheduleUtility.InvokeDelayedByFrames(() => settingsList.AddToSelection(0));
    }

    private void OnSelectionChanged(IEnumerable<object> enumerable)
    {
      UpdateSelectedSettingDetails();
      ScheduleUtility.InvokeDelayedByFrames(UpdateSelectedSettingDetails);
    }

    //private void CreateAndAttachAdditionalFooterButton(ListView listView, string label, Action onClick, params string[] classNames)
    //{
    //  if (listView == null) { return; }
    //  var parent = listView.Q(className: UIToolkitClassNames.LISTVIEW_FOOTER);
    //  if (parent == null)
    //  {
    //    ConsoleLogger.LogWarning("Can't add list view footer button without a footer enabled.\n" +
    //                             "Please enable the footer in the UXML file or via code using " +
    //                             "'list.showAddRemoveFooter = true;'");
    //    return;
    //  }
    //  var button = new Button();
    //  button.text = label;
    //  parent.Add(button);
    //  if (classNames != null)
    //  {
    //    foreach (var c in classNames)
    //    {
    //      button.AddToClassList(c);
    //    }
    //  }
    //  button.clickable = new Clickable(() =>
    //  {
    //    onClick?.Invoke();
    //  });
    //}

    private VisualElement MakeSettingsListItem()
    {
      var root = new VisualElement();
      root.AddToClassList(ITEM_ROOT_CLASS);

      var label = new Label();
      label.AddToClassList("item-label");
      label.displayTooltipWhenElided = true;
      root.Add(label);
      //var foldout = root.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      //if (foldout == null)
      //{
      //  foldout = new Foldout() { value = false };
      //  foldout.AddToClassList(ITEM_FOLDOUT_CLASS);
      //  root.Add(foldout);
      //}

      //var propertyField = new PropertyField();
      //propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
      //if (foldout != null)
      //{
      //  foldout.value = false;
      //  foldout.Add(propertyField);
      //}
      //else
      //{
      //  root.Add(propertyField);
      //}

      return root;
    }

    private void BindSettingsListItem(VisualElement elem, int index)
    {
      SettingHolder holder = collection.Settings[index];

      //// TODO Make this work so the foldout state persists between playmode and recompile?
      //var foldout = elem.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      //if (foldout != null)
      //{
      //  //foldout.viewDataKey = $"Item Foldout {index}";
      //  var holderProperty = serializedObject.FindProperty(PathToListItem(index));
      //  var expandedProperty = holderProperty?.FindPropertyRelative("expanded");

      //  if (expandedProperty != null)
      //  {
      //    foldout.BindProperty(expandedProperty);
      //  }
      //}
      //foldout.UnregisterValueChangedCallback(OnSettingHolderFoldoutChange);
      //foldout.RegisterValueChangedCallback(OnSettingHolderFoldoutChange);

      BindListItemLabel(elem, holder, index);

      //BindPropertyField(elem, holder, index);
    }

    private void BindListItemLabel(VisualElement elem, SettingHolder holder, int index)
    {
      var label = elem.Q<Label>(className: "item-label");
      if (label == null) { return; }
      // Update the label text for the list item
      label.SetText(holder.MenuName);
      label.tooltip = holder.MenuName;

      //var foldout = elem.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      //if (foldout != null)
      //{
      //  foldout.SetText("List Element");
      //  var foldoutLabel = foldout.Q<Label>(className: "unity-foldout__text");
      //  // Update the label text for the list item
      //  foldoutLabel?.SetText(holder.MenuName);
      //}
    }

    private void UpdateSelectedSettingDetails()
    {
      try
      {
        if (selectedSettingContainer != null)
        {
          //selectedSettingContainer.RemoveAllChildren();

          var amountSelected = settingsList.selectedItems.Count();
          var isMultiSelect = amountSelected > 1;

          if (amountSelected < 1)
          {
            boundIndex = -1;
            selectedSettingContainer.Show(false);
            return;
          }

          multiSelectContainer?.Show(isMultiSelect);
          propertyField.Show(!isMultiSelect);

          if (isMultiSelect)
          {
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serializedObject.targetObject);


            var selectedItems = settingsList.selectedItems.ToList();

            if (applyImmediatelyToggle == null)
            {
              applyImmediatelyToggle = root.Q<Toggle>(className: APPLY_IMMEDIATELY_TOGGLE_CLASS);
              applyImmediatelyToggle?.RegisterValueChangedCallback(OnApplyImmediatelyToggleValueChanged);
            }

            if (overrideIdentifierWhenCopiedToggle == null)
            {
              overrideIdentifierWhenCopiedToggle = root.Q<Toggle>(className: OVERRIDE_IDENTIFIER_WHEN_COPIED_TOGGLE_CLASS);
              overrideIdentifierWhenCopiedToggle?.RegisterValueChangedCallback(OnOverrideIdentifierWhenCopiedToggleValueChanged);
            }

            if (parentIdentifierTextField == null)
            {
              parentIdentifierTextField = root.Q<TextField>(className: PARENT_IDENTIFIER_FIELD_CLASS);
              parentIdentifierTextField?.RegisterValueChangedCallback(OnParentIdentifierTextFieldValueChanged);
            }

            var firstItem = (selectedItems[0] as SettingHolder);
            bool applyImmediatelyValue = firstItem.ApplyImmediately;
            bool overrideIdentifierValue = firstItem.OverrideIdentifierWhenCopied;
            string parentIdentifierValue = firstItem.InputElementProviderSettings.ParentIdentifier;

            bool mixedValuesApplyImmediately = false;
            bool mixedValuesOverrideIdentifier = false;
            bool mixedValuesParentIdentifier = false;

            for (int i = 1; i < selectedItems.Count; i++)
            {
              var setting = selectedItems[i] as SettingHolder;

              if (!mixedValuesApplyImmediately && setting.ApplyImmediately != applyImmediatelyValue)
              {
                mixedValuesApplyImmediately = true;
              }

              if (!mixedValuesOverrideIdentifier && setting.OverrideIdentifierWhenCopied != overrideIdentifierValue)
              {
                mixedValuesOverrideIdentifier = true;
              }

              if (!mixedValuesParentIdentifier && setting.InputElementProviderSettings.ParentIdentifier != parentIdentifierValue)
              {
                mixedValuesParentIdentifier = true;
              }

              if (mixedValuesApplyImmediately && mixedValuesOverrideIdentifier && mixedValuesParentIdentifier)
              {
                break;
              }
            }

            SetToggleValue(applyImmediatelyToggle, applyImmediatelyValue, mixedValuesApplyImmediately);
            SetToggleValue(overrideIdentifierWhenCopiedToggle, overrideIdentifierValue, mixedValuesOverrideIdentifier);
            SetTextFieldValue(parentIdentifierTextField, parentIdentifierValue, mixedValuesParentIdentifier);

            return;
          }

          var selectedItem = settingsList.selectedItems.Last();

          if (selectedItem is SettingHolder settingHolder)
          {
            //var so = new SerializedObject(collection);
            //var selectedIndex = settingsList.selectedIndices.First();
            var selectedIndex = collection.Settings.IndexOf(settingHolder);

            //ConsoleLogger.Log($"{settingHolder.Setting.EditorName}: {selectedIndex}");
            //return;
            var settingsProperty = serializedObject.FindProperty(PathToListItem(selectedIndex));
            //var propertyField = selectedSettingContainer.Q<PropertyField>();
            if (propertyField == null)
            {
              propertyField = new PropertyField();
              selectedSettingContainer.Add(propertyField);
              propertyField.RegisterValueChangeCallback(OnSettingChanged);
              //serializedObject.Update();
              //serializedObject.ApplyModifiedProperties();
              propertyField.Bind(serializedObject);
            }
            //serializedObject.ApplyModifiedProperties();
            //serializedObject.Update();
            propertyField.BindProperty(settingsProperty);
            boundIndex = selectedIndex;
            selectedSettingContainer.Show(true);
          }
          else
          {
            boundIndex = -1;
            selectedSettingContainer.Show(false);
          }
        }

      }
      catch (Exception e)
      {
        ConsoleLogger.Log(e);
      }
    }

    protected void SetToggleValue(Toggle toggle, bool value, bool isMixedValue)
    {
      if (toggle == null) { return; }
      var checkMark = toggle.Q(className: "unity-toggle__checkmark");
      if (checkMark == null) { return; }

      if (isMixedValue)
      {
        if (isUnity2022OrNewer)
        {
          checkMark.AddToClassList("unity-toggle__mixed-values");
        }
        else
        {
          checkMark.SetPadding(3, false, false, false, true);
          var label = new Label("-");
          checkMark.Add(label);
          label.AddToClassList("unity-base-field__label--mixed-value");
        }

        toggle.SetValueWithoutNotify(false);
      }
      else
      {
        if (isUnity2022OrNewer)
        {
          checkMark.RemoveFromClassList("unity-toggle__mixed-values");
        }
        else
        {
          checkMark.RemoveAllChildren();
        }

        toggle.SetValueWithoutNotify(value);
      }
    }

    protected void SetTextFieldValue(TextField textField, string value, bool isMixedValue)
    {
      if (textField == null) { return; }
      if (isMixedValue)
      {
        textField.Q(className: "unity-text-field__input")?.AddToClassList("unity-base-field__label--mixed-value");
        textField.SetValueWithoutNotify("-");
      }
      else
      {
        textField.Q(className: "unity-text-field__input")?.RemoveFromClassList("unity-base-field__label--mixed-value");
        textField.SetValueWithoutNotify(value);
      }
    }

    private void OnApplyImmediatelyToggleValueChanged(ChangeEvent<bool> evt)
    {
      bool value = evt.newValue;
      for (int i = 0; i < settingsList.selectedItems.Count(); i++)
      {
        if (settingsList.selectedItems.ElementAt(i) is SettingHolder settingHolder)
        {
          settingHolder.ApplyImmediately = value;
        }
      }

      SetToggleValue(applyImmediatelyToggle, value, false);
      serializedObject.Update();
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void OnOverrideIdentifierWhenCopiedToggleValueChanged(ChangeEvent<bool> evt)
    {
      bool value = evt.newValue;
      for (int i = 0; i < settingsList.selectedItems.Count(); i++)
      {
        if (settingsList.selectedItems.ElementAt(i) is SettingHolder settingHolder)
        {
          settingHolder.OverrideIdentifierWhenCopied = value;
        }
      }

      SetToggleValue(overrideIdentifierWhenCopiedToggle, value, false);
      serializedObject.Update();
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void OnParentIdentifierTextFieldValueChanged(ChangeEvent<string> evt)
    {
      string value = evt.newValue;
      for (int i = 0; i < settingsList.selectedItems.Count(); i++)
      {
        if (settingsList.selectedItems.ElementAt(i) is SettingHolder settingHolder)
        {
          settingHolder.InputElementProviderSettings.ParentIdentifier = value;
        }
      }

      SetTextFieldValue(parentIdentifierTextField, value, false);
      serializedObject.Update();
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(serializedObject.targetObject);
    }

    private void OnSettingChanged(SerializedPropertyChangeEvent evt)
    {
      //return;
      //ConsoleLogger.Log("Setting changed");
      serializedObject.Update();
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(serializedObject.targetObject);
      if (boundIndex < 0) { return; }
      settingsList?.RefreshItem(boundIndex);
    }

    //private void BindPropertyField(VisualElement elem, SettingHolder holder, int index)
    //{
    //  var propertyField = elem.Q<PropertyField>(className: ITEM_PROPERTY_CLASS);

    //  if (propertyField == null)
    //  {
    //    propertyField = new PropertyField();
    //    propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
    //    elem.Add(propertyField);
    //    //propertyField.RegisterCallback<ChangeEvent<SettingHolder_FromClass>>(OnSettingHolderChanged);
    //  }

    //  propertyField.RegisterValueChangeCallback((evt) => OnItemPropertyChanged(elem, holder, index));

    //  propertyField.bindingPath = PathToListItem(index);
    //  propertyField.Bind(serializedObject);
    //  //propertyField.OpenPropertyFieldFoldout();

    //  var foldout = elem.Q<Foldout>();
    //  foldout?.RegisterValueChangedCallback((evt) => OpenPropertyFoldoutsInHierarchy(evt, elem));
    //}

    private void OpenPropertyFoldoutsInHierarchy(ChangeEvent<bool> evt, VisualElement elem)
    {
      if (evt.newValue == true)
      {
        var propertyFields = elem.Query<PropertyField>().ToList();
        foreach (var propertyField in propertyFields)
        {
          propertyField?.OpenPropertyFieldFoldout();
        }
      }
    }

    private void OnItemPropertyChanged(VisualElement elem, SettingHolder holder, int index)
    {
      BindListItemLabel(elem, holder, index);
    }

    private void OnSettingsListAddButtonClicked()
    {
      //var holder = new SettingHolder();

      //Undo.RecordObject(serializedObject.targetObject, "Added setting");

      //collection.Settings.Add(holder);

      //serializedObject.Update();
      //EditorUtility.SetDirty(serializedObject.targetObject);

      //RefreshItems();

      EditorUtilities.GetInstanceOfTypeFromAdvancedDropdown
        (findDerivedTypes: true, (o) => AddSetting(o), label: "", "Setting_", "", typeof(Setting));
    }

    //private void OnSettingsListAddQualitySettingButtonClicked()
    //{
    //  EditorUtilities.GetInstanceOfTypeFromAdvancedDropdown
    //    (findDerivedTypes: true, (o) => AddQualitySetting(o), label: "", "Setting", "", typeof(UnitySetting));
    //}

    // TODO simplify by making more shared code with regular add
    //private void AddQualitySetting(object value)
    //{
    //  if (!(value is UnitySetting unitySetting))
    //  {
    //    ConsoleLogger.LogWarning($"Provided value must be of type {nameof(UnitySetting)}");
    //    return;
    //  }
    //  var holder = new SettingHolder();
    //  var qualitySetting = new QualitySetting();
    //  qualitySetting.unitySetting = unitySetting;

    //  holder.Setting = qualitySetting;

    //  Undo.RecordObject(serializedObject.targetObject, "Added quality setting");

    //  collection.Settings.Add(holder);

    //  serializedObject.Update();
    //  EditorUtility.SetDirty(serializedObject.targetObject);

    //  RefreshItems();
    //}

    private void AddSetting(object value)
    {
      if (!(value is Setting setting))
      {
        ConsoleLogger.LogWarning($"Provided value must be of type {nameof(Setting)}");
        return;
      }
      var holder = new SettingHolder();
      //var qualitySetting = new QualitySetting();
      //qualitySetting.unitySetting = unitySetting;

      holder.Setting = setting;

      Undo.RecordObject(serializedObject.targetObject, "Added setting");

      collection.Settings.Add(holder);

      serializedObject.Update();
      EditorUtility.SetDirty(serializedObject.targetObject);

      RefreshItems();
    }

    private void OnSettingsListRemoveButtonClicked()
    {
      if (settingsList == null) { return; }
      var settingsCount = settingsList.itemsSource.Count;
      if (settingsCount < 1) { return; }

      var selectedIndices = settingsList.selectedIndices.ToList();
      if (selectedIndices.Count == 0)
      {
        selectedIndices.Add(settingsList.childCount - 1);
      }

      var settingHolders = new List<SettingHolder>();

      for (int i = 0; i < selectedIndices.Count; i++)
      {
        var index = selectedIndices[i];
        bool isOutOfBounds = index < 0 || index >= settingsCount;
        if (isOutOfBounds && selectedIndices.Count <= 1)
        {
          index = settingsCount - 1;
        }

        var instance = collection.Settings[index];
        if (instance != null)
        {
          settingHolders.AddIfNotContains(instance);
        }
      }

      if (settingHolders.Count > 0)
      {
        propertyField?.Unbind();
        Undo.RecordObject(serializedObject.targetObject, "Removed settings");
        foreach (var holder in settingHolders)
        {
          collection.Settings.Remove(holder);
        }
        serializedObject.Update();
        EditorUtility.SetDirty(serializedObject.targetObject);

        settingsList.ClearSelection();
        RefreshItems();
        UpdateSelectedSettingDetails();
      }
    }

    private void OnItemIndexChanged(int index1, int index2)
    {
      try
      {
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        EditorUtility.SetDirty(serializedObject.targetObject);
        // TODO refresh might not be needed!
        // OLD 2021
        //settingsList?.RefreshItems();
        //settingsList?.Rebuild();

        // TODO Enable if list breaks!
        //RefreshItems();
      }
      catch (Exception/* e*/)
      {
        // TODO Enable logging in debug mode
        //ConsoleLogger.Log(e);
        //throw;
      }
      UpdateSelectedSettingDetails();
    }

    public void RefreshItems()
    {
#if UNITY_2021_1_OR_NEWER
      try
      {
        settingsList?.RefreshItems();
      }
      catch (Exception)
      {
        // TODO Should anything happen in this case like another scheduled refresh?
        //throw;
      }
#else
      settingsList?.Refresh();
#endif
    }

    private void OnUndoPerformed()
    {
      // TODO Check if this can be removed at some point
      // Currently required because of Unity not refreshing the list upon undo
      EditorApplication.delayCall += RefreshItems;
    }

    protected void OpenInEditor()
    {
      var managerWindow = ManagerWindow_SettingsMenuCreator.ShowManagerTab_SettingsCollection();

      if (managerWindow == null) { return; }

      var asset = managerWindow.tabContents.assets.Find(i => i.displayName == "Settings Collection");
      if (asset == null) { return; }
      var controller = asset.controller;
      if (controller == null) { return; }
      if (controller is SettingsCollectionController settingsCollectionController)
      {
        settingsCollectionController.SettingsCollection = target as SettingsCollection;
      }
    }

    protected void OpenPrefab()
    {
      if (collection.UguiLayoutPrefab == null)
      {
        ConsoleLogger.LogWarning("No layout prefab referenced!");
        return;
      }

      var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
      if (currentStage != null)
      {
        EditorSceneManager.CloseScene(currentStage.scene, true);
      }

      var assetPath = AssetDatabase.GetAssetPath(collection.UguiLayoutPrefab);
      PrefabStageUtility.OpenPrefab(assetPath);
      Selection.activeObject = collection;
    }

    public static GameObject CreatePrefab(ListView list, SettingsCollection collection, bool createNewPrefabVariant)
    {
      if (collection == null) { return null; }
      var layoutPrefab = collection.UguiLayoutPrefab;
      if (layoutPrefab == null) { return null; }
      if (GameObjectExtensions.IsSceneObject(layoutPrefab))
      {
        ConsoleLogger.LogWarning("The referenced object is a scene object which " +
                                 "is not supported for the prefab creation!");
        return null;
      }

      var prefabAssetPath = AssetDatabase.GetAssetPath(layoutPrefab);
      if (prefabAssetPath.StartsWith("Packages/com.citrion") && !createNewPrefabVariant)
      {
        ConsoleLogger.LogError("Updating prefabs inside any CitrioN packages is not allowed!");
        return null;
      }

      //string assetSaveDirectory = null;
      //var collectionAssetPath = AssetDatabase.GetAssetPath(collection);
      //bool collectionInPackage = collectionAssetPath.StartsWith("Packages/com.citrion");

      //if (collectionInPackage)
      //{
      //  //assetSaveDirectory = $"Assets/Settings Menu Creator/Settings Menu";
      //}
      //else
      //{
      //  assetSaveDirectory = Path.GetDirectoryName(collectionAssetPath);
      //  assetSaveDirectory = $"{assetSaveDirectory}/UGUI/Menu";
      //  bool assetDirectoryExists = Directory.Exists(assetSaveDirectory);
      //  Directory.CreateDirectory(assetSaveDirectory);
      //}

      //var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
      //string prefabPath = prefabStage?.assetPath;
      //var activeObject = Selection.activeObject;

      //if (createNewPrefabVariant)
      //{
      //  StageUtility.GoToMainStage();
      //}

      var currentlySelected = Selection.activeObject;

      GameObject layoutVariant = createNewPrefabVariant ? CreatePrefabVariant(collection, layoutPrefab) : layoutPrefab;

      if (layoutVariant == null) { return null; }

      //var instance = PrefabUtility.InstantiatePrefab(layoutVariant) as GameObject;

      //if (instance == null) { return null; }

      var prefabStage = OpenPrefabStage(layoutVariant);
      var prefabContentsRoot = prefabStage.prefabContentsRoot;

      // TODO Should there be some recursive algorithm invoking it until there are no more new ones found?
      var actionInvokers = prefabContentsRoot.transform.GetComponentsInChildren<OnPrefabCreationActionInvoker>(true, true).ToList();

      if (actionInvokers != null)
      {
        foreach (var invoker in actionInvokers)
        {
          if (invoker == null) { continue; }
          invoker.InvokeAction();
        }
      }

      var secondActionInvokers = prefabContentsRoot.transform.GetComponentsInChildren<OnPrefabCreationActionInvoker>(true, true);

      if (secondActionInvokers != null)
      {
        foreach (var invoker in secondActionInvokers)
        {
          if (invoker != null && !actionInvokers.Contains(invoker))
          {
            invoker.InvokeAction();
          }
        }
      }

      AddAndReorderElements(list, collection, layoutVariant, selected: false, createElements: true, includeExisting: false, reorder: true);

      if (collection.AddEditModeStyleProfileListening)
      {
        var registerStyleProfileScript = prefabContentsRoot.AddOrGetComponent<RegisterStyleListenersInHierarchyInEditMode>();
        if (registerStyleProfileScript != null)
        {
          registerStyleProfileScript.GetAndRegisterStyleListenersFromHierarchy();
        }
      }
      // Check if we are updating an existing prefab
      else if (!createNewPrefabVariant)
      {
        if (prefabContentsRoot.TryGetComponent<RegisterStyleListenersInHierarchyInEditMode>(out var c))
        {
          DestroyImmediate(c);
        }
      }

      if (collection.AddApplyStyleProfileScript)
      {
        var applyStyleProfileScript = prefabContentsRoot.AddOrGetComponent<ApplyStyleProfile>();
        if (collection.StyleProfile != null && applyStyleProfileScript != null)
        {
          applyStyleProfileScript.StyleProfile = collection.StyleProfile;
          applyStyleProfileScript.ApplyProfile();
        }
      }
      else if (!createNewPrefabVariant)
      {
        if (prefabContentsRoot.TryGetComponent<ApplyStyleProfile>(out var c))
        {
          DestroyImmediate(c);
        }
      }

      RectTransform root = prefabContentsRoot != null ? prefabContentsRoot.GetComponentInChildren<RectTransform>() : null;
      ApplyStyleProfileToHierarchy(root, 0);
      ApplyStyleProfileToHierarchy(root, 2);
      //else if (collection.StyleProfile != null)
      //{
      //  if (prefabContentsRoot.TryGetComponent<ApplyStyleProfile>(out var applyStyleProfileScript))
      //  {
      //    applyStyleProfileScript.StyleProfile = collection.StyleProfile;
      //    applyStyleProfileScript.ApplyProfile();
      //  }
      //}



      //PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);

      //EditorUtilities.PingObject(layoutVariant);
      //DestroyImmediate(instance);

      //if (createNewPrefabVariant)
      //{
      //  if (!string.IsNullOrEmpty(prefabPath))
      //  {
      //    PrefabStageUtility.OpenPrefab(prefabPath);
      //  }
      //  Selection.activeObject = activeObject;
      //}


      if (prefabContentsRoot != null)
      {
        PrefabUtility.SaveAsPrefabAsset(prefabContentsRoot, prefabStage.assetPath);
      }
      prefabStage.ClearDirtiness();

      ScheduleUtility.InvokeDelayedByFrames(() => Selection.activeObject = currentlySelected, 2);

      return layoutVariant;
      //PrefabUtility.ApplyAddedGameObjects(addedGameObjects.ToArray(), assetPath, InteractionMode.AutomatedAction);
    }

    protected static GameObject CreatePrefabVariant(SettingsCollection collection, GameObject source)
    {
      if (collection == null || source == null) { return null; }

      string prefabAssetPath = AssetDatabase.GetAssetPath(source);
      string collectionAssetPath = AssetDatabase.GetAssetPath(collection);
      var collectionDirectory = Path.GetDirectoryName(collectionAssetPath);
      var prefabFileName = Path.GetFileNameWithoutExtension(prefabAssetPath);
      string suffix = prefabFileName.Contains("WithInputElements") ? "" : "_WithInputElements";
      string extension = Path.GetExtension(prefabAssetPath);

      bool prefabInPackage = prefabAssetPath.StartsWith("Packages/com.citrion");
      bool collectionInPackage = collectionAssetPath.StartsWith("Packages/com.citrion");

      string saveDirectoryPath = string.Empty;

      // Check if both the collection and the prefab are in a package.
      // Place the new prefab inside the assets folder
      if (prefabInPackage && collectionInPackage)
      {
        saveDirectoryPath = "Assets/Settings Menu Creator/Settings Menu/Settings/UGUI/Menu";
      }
      else if (!prefabInPackage)
      {
        // If the prefab is not in the package save the variant in the same folder as the source
        saveDirectoryPath = Path.GetDirectoryName(prefabAssetPath);
      }
      else if (!collectionInPackage)
      {
        // If only the collection is not in the package then place it
        // relative to the collection just like when generating resources for the menu
        saveDirectoryPath = $"{Path.GetDirectoryName(collectionAssetPath)}/UGUI/Menu";
      }

      bool assetDirectoryExists = Directory.Exists(saveDirectoryPath);
      if (!assetDirectoryExists)
      {
        Directory.CreateDirectory(saveDirectoryPath);
      }

      var newFolderPath = AssetDatabase.GenerateUniqueAssetPath($"{saveDirectoryPath}/{prefabFileName}{suffix}{extension}");

      GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
      //var prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, $"{folderPath}/{source.name}_Variant.prefab");
      var prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, newFolderPath);
      GameObject.DestroyImmediate(instance);
      EditorUtility.SetDirty(prefabVariant);
      EditorUtilities.PingObject(prefabVariant);
      return prefabVariant;
    }

    //protected static void AddElements(ListView list, SettingsCollection collection, GameObject prefab, bool selected, bool includeExisting, bool reorder)
    //{
    //  if (collection == null || prefab == null) { return; }

    //  //var currentlySelected = Selection.activeObject;

    //  //var layoutPrefab = collection.UguiLayoutPrefab;
    //  //if (layoutPrefab == null) { return; }

    //  var prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
    //  if (prefabAssetPath.StartsWith("Packages/com.citrion"))
    //  {
    //    ConsoleLogger.LogError("Updating prefabs inside any CitrioN packages is not allowed!");
    //    return;
    //  }

    //  //var instance = PrefabUtility.InstantiatePrefab(layoutPrefab) as GameObject;
    //  //if (instance == null) { return; }

    //  AddElements(list, collection, prefab, selected, includeExisting, reorder);

    //  //PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
    //  //DestroyImmediate(instance);

    //  //PrefabUtility.SavePrefabAsset(layoutPrefab);
    //  //var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
    //  //if (AssetDatabase.GetAssetPath(layoutPrefab) == prefabStage?.assetPath)
    //  //{
    //  //  prefabStage.ClearDirtiness();
    //  //}
    //  //ScheduleUtility.InvokeDelayedByFrames(() => Selection.activeObject = currentlySelected, 2);
    //}

    public static PrefabStage OpenPrefabStage(GameObject prefab)
    {
      if (prefab == null) { return null; }
      var assetPath = AssetDatabase.GetAssetPath(prefab);
      var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
      // Check if the prefab is already open
      if (!(prefabStage != null && prefabStage.assetPath == assetPath))
      {
        if (prefabStage != null)
        {
          PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
        }
        // TODO Save current prefab stage
        prefabStage = PrefabStageUtility.OpenPrefab(assetPath);
      }
      return prefabStage;
    }

    //protected static void AddElements(ListView list, SettingsCollection collection, GameObject prefab, bool selected, bool includeExisting, bool reorder)
    //{
    //  if (prefab == null) { return; }

    //  //var settingObjects = prefabContentsRoot.transform.GetComponentsInChildren<SettingObject>(true, true).ToList();

    //  //var identifierOrder = settingObjects
    //  //    //.Select((item, index) => new { item.Identifier, Index = index })
    //  //    .ToDictionary(x => x.Identifier, x => x);

    //  //for (int i = 0; i < collection.Settings.Count; i++)
    //  //{
    //  //  var setting = collection.Settings[i];
    //  //  var parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
    //  //}

    //  //var list = new List<GameObject>();
    //  if (/*prefabContentsRoot != null &&*/ collection != null)
    //  {
    //    //var originalProviderCollection = collection.InputElementProviders_UGUI;

    //    //if (collection.InputElementProviders_UGUI == null && collection.ProviderCollectionOverride == null)
    //    //{
    //    //  ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UGUI)} reference assigned!");
    //    //  return list;
    //    //}

    //    //if (collection.ProviderCollectionOverride != null)
    //    //{
    //    //  collection.InputElementProviders_UGUI = collection.ProviderCollectionOverride;
    //    //}

    //    //List<SettingHolder> settings = null;
    //    //if (selected)
    //    //{
    //    //  settings = settingsList.selectedItems?.Cast<SettingHolder>().ToList();
    //    //}
    //    //else
    //    //{
    //    //  settings = collection.Settings;
    //    //}

    //    AddAndReorderElements(list, collection, prefab, selected, createElements: true, includeExisting, reorder);

    //    #region Old
    //    //Dictionary<string, List<string>> parentChildrenMapping = new Dictionary<string, List<string>>();
    //    //var settingsMapping = collection.Settings.ToDictionary(x => x.Identifier, x => x);

    //    //foreach (var setting in collection.Settings)
    //    //{
    //    //  string parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
    //    //  if (!parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
    //    //  {
    //    //    children = new List<string>() { setting.Identifier };
    //    //    parentChildrenMapping.Add(parentIdentifier, children);
    //    //  }
    //    //  else
    //    //  {
    //    //    children.Add(setting.Identifier);
    //    //  }
    //    //}

    //    //if (settings != null)
    //    //{
    //    //  bool parented = false;
    //    //  bool wasCreated = false;
    //    //  bool createElements = true;
    //    //  SettingObject currentParent = null;
    //    //  for (int i = 0; i < settings.Count; i++)
    //    //  {
    //    //    var s = settings[i];
    //    //    if (s == null) { continue; }
    //    //    wasCreated = false;
    //    //    currentParent = null;
    //    //    var elem = s.FindElement_UGUI(root, collection);

    //    //    if (elem == null && createElements)
    //    //    {
    //    //      elem = s.CreateElement_UGUI(root, collection);
    //    //      currentParent = elem.parent.GetComponent<SettingObject>();

    //    //      // Unparent element so we can ensure it's not interfering with the index checking.
    //    //      // We will reparent it later.
    //    //      elem.SetParent(null);
    //    //      wasCreated = true;
    //    //    }

    //    //    parented = false;
    //    //    if (elem != null/* && (includeExisting || !wasCreated)*/)
    //    //    {
    //    //      //if (InitializeInputElements)
    //    //      {
    //    //        s.InitializeElement_UGUI(elem, collection, initialize: false);
    //    //      }
    //    //      list.Add(elem.gameObject);

    //    //      var parentIdentifier = s.InputElementProviderSettings.ParentIdentifier;
    //    //      if (parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
    //    //      {
    //    //        if (children != null)
    //    //        {
    //    //          // Get current child index
    //    //          var childIndex = children.IndexOf(s.Identifier);
    //    //          // Check if the child is not the first child
    //    //          if (childIndex != -1 && childIndex >= 0 && childIndex < children.Count)
    //    //          {
    //    //            if (childIndex > 0)
    //    //            {
    //    //              // Iterate over the children before this one and try parenting it after the first one parented correctly
    //    //              for (int j = childIndex - 1; j >= 0; j--)
    //    //              {
    //    //                var indexBefore = j;

    //    //                //var indexBefore = childIndex - 1;
    //    //                var settingIdentifierBefore = children[indexBefore];
    //    //                if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
    //    //                {
    //    //                  var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //                  if (siblingElement != null)
    //    //                  {
    //    //                    var siblingParentObj = siblingElement.parent;
    //    //                    if (siblingParentObj != null)
    //    //                    {
    //    //                      var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //                      bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //                      if (isParentedCorrectly)
    //    //                      {
    //    //                        var siblingIndex = siblingElement.GetSiblingIndex();
    //    //                        elem.SetParent(parentSettingObject.transform);
    //    //                        elem.SetSiblingIndex(siblingIndex + 1);
    //    //                        parented = true;
    //    //                        break;
    //    //                      }
    //    //                    }
    //    //                  }
    //    //                }
    //    //              }
    //    //            }

    //    //            // Check if parenting was successful
    //    //            if (parented) { continue; }

    //    //            // Check if the child is the last one.
    //    //            // Parent it back to its original parent if it was just created.
    //    //            //if (childIndex == children.Count - 1)
    //    //            //{
    //    //            //  if (currentParent != null)
    //    //            //  {
    //    //            //    elem.SetParent(currentParent.transform);
    //    //            //  }
    //    //            //  continue;
    //    //            //}

    //    //            // Iterate over the children after this one and try parenting it before the first one parented correctly
    //    //            for (int j = childIndex + 1; j < children.Count; j++)
    //    //            {
    //    //              var indexAfter = j;

    //    //              var settingIdentifierAfter = children[indexAfter];
    //    //              if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingBefore))
    //    //              {
    //    //                var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //                if (siblingElement != null)
    //    //                {
    //    //                  var siblingParentObj = siblingElement.parent;
    //    //                  if (siblingParentObj != null)
    //    //                  {
    //    //                    var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //                    bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //                    if (isParentedCorrectly)
    //    //                    {
    //    //                      var siblingIndex = siblingElement.GetSiblingIndex();
    //    //                      elem.SetParent(parentSettingObject.transform);
    //    //                      elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
    //    //                      parented = true;
    //    //                      break;
    //    //                    }
    //    //                  }
    //    //                }
    //    //              }
    //    //            }

    //    //            if (parented) { continue; }

    //    //            // Check if there is a valid parent
    //    //            if (currentParent != null)
    //    //            {
    //    //              elem.SetParent(currentParent.transform);
    //    //              continue;
    //    //            }

    //    //            var settingIdentifiers = root.GetComponentsInChildren<SettingObject>(true, true);
    //    //            var parentElement = settingIdentifiers?.Find(si => si.Identifier == parentIdentifier);
    //    //            if (parentElement != null)
    //    //            {
    //    //              Transform parentTransform = parentElement.GetContentParent();
    //    //              elem.SetParent(parentTransform != null ? parentTransform : root, false);
    //    //            }

    //    //            //ConsoleLogger.LogWarning($"Unable to parent {s.Identifier}");
    //    //          }
    //    //          // Setting is the first element of the list using that parent


    //    //          // Remove this later
    //    //          //else if (childIndex == 0)
    //    //          //{
    //    //          //  //// Check if the setting has no siblings
    //    //          //  //if (children.Count == 1)
    //    //          //  //{
    //    //          //  //  //elem.SetSiblingIndex(0);
    //    //          //  //  continue;
    //    //          //  //}

    //    //          //  // Iterate over the children before this one and try parenting it after the first one parented correctly
    //    //          //  for (int j = childIndex - 1; j >= 0; j--)
    //    //          //  {
    //    //          //    var indexBefore = j;

    //    //          //    //var indexBefore = childIndex - 1;
    //    //          //    var settingIdentifierBefore = children[indexBefore];
    //    //          //    if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
    //    //          //    {
    //    //          //      var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //          //      if (siblingElement != null)
    //    //          //      {
    //    //          //        var siblingParentObj = siblingElement.parent;
    //    //          //        if (siblingParentObj != null)
    //    //          //        {
    //    //          //          var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //          //          bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //          //          if (isParentedCorrectly)
    //    //          //          {
    //    //          //            var siblingIndex = siblingElement.GetSiblingIndex();
    //    //          //            elem.SetParent(parentSettingObject.transform);
    //    //          //            elem.SetSiblingIndex(siblingIndex + 1);
    //    //          //            parented = true;
    //    //          //            break;
    //    //          //          }
    //    //          //        }
    //    //          //      }
    //    //          //    }
    //    //          //  }

    //    //          //  if (parented)
    //    //          //  {
    //    //          //    continue;
    //    //          //  }

    //    //          //  // Check if the child is the last one.
    //    //          //  // Parent it back to its original parent if it was just created.
    //    //          //  if (childIndex == children.Count - 1)
    //    //          //  {
    //    //          //    if (currentParent != null)
    //    //          //    {
    //    //          //      elem.SetParent(currentParent.transform);
    //    //          //    }
    //    //          //    continue;
    //    //          //  }

    //    //          //  // Iterate over the children after this one and try parenting it before the first one parented correctly
    //    //          //  for (int j = childIndex + 1; j < children.Count; j++)
    //    //          //  {
    //    //          //    var indexAfter = j;

    //    //          //    var settingIdentifierAfter = children[indexAfter];
    //    //          //    if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingBefore))
    //    //          //    {
    //    //          //      var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //          //      if (siblingElement != null)
    //    //          //      {
    //    //          //        var siblingParentObj = siblingElement.parent;
    //    //          //        if (siblingParentObj != null)
    //    //          //        {
    //    //          //          var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //          //          bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //          //          if (isParentedCorrectly)
    //    //          //          {
    //    //          //            var siblingIndex = siblingElement.GetSiblingIndex();
    //    //          //            elem.SetParent(parentSettingObject.transform);
    //    //          //            elem.SetSiblingIndex(siblingIndex - 1);
    //    //          //            parented = true;
    //    //          //            break;
    //    //          //          }
    //    //          //        }
    //    //          //      }
    //    //          //    }
    //    //          //  }


    //    //          //  //if (children.Count > childIndex + 1)
    //    //          //  //{
    //    //          //  //  for (int j = childIndex + 1; j >= 0; j--)
    //    //          //  //  {
    //    //          //  //    var indexAfter = j;
    //    //          //  //    //var indexAfter = childIndex + 1;
    //    //          //  //    if (children.Count > indexAfter)
    //    //          //  //    {
    //    //          //  //      var settingIdentifierAfter = children[indexAfter];
    //    //          //  //      if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingAfter))
    //    //          //  //      {
    //    //          //  //        var siblingElement = settingAfter?.FindElement_UGUI(root, collection);
    //    //          //  //        if (siblingElement != null)
    //    //          //  //        {
    //    //          //  //          var siblingParentObj = siblingElement.parent;
    //    //          //  //          if (siblingParentObj != null)
    //    //          //  //          {
    //    //          //  //            var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //          //  //            bool isParentedCorrectly = parentSettingObject.Identifier == settingAfter.InputElementProviderSettings.ParentIdentifier;
    //    //          //  //            if (isParentedCorrectly)
    //    //          //  //            {
    //    //          //  //              var siblingIndex = siblingElement.GetSiblingIndex();
    //    //          //  //              elem.SetParent(parentSettingObject.transform);
    //    //          //  //              elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
    //    //          //  //              parented = true;
    //    //          //  //              break;
    //    //          //  //            }
    //    //          //  //          }
    //    //          //  //        }
    //    //          //  //      }
    //    //          //  //    }
    //    //          //  //  }
    //    //          //  //}
    //    //          //}







    //    //          //var childIndex = children.IndexOf(s.Identifier);
    //    //          //if (childIndex != -1 && childIndex > 0)
    //    //          //{
    //    //          //  var indexBefore = childIndex - 1;
    //    //          //  var settingIdentifierBefore = children[indexBefore];
    //    //          //  if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
    //    //          //  {
    //    //          //    var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //          //    if (siblingElement != null)
    //    //          //    {
    //    //          //      var siblingParentObj = siblingElement.parent;
    //    //          //      if (siblingParentObj != null)
    //    //          //      {
    //    //          //        var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //          //        bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //          //        if (isParentedCorrectly)
    //    //          //        {
    //    //          //          var siblingIndex = siblingElement.GetSiblingIndex();
    //    //          //          elem.SetSiblingIndex(siblingIndex + 1);
    //    //          //        }
    //    //          //      }
    //    //          //    }
    //    //          //  }
    //    //          //}
    //    //          // Setting is the first element of the list using that parent
    //    //          //else if (childIndex == 0)
    //    //          //{
    //    //          //  // Check if the setting has no siblings
    //    //          //  if (children.Count == 1)
    //    //          //  {
    //    //          //    elem.SetSiblingIndex(0);
    //    //          //  }
    //    //          //  else
    //    //          //  {
    //    //          //    var indexAfter = childIndex + 1;
    //    //          //    if (children.Count > indexAfter)
    //    //          //    {
    //    //          //      var settingIdentifierAfter = children[indexAfter];
    //    //          //      if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingAfter))
    //    //          //      {
    //    //          //        var siblingElement = settingAfter?.FindElement_UGUI(root, collection);
    //    //          //        if (siblingElement != null)
    //    //          //        {
    //    //          //          var siblingParentObj = siblingElement.parent;
    //    //          //          if (siblingParentObj != null)
    //    //          //          {
    //    //          //            var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //          //            bool isParentedCorrectly = parentSettingObject.Identifier == settingAfter.InputElementProviderSettings.ParentIdentifier;
    //    //          //            if (isParentedCorrectly)
    //    //          //            {
    //    //          //              var siblingIndex = siblingElement.GetSiblingIndex();
    //    //          //              elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
    //    //          //            }
    //    //          //          }
    //    //          //        }
    //    //          //      }
    //    //          //    }
    //    //          //    //else
    //    //          //    //{
    //    //          //    //  elem.SetSiblingIndex(childIndex);
    //    //          //    //}
    //    //          //  }
    //    //          //}
    //    //        }
    //    //      }
    //    //    }
    //    //    else
    //    //    {
    //    //      ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
    //    //                   $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
    //    //    }

    //    //    //if (parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
    //    //    //{
    //    //    //  if (children != null)
    //    //    //  {
    //    //    //    var childIndex = children.IndexOf(s.Identifier);
    //    //    //    if (childIndex != -1 && childIndex > 0)
    //    //    //    {
    //    //    //      for (int j = childIndex - 1; j >= 0; j--)
    //    //    //      {
    //    //    //        var indexBefore = j;

    //    //    //        //var indexBefore = childIndex - 1;
    //    //    //        var settingIdentifierBefore = children[indexBefore];
    //    //    //        if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
    //    //    //        {
    //    //    //          var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //    //    //          if (siblingElement != null)
    //    //    //          {
    //    //    //            var siblingParentObj = siblingElement.parent;
    //    //    //            if (siblingParentObj != null)
    //    //    //            {
    //    //    //              var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //    //              bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //    //    //              if (isParentedCorrectly)
    //    //    //              {
    //    //    //                var siblingIndex = siblingElement.GetSiblingIndex();
    //    //    //                elem.SetParent(parentSettingObject.transform);
    //    //    //                elem.SetSiblingIndex(siblingIndex + 1);
    //    //    //                parented = true;
    //    //    //                break;
    //    //    //              }
    //    //    //            }
    //    //    //          }
    //    //    //        }
    //    //    //      }
    //    //    //    }
    //    //    //    // Setting is the first element of the list using that parent
    //    //    //    else if (childIndex == 0)
    //    //    //    {
    //    //    //      // Check if the setting has no siblings
    //    //    //      if (children.Count == 1)
    //    //    //      {
    //    //    //        elem.SetSiblingIndex(0);
    //    //    //      }
    //    //    //      else if (children.Count > childIndex + 1)
    //    //    //      {
    //    //    //        for (int j = childIndex + 1; j >= 0; j--)
    //    //    //        {
    //    //    //          var indexAfter = j;
    //    //    //          //var indexAfter = childIndex + 1;
    //    //    //          if (children.Count > indexAfter)
    //    //    //          {
    //    //    //            var settingIdentifierAfter = children[indexAfter];
    //    //    //            if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingAfter))
    //    //    //            {
    //    //    //              var siblingElement = settingAfter?.FindElement_UGUI(root, collection);
    //    //    //              if (siblingElement != null)
    //    //    //              {
    //    //    //                var siblingParentObj = siblingElement.parent;
    //    //    //                if (siblingParentObj != null)
    //    //    //                {
    //    //    //                  var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //    //    //                  bool isParentedCorrectly = parentSettingObject.Identifier == settingAfter.InputElementProviderSettings.ParentIdentifier;
    //    //    //                  if (isParentedCorrectly)
    //    //    //                  {
    //    //    //                    var siblingIndex = siblingElement.GetSiblingIndex();
    //    //    //                    elem.SetParent(parentSettingObject.transform);
    //    //    //                    elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
    //    //    //                    parented = true;
    //    //    //                    break;
    //    //    //                  }
    //    //    //                }
    //    //    //              }
    //    //    //            }
    //    //    //          }
    //    //    //        }
    //    //    //        //else
    //    //    //        //{
    //    //    //        //  elem.SetSiblingIndex(childIndex);
    //    //    //        //}
    //    //    //      }
    //    //    //    }
    //    //    //  }
    //    //    //}
    //    //  }
    //    //} 
    //    #endregion

    //    //collection.InputElementProviders_UGUI = originalProviderCollection;
    //  }

    //  //PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
    //  //prefabStage.ClearDirtiness();
    //  //return list;
    //}

    protected void SelectElementsInPrefab(bool selected)
    {
      if (collection == null) { return; }
      var prefab = collection.UguiLayoutPrefab;
      if (prefab == null) { return; }

      PrefabUtility.SavePrefabAsset(prefab);
      var prefabStage = OpenPrefabStage(prefab);
      if (prefabStage == null) { return; }

      var prefabContentsRoot = prefabStage.prefabContentsRoot;
      if (prefabContentsRoot == null) { return; }
      var root = prefabContentsRoot.GetComponentInChildren<RectTransform>();
      if (root == null) { return; }

      List<SettingHolder> settings;
      if (selected)
      {
        settings = settingsList?.selectedItems?.Cast<SettingHolder>()?.ToList();
      }
      else
      {
        settings = collection.Settings;
      }

      if (settings == null || settings.Count < 1) { return; }

      var objects = new List<UnityEngine.Object>();
      for (int i = 0; i < settings.Count; i++)
      {
        var s = settings[i];
        if (s == null) { continue; }

        var elem = s.FindElement_UGUI(root, collection);
        if (elem == null) { continue; }

        objects.Add(elem.gameObject);
      }

      Selection.objects = objects.ToArray();
    }

    protected void ApplyHierarchyFilter(string filter)
    {
      if (collection == null) { return; }
      var prefab = collection.UguiLayoutPrefab;
      if (prefab == null) { return; }

      PrefabUtility.SavePrefabAsset(prefab);
      var prefabStage = OpenPrefabStage(prefab);
      if (prefabStage == null) { return; }

      EditorWindow hierarchyWindow = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow"));

      // Use reflection to access the SearchableEditorWindow's private method
      var searchableWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SearchableEditorWindow");
      var setSearchFilterMethod = searchableWindowType?.GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);

      // Apply the search filter "t:RectTransform"
      if (setSearchFilterMethod != null)
      {
        setSearchFilterMethod.Invoke(hierarchyWindow, new object[] { filter, null, true, true });
      }
    }

    protected static bool CanModifyPrefab(GameObject prefab)
    {
      var prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
      if (prefabAssetPath.StartsWith("Packages/com.citrion"))
      {
        ConsoleLogger.LogError("Updating prefabs inside any CitrioN packages is not allowed!");
        return false;
      }
      return true;
    }

    protected void AddRemoveStyleProfileScripts(GameObject prefab, bool add)
    {
      if (prefab == null) { return; }

      if (!CanModifyPrefab(prefab)) { return; }

      var prefabStage = OpenPrefabStage(prefab);
      var prefabContentsRoot = prefabStage.prefabContentsRoot;

      if (add)
      {
        var registerStyleProfileScript = prefabContentsRoot.AddOrGetComponent<RegisterStyleListenersInHierarchyInEditMode>();
        registerStyleProfileScript.GetAndRegisterStyleListenersFromHierarchy();

        var applyStyleProfileScript = prefabContentsRoot.AddOrGetComponent<ApplyStyleProfile>();
        if (collection.StyleProfile != null && applyStyleProfileScript != null)
        {
          applyStyleProfileScript.StyleProfile = collection.StyleProfile;
          applyStyleProfileScript.ApplyProfile();
        }
      }
      else
      {
        if (prefabContentsRoot.TryGetComponent<RegisterStyleListenersInHierarchyInEditMode>(out var c1))
        {
          DestroyImmediate(c1);
        }

        if (prefabContentsRoot.TryGetComponent<ApplyStyleProfile>(out var c2))
        {
          DestroyImmediate(c2);
        }
      }
    }

    protected static bool AddAndReorderElements(ListView list, SettingsCollection collection, GameObject prefab, bool selected, bool createElements, bool includeExisting, bool reorder = true)
    {
      if (prefab == null || collection == null || collection.Settings == null) { return false; }

      if (!CanModifyPrefab(prefab)) { return false; }

      PrefabUtility.SavePrefabAsset(prefab);
      var prefabStage = OpenPrefabStage(prefab);
      if (prefabStage == null) { return false; }

      var prefabContentsRoot = prefabStage.prefabContentsRoot;
      if (prefabContentsRoot == null) { return false; }
      var root = prefabContentsRoot.GetComponentInChildren<RectTransform>();
      if (root == null) { return false; }

      var originalProviderCollection = collection.InputElementProviders_UGUI;

      if (createElements)
      {
        if (collection.InputElementProviders_UGUI == null && collection.ProviderCollectionOverride == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UGUI)} reference assigned!");
          return false;
        }

        if (collection.ProviderCollectionOverride != null)
        {
          collection.InputElementProviders_UGUI = collection.ProviderCollectionOverride;
        }
      }

      List<SettingHolder> settings = null;
      if (selected)
      {
        settings = list?.selectedItems?.Cast<SettingHolder>()?.ToList();
      }
      else
      {
        settings = collection.Settings;
      }

      if (settings != null)
      {
        Dictionary<string, List<string>> parentChildrenMapping = new Dictionary<string, List<string>>();
        var settingsMapping = collection.Settings.ToDictionary(x => x.Identifier, x => x);

        foreach (var setting in collection.Settings)
        {
          if (setting == null) { continue; }
          string parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
          if (string.IsNullOrEmpty(parentIdentifier)) { continue; }
          if (!parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
          {
            children = new List<string>() { setting.Identifier };
            parentChildrenMapping.Add(parentIdentifier, children);
          }
          else
          {
            children.Add(setting.Identifier);
          }
        }

        if (settings == null || settings.Count == 0) { return false; }

        bool parented = false;
        bool wasCreated = false;
        SettingObject currentParent = null;

        for (int i = 0; i < settings.Count; i++)
        {
          var s = settings[i];
          if (s == null) { continue; }

          wasCreated = false;
          currentParent = null;

          var elem = s.FindElement_UGUI(root, collection);

          if (elem == null && createElements)
          {
            elem = s.CreateElement_UGUI(root, collection);
            wasCreated = true;
          }

          if (elem != null)
          {
            //if (InitializeInputElements)
            {
              s.InitializeElement_UGUI(elem, collection, initialize: true);
            }
          }

          //bool isUnity2022 = true;
#if !UNITY_2022_1_OR_NEWER
          //isUnity2022 = false;
          //reorder = false;
#endif

          if (elem != null && reorder && (wasCreated || includeExisting))
          {
            var parentIdentifier = s.InputElementProviderSettings?.ParentIdentifier;
            if (parentIdentifier == null) { continue; }

            parented = false;

            // Unparent element so we can ensure it's not interfering with the index checking.
            // We will reparent it later.
            if (elem != null && elem.parent != null)
            {
              currentParent = elem.parent.GetComponent<SettingObject>();
            }

            try
            {
              elem.SetParent(null);
            }
            catch (Exception)
            {
#if !UNITY_2022_1_OR_NEWER
              ConsoleLogger.LogWarning("Reordering objects inside a nested prefab in which they were not originally added in is not possible in Unity 2021. " +
                                       "To use this functionality please use Unity 2022 or later.");
#else
              // TODO
#endif
            }

            if (parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
            {
              if (children != null)
              {
                // Get current child index
                var childIndex = children.IndexOf(s.Identifier);

                if (childIndex != -1 && childIndex >= 0 && childIndex < children.Count)
                {
                  if (childIndex > 0)
                  {
                    // Iterate over the children before this one and try parenting it after the first one parented correctly
                    for (int j = childIndex - 1; j >= 0; j--)
                    {
                      var indexBefore = j;

                      //var indexBefore = childIndex - 1;
                      if (indexBefore >= children.Count || indexBefore < 0) { continue; }
                      var settingIdentifierBefore = children[indexBefore];
                      if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
                      {
                        var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
                        if (siblingElement != null)
                        {
                          var siblingParentObj = siblingElement.parent;
                          if (siblingParentObj != null)
                          {
                            var parentSettingObject = siblingParentObj.GetComponentInParent<SettingObject>();

                            if (parentSettingObject == null) { continue; }

                            bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
                            if (isParentedCorrectly)
                            {
                              var siblingIndex = siblingElement.GetSiblingIndex();
                              if (siblingElement.parent != null)
                              {
                                elem.SetParent(siblingElement.parent.transform);
                              }
                              try
                              {
                                elem.SetSiblingIndex(siblingIndex + 1);
                              }
                              catch (Exception)
                              {
#if !UNITY_2022_1_OR_NEWER
                                ConsoleLogger.LogWarning("Reordering objects inside a nested prefab in which they were not originally added in is not possible in Unity 2021. " +
                                                         "To use this functionality please use Unity 2022 or later.");
#else
                                // TODO
#endif
                              }
                              parented = true;
                              break;
                            }
                          }
                        }
                      }
                    }
                  }

                  // Check if parenting was successful
                  if (parented) { continue; }

                  // Check if the child is the last one.
                  // Parent it back to its original parent if it was just created.
                  //if (childIndex == children.Count - 1)
                  //{
                  //  if (currentParent != null)
                  //  {
                  //    elem.SetParent(currentParent.transform);
                  //  }
                  //  continue;
                  //}

                  // Iterate over the children after this one and try parenting it before the first one parented correctly
                  for (int j = childIndex + 1; j < children.Count; j++)
                  {
                    var indexAfter = j;

                    if (indexAfter >= children.Count || indexAfter < 0) { continue; }
                    var settingIdentifierAfter = children[indexAfter];
                    if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingBefore))
                    {
                      var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
                      if (siblingElement != null)
                      {
                        var siblingParentObj = siblingElement.parent;
                        if (siblingParentObj != null)
                        {
                          var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();

                          if (parentSettingObject == null) { continue; }

                          bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
                          if (isParentedCorrectly)
                          {
                            var siblingIndex = siblingElement.GetSiblingIndex();
                            if (siblingElement.parent != null)
                            {
                              elem.SetParent(siblingElement.parent.transform);
                            }

                            try
                            {
                              elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
                            }
                            catch (Exception)
                            {
#if !UNITY_2022_1_OR_NEWER
                              ConsoleLogger.LogWarning("Reordering objects inside a nested prefab in which they were not originally added in is not possible in Unity 2021. " +
                                                       "To use this functionality please use Unity 2022 or later.");
#else
                              // TODO
#endif
                            }
                            parented = true;
                            break;
                          }
                        }
                      }
                    }
                  }

                  if (parented) { continue; }

                  // Check if there is a valid parent
                  //if (currentParent != null)
                  //{
                  //  elem.SetParent(currentParent.transform);
                  //  continue;
                  //}

                  var settingIdentifiers = root.GetComponentsInChildren<SettingObject>(true, true);
                  var parentElement = settingIdentifiers?.Find(si => si.Identifier == parentIdentifier);
                  if (parentElement != null)
                  {
                    Transform parentTransform = parentElement.GetContentParent();
                    if (parentTransform != null)
                    {
                      elem.SetParent(parentTransform, false);
                      continue;
                    }
                  }

                  //DestroyImmediate(elem.gameObject);
                  //ConsoleLogger.LogWarning($"Unable to parent {s.Identifier}");
                }
              }
            }
          }
          //else
          //{
          //  ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
          //               $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
          //}
        }
      }

      collection.InputElementProviders_UGUI = originalProviderCollection;
      PrefabUtility.SaveAsPrefabAsset(prefabContentsRoot, prefabStage.assetPath);
      prefabStage.ClearDirtiness();

      ApplyStyleProfileToHierarchy(root, 0);
      ApplyStyleProfileToHierarchy(root, 2);

      return true;
    }

    protected GameObject MatchSettingsOrderToElementOrder()
    {
      if (collection == null || collection.Settings == null) { return null; }
      if (collection.UguiLayoutPrefab == null) { return null; }

      var prefab = collection.UguiLayoutPrefab;
      var prefabAssetPath = AssetDatabase.GetAssetPath(prefab);
      bool prefabInPackage = prefabAssetPath.StartsWith("Packages/com.citrion");
      GameObject instance = null;
      PrefabStage prefabStage = null;
      GameObject prefabContentsRoot = null;

      if (prefabInPackage)
      {
        instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null) { return null; }
        prefabContentsRoot = instance;
      }
      else
      {
        prefabStage = OpenPrefabStage(prefab);
        if (prefabStage == null) { return null; }
        prefabContentsRoot = prefabStage.prefabContentsRoot;
      }

      if (prefabContentsRoot == null) { return null; }

      var settingObjects = prefabContentsRoot.transform.GetComponentsInChildren<SettingObject>(true, true).ToList();

      var identifierOrder = settingObjects
          .Select((item, index) => new { item.Identifier, Index = index })
          .ToDictionary(x => x.Identifier, x => x.Index);

      //collection.Settings.Sort((x, y) => identifierOrder[x.Identifier].CompareTo(identifierOrder[y.Identifier]));

      collection.Settings.Sort((x, y) =>
      {
        int indexX = identifierOrder.ContainsKey(x.Identifier) ? identifierOrder[x.Identifier] : int.MaxValue;
        int indexY = identifierOrder.ContainsKey(y.Identifier) ? identifierOrder[y.Identifier] : int.MaxValue;

        return indexX.CompareTo(indexY);
      });

      RefreshItems();
      UpdateSelectedSettingDetails();

      //PrefabUtility.SaveAsPrefabAsset(prefabContentsRoot, prefabStage.assetPath);
      //prefabStage.ClearDirtiness();

      //PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);

      if (prefabInPackage)
      {
        DestroyImmediate(instance);
      }

      //EditorUtilities.PingObject(prefab);
      //Selection.activeObject = currentlySelected;
      return prefab;
    }

    protected void MatchElementsOrderToListOrder(bool selected, bool includeExisting)
    {
      AddAndReorderElements(settingsList, collection, collection?.UguiLayoutPrefab, selected, createElements: false, includeExisting);
      return;

      //if (collection?.Settings == null) { return; }

      //var prefab = collection.UguiLayoutPrefab;
      //var prefabAssetPath = AssetDatabase.GetAssetPath(prefab);

      //if (prefabAssetPath.StartsWith("Packages/com.citrion"))
      //{
      //  ConsoleLogger.LogError("Can't use prefabs inside any CitrioN packages.");
      //  return;
      //}

      //var currentlySelected = Selection.activeObject;
      //var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
      //bool isOpenInPrefabStage = false;
      //if (prefabStage != null && prefabStage.assetPath == prefabAssetPath)
      //{
      //  isOpenInPrefabStage = true;
      //  GameObject prefabRoot = prefabStage.prefabContentsRoot;
      //  //PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabStage.assetPath);
      //  //StageUtility.GoBackToPreviousStage();
      //  //if (prefabStage.scene.isDirty)
      //  //{
      //  //  prefabStage.ClearDirtiness();
      //  //}
      //  //prefabStage.ClearDirtiness();
      //  MatchElementsToList();
      //  //ScheduleUtility.InvokeDelayedByFrames(() => MatchElementsToList(prefab), 1);
      //}
      //else
      //{
      //  MatchElementsToList();
      //}

      //Selection.activeObject = currentlySelected;
      //return;
    }

    //private void MatchElementsToList()
    //{
    //  var prefab = collection?.UguiLayoutPrefab;
    //  if (prefab == null) { return; }
    //  //var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    //  //if (instance == null) { return; }
    //  var assetPath = AssetDatabase.GetAssetPath(prefab);
    //  var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
    //  // Check if the prefab is already open
    //  if (!(prefabStage != null && prefabStage.assetPath == assetPath))
    //  {
    //    prefabStage = PrefabStageUtility.OpenPrefab(assetPath);
    //  }
    //  var prefabContentsRoot = prefabStage.prefabContentsRoot;

    //  var settingObjects = prefabContentsRoot.transform.GetComponentsInChildren<SettingObject>(true, true).ToList();

    //  var identifierOrder = settingObjects
    //      //.Select((item, index) => new { item.Identifier, Index = index })
    //      .ToDictionary(x => x.Identifier, x => x);

    //  //for (int i = 0; i < collection.Settings.Count; i++)
    //  //{
    //  //  var setting = collection.Settings[i];
    //  //  var parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
    //  //}

    //  var root = prefabContentsRoot.GetComponentInChildren<RectTransform>();
    //  var settings = collection.Settings;

    //  Dictionary<string, List<string>> parentChildrenMapping = new Dictionary<string, List<string>>();
    //  var settingsMapping = collection.Settings.ToDictionary(x => x.Identifier, x => x);

    //  foreach (var setting in collection.Settings)
    //  {
    //    string parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
    //    if (!parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
    //    {
    //      children = new List<string>() { setting.Identifier };
    //      parentChildrenMapping.Add(parentIdentifier, children);
    //    }
    //    else
    //    {
    //      children.Add(setting.Identifier);
    //    }
    //  }

    //  if (settings != null)
    //  {
    //    for (int i = 0; i < settings.Count; i++)
    //    {
    //      var s = settings[i];
    //      if (s == null) { continue; }
    //      var elem = s.FindElement_UGUI(root, collection);
    //      //if (elem == null)
    //      //{
    //      //  elem = s.CreateElement_UGUI(root, collection);
    //      //}
    //      //bool parented = false;
    //      if (elem != null)
    //      {
    //        var parentIdentifier = s.InputElementProviderSettings.ParentIdentifier;
    //        if (parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
    //        {
    //          if (children != null)
    //          {
    //            var childIndex = children.IndexOf(s.Identifier);
    //            if (childIndex != -1 && childIndex > 0)
    //            {
    //              for (int j = childIndex - 1; j >= 0; j--)
    //              {
    //                var indexBefore = j;

    //                //var indexBefore = childIndex - 1;
    //                var settingIdentifierBefore = children[indexBefore];
    //                if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
    //                {
    //                  var siblingElement = settingBefore?.FindElement_UGUI(root, collection);
    //                  if (siblingElement != null)
    //                  {
    //                    var siblingParentObj = siblingElement.parent;
    //                    if (siblingParentObj != null)
    //                    {
    //                      var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //                      bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
    //                      if (isParentedCorrectly)
    //                      {
    //                        var siblingIndex = siblingElement.GetSiblingIndex();
    //                        elem.SetParent(parentSettingObject.transform);
    //                        elem.SetSiblingIndex(siblingIndex + 1);
    //                        //parented = true;
    //                        break;
    //                      }
    //                    }
    //                  }
    //                }
    //              }
    //            }
    //            // Setting is the first element of the list using that parent
    //            else if (childIndex == 0)
    //            {
    //              // Check if the setting has no siblings
    //              if (children.Count == 1)
    //              {
    //                elem.SetSiblingIndex(0);
    //              }
    //              else if (children.Count > childIndex + 1)
    //              {
    //                for (int j = childIndex + 1; j >= 0; j--)
    //                {
    //                  var indexAfter = j;
    //                  //var indexAfter = childIndex + 1;
    //                  if (children.Count > indexAfter)
    //                  {
    //                    var settingIdentifierAfter = children[indexAfter];
    //                    if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingAfter))
    //                    {
    //                      var siblingElement = settingAfter?.FindElement_UGUI(root, collection);
    //                      if (siblingElement != null)
    //                      {
    //                        var siblingParentObj = siblingElement.parent;
    //                        if (siblingParentObj != null)
    //                        {
    //                          var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();
    //                          bool isParentedCorrectly = parentSettingObject.Identifier == settingAfter.InputElementProviderSettings.ParentIdentifier;
    //                          if (isParentedCorrectly)
    //                          {
    //                            var siblingIndex = siblingElement.GetSiblingIndex();
    //                            elem.SetParent(parentSettingObject.transform);
    //                            elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
    //                            //parented = true;
    //                            break;
    //                          }
    //                        }
    //                      }
    //                    }
    //                  }
    //                }
    //                //else
    //                //{
    //                //  elem.SetSiblingIndex(childIndex);
    //                //}
    //              }
    //            }
    //          }
    //        }
    //      }
    //      else
    //      {
    //        ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
    //                     $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
    //      }
    //    }
    //  }

    //  PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
    //  prefabStage.ClearDirtiness();
    //  //PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);

    //  //EditorUtilities.PingObject(prefab);
    //  //DestroyImmediate(instance);
    //}

    protected GameObject DestroyUIElementsForSettings(bool selected)
    {
      //if (!isUnity2022) { return null; }
      if (collection == null) { return null; }
      if (collection.UguiLayoutPrefab == null) { return null; }

      var prefab = collection.UguiLayoutPrefab;
      if (prefab == null) { return null; }
      var prefabAssetPath = AssetDatabase.GetAssetPath(prefab);

      if (!CanModifyPrefab(prefab)) { return null; }

      //var currentlySelectedObject = Selection.activeObject;
      bool isPrefabOpen = false;

      PrefabUtility.SavePrefabAsset(prefab);
      var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
      if (currentPrefabStage != null)
      {
        if (currentPrefabStage.assetPath == prefabAssetPath)
        {
          //ConsoleLogger.LogWarning("Saving");
          PrefabUtility.SaveAsPrefabAsset(currentPrefabStage.prefabContentsRoot, currentPrefabStage.assetPath);
          currentPrefabStage.ClearDirtiness();
          isPrefabOpen = true;
        }
      }

      //var currentlySelected = Selection.activeObject;

      List<SettingObject> settingObjects = null;
      GameObject instance = null;

      if (isPrefabOpen)
      {
        if (currentPrefabStage.prefabContentsRoot != null)
        {
          settingObjects = currentPrefabStage.prefabContentsRoot.transform.GetComponentsInChildren<SettingObject>(true, true).ToList();
        }
        else
        {
          settingObjects = new List<SettingObject>();
        }
      }
      else
      {
        instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance == null) { return null; }
        settingObjects = instance.transform.GetComponentsInChildren<SettingObject>(true, true).ToList();
      }

      List<SettingHolder> settings = null;
      if (selected)
      {
        settings = settingsList.selectedItems?.Cast<SettingHolder>().ToList();
      }
      else
      {
        settings = collection.Settings;
      }

      if (settings != null)
      {
        foreach (var setting in settings)
        {
          if (setting == null) { continue; }

          var script = settingObjects.Find(s => s.Identifier == setting.Identifier);
          if (script == null) { continue; }
          try
          {
            DestroyImmediate(script.gameObject);
          }
          catch (Exception)
          {
#if !UNITY_2022_1_OR_NEWER
            ConsoleLogger.LogWarning("Removing objects inside a nested prefab in which they were not originally added in can NOT be removed in Unity 2021. " +
                                     "To use this functionality please use Unity 2022 or later.");
#else
            // TODO
#endif
          }
        }
      }

      if (!isPrefabOpen && instance != null)
      {
        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
        DestroyImmediate(instance);
      }

      if (currentPrefabStage != null && currentPrefabStage.prefabContentsRoot != null && !string.IsNullOrEmpty(currentPrefabStage.assetPath))
      {
        PrefabUtility.SaveAsPrefabAsset(currentPrefabStage.prefabContentsRoot, currentPrefabStage.assetPath);
        currentPrefabStage.ClearDirtiness();
      }
      //EditorUtility.SetDirty(prefab);
      //PrefabUtility.SavePrefabAsset(prefab);

      //EditorUtilities.PingObject(prefab);

      //ScheduleUtility.InvokeDelayedByFrames(() => Selection.activeObject = currentlySelectedObject);

      //ScheduleUtility.InvokeDelayedByFrames(() =>
      //{
      //  Selection.activeObject = currentlySelected;
      //}, 2);

      return prefab;
    }

    protected static void ApplyStyleProfileToHierarchy(RectTransform root, int frameDelay = 0)
    {
      if (root == null) { return; }
      if (root.TryGetComponent<RegisterStyleListenersInHierarchyInEditMode>(out var registerScript))
      {
        ScheduleUtility.InvokeDelayedByFrames(registerScript.GetAndRegisterStyleListenersFromHierarchy, frameDelay);
      }
      if (root.TryGetComponent<ApplyStyleProfile>(out var applyStyleProfileScript))
      {
        ScheduleUtility.InvokeDelayedByFrames(applyStyleProfileScript.ApplyProfile, frameDelay);
      }
    }

    #region Menu Items
    [MenuItem("CONTEXT/SettingsCollection/Utility/Deselect All", priority = 1)]
    protected static void DeselectAllCommand(MenuCommand command)
    {
      var collection = (SettingsCollection)command.context;
      GlobalEventHandler.InvokeEvent(DESELECT_ALL_EVENT_NAME, collection);
    }

    [MenuItem("CONTEXT/SettingsCollection/Utility/Select All", priority = 2)]
    protected static void SelectAllCommand(MenuCommand command)
    {
      var collection = (SettingsCollection)command.context;
      GlobalEventHandler.InvokeEvent(SELECT_ALL_EVENT_NAME, collection);
    }

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Duplicate Selected", priority = 3)]
    //protected static void DuplicateSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(DUPLICATE_SELECTED_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Expand All", priority = 10)]
    //protected static void ExpandAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(EXPAND_ALL_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Expand Selected", priority = 11)]
    //protected static void ExpandSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(EXPAND_SELECTED_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Collapse All", priority = 12)]
    //protected static void CollapseAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(COLLAPSE_ALL_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Collapse Selected", priority = 13)]
    //protected static void CollapseSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(COLLAPSE_SELECTED_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab All/Setting", priority = 100)]
    //protected static void SelectSettingTabAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_ALL_EVENT_NAME, collection, 0);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab All/Advanced", priority = 101)]
    //protected static void SelectAdvancedTabAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_ALL_EVENT_NAME, collection, 1);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab All/Input", priority = 102)]
    //protected static void SelectInputTabAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_ALL_EVENT_NAME, collection, 2);
    //}

    [MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab Selected/Setting", priority = 105)]
    protected static void SelectSettingTabSelectedCommand(MenuCommand command)
    {
      var collection = (SettingsCollection)command.context;
      GlobalEventHandler.InvokeEvent(SELECT_TAB_SELECTED_EVENT_NAME, collection, 0);
    }

    [MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab Selected/Advanced", priority = 106)]
    protected static void SelectAdvancedTabSelectedCommand(MenuCommand command)
    {
      var collection = (SettingsCollection)command.context;
      GlobalEventHandler.InvokeEvent(SELECT_TAB_SELECTED_EVENT_NAME, collection, 1);
    }

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab Selected/Input", priority = 107)]
    //protected static void SelectInputTabSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_SELECTED_EVENT_NAME, collection, 2);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Create/Duplicate (incl. Resources)", priority = 110)]
    //protected static void CreateDuplicateWithNewResources(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  SettingsCollectionCreationUtility.CreateSettingsCollectionAndResources(collection, false, false);
    //}

    #endregion
  }
}
