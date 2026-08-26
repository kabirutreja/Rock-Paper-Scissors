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
  [CanEditMultipleObjects]
  //[CustomEditor(typeof(SettingsCollection))]
  public class SettingsCollectionEditor : EditorFromVisualTreeAsset_SettingsMenu
  {
    #region Fields
    protected SettingsCollection collection;

    protected ListView settingsList;

    protected Toggle sharedApplyImmediatelyToggle;
    protected TextField inputElementParentClassTextField;

    protected const string ADVANCED_FOLDOUT_TEXT = "Extra";

    protected const string LIST_CLASS = "settings-list";
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

    protected const string REFRESH_LIST_BUTTON_CLASS = "button__refresh-list-view";
    protected const string REBUILD_LIST_BUTTON_CLASS = "button__rebuild-list-view";
    protected const string SELECT_ALL_BUTTON_CLASS = "button__select-all";
    protected const string DESELECT_ALL_BUTTON_CLASS = "button__deselect-all";
    protected const string CLEAR_ALL_BUTTON_CLASS = "button__clear-all";

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
      var root = base.CreateInspectorGUI();

      collection = serializedObject.targetObject as SettingsCollection;

      Undo.undoRedoPerformed += OnUndoPerformed;
      //Undo.postprocessModifications += MyPostprocessModificationsCallback;

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
      if (tabMenu != null)
      {
        tabMenu.SelectTab(index);
      }
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
            AddSettingsFromCollection(collection);
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

      AddTooltipsAttribute.AddTooltip(typeof(SettingsCollection), settingsList, "settings");
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

      var foldout = root.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      if (foldout == null)
      {
        foldout = new Foldout() { value = false };
        foldout.AddToClassList(ITEM_FOLDOUT_CLASS);
        root.Add(foldout);
      }

      var propertyField = new PropertyField();
      propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
      if (foldout != null)
      {
        foldout.value = false;
        foldout.Add(propertyField);
      }
      else
      {
        root.Add(propertyField);
      }

      return root;
    }

    private void BindSettingsListItem(VisualElement elem, int index)
    {
      SettingHolder holder = collection.Settings[index];

      // TODO Make this work so the foldout state persists between playmode and recompile?
      var foldout = elem.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      if (foldout != null)
      {
        //foldout.viewDataKey = $"Item Foldout {index}";
        var holderProperty = serializedObject.FindProperty(PathToListItem(index));
        var expandedProperty = holderProperty?.FindPropertyRelative("expanded");

        if (expandedProperty != null)
        {
          foldout.BindProperty(expandedProperty);
        }
      }
      //foldout.UnregisterValueChangedCallback(OnSettingHolderFoldoutChange);
      //foldout.RegisterValueChangedCallback(OnSettingHolderFoldoutChange);

      BindListItemLabel(elem, holder, index);

      BindPropertyField(elem, holder, index);
    }

    private void BindListItemLabel(VisualElement elem, SettingHolder holder, int index)
    {
      var foldout = elem.Q<Foldout>(className: ITEM_FOLDOUT_CLASS);
      if (foldout != null)
      {
        foldout.SetText("List Element");
        var foldoutLabel = foldout.Q<Label>(className: "unity-foldout__text");
        // Update the label text for the list item
        foldoutLabel?.SetText(holder.MenuName);
      }
    }

    private void BindPropertyField(VisualElement elem, SettingHolder holder, int index)
    {
      var propertyField = elem.Q<PropertyField>(className: ITEM_PROPERTY_CLASS);

      if (propertyField == null)
      {
        propertyField = new PropertyField();
        propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
        elem.Add(propertyField);
        //propertyField.RegisterCallback<ChangeEvent<SettingHolder_FromClass>>(OnSettingHolderChanged);
      }

      propertyField.RegisterValueChangeCallback((evt) => OnItemPropertyChanged(elem, holder, index));

      propertyField.bindingPath = PathToListItem(index);
      propertyField.Bind(serializedObject);
      //propertyField.OpenPropertyFieldFoldout();

      var foldout = elem.Q<Foldout>();
      foldout?.RegisterValueChangedCallback((evt) => OpenPropertyFoldoutsInHierarchy(evt, elem));
    }

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
        Undo.RecordObject(serializedObject.targetObject, "Removed settings");
        foreach (var holder in settingHolders)
        {
          collection.Settings.Remove(holder);
        }
        serializedObject.Update();
        EditorUtility.SetDirty(serializedObject.targetObject);

        settingsList.ClearSelection();
        RefreshItems();
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

    #region Menu Items
    //[MenuItem("CONTEXT/SettingsCollection/Utility/Deselect All", priority = 1)]
    //protected static void DeselectAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(DESELECT_ALL_EVENT_NAME, collection);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select All", priority = 2)]
    //protected static void SelectAllCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_ALL_EVENT_NAME, collection);
    //}

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

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab Selected/Setting", priority = 105)]
    //protected static void SelectSettingTabSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_SELECTED_EVENT_NAME, collection, 0);
    //}

    //[MenuItem("CONTEXT/SettingsCollection/Utility/Select Tab Selected/Advanced", priority = 106)]
    //protected static void SelectAdvancedTabSelectedCommand(MenuCommand command)
    //{
    //  var collection = (SettingsCollection)command.context;
    //  GlobalEventHandler.InvokeEvent(SELECT_TAB_SELECTED_EVENT_NAME, collection, 1);
    //}

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
