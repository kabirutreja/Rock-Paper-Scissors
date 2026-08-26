using CitrioN.Common;
using CitrioN.Common.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.Editor
{
  [CanEditMultipleObjects]
  [CustomEditor(typeof(StyleProfile))]
  public class StyleProfileEditor : EditorFromVisualTreeAsset
  {
    protected ListView dataList;

    protected StyleProfile styleProfile;

    protected const string LIST_CLASS = "style-list";
    protected const string ITEM_ROOT_CLASS = "item-root";
    protected const string ITEM_PROPERTY_CLASS = "item-property";
    protected const string IDENTIFIER_PROPERTY_FIELD_CLASS = "identifier-property";

    public override string UxmlPath => $"Packages/com.citrion.styleprofile/UI Toolkit/UXML/Editors/{GetType().Name}.uxml";

    public override string StyleSheetPath => $"Packages/com.citrion.styleprofile/UI Toolkit/USS/Editors/{GetType().Name}";

    public string PathToListItem(int index) => $"data.Array.data[{index}]";

    public override VisualElement CreateInspectorGUI()
    {
      var root = base.CreateInspectorGUI();

      styleProfile = serializedObject.targetObject as StyleProfile;

      //Undo.undoRedoPerformed += OnUndoPerformed;

      InitIdentifierProperty(root);
      InitSettingsList(root);

      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);

      return root;
    }

    private void InitIdentifierProperty(VisualElement root)
    {
      var identifierField = root?.Q<PropertyField>(className: IDENTIFIER_PROPERTY_FIELD_CLASS);
      if (identifierField == null)
      {
        identifierField = new PropertyField();
        root.Add(identifierField);
      }
      string identifierPropertyName = "identifier";
      var identifierProperty = serializedObject?.FindProperty(identifierPropertyName);
      if (identifierProperty == null)
      {
        ConsoleLogger.LogWarning($"No '{identifierPropertyName}' property found");
        return;
      }
      identifierField.BindProperty(identifierProperty);
    }

    private void InitSettingsList(VisualElement root)
    {
      dataList = root.Q<ListView>(className: LIST_CLASS);
      if (dataList == null)
      {
        dataList = new ListView();
        dataList.AddToClassList(LIST_CLASS);
        root.Add(dataList);
      }

      // TODO Only apply the following if the settings list was created via code and
      // was not in the uxml beforehand because then those settings should be used
#if UNITY_2021_1_OR_NEWER
      dataList.headerTitle = "List";
      dataList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
      dataList.reorderMode = ListViewReorderMode.Animated;
      dataList.fixedItemHeight = 100;
      dataList.showAddRemoveFooter = true;
#else
      settingsList.itemHeight = 100;
#endif
      dataList.reorderable = true;
      dataList.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
      dataList.itemsSource = styleProfile.Data;
      dataList.showBorder = true;
      dataList.selectionType = SelectionType.Multiple;

      dataList.makeItem = MakeListItem;
      dataList.bindItem = BindSettingsListItem;


#if UNITY_2021_1_OR_NEWER
      dataList.SetAddRemoveButtonAction(true, OnStyleDataListAddButtonClicked);
      dataList.SetAddRemoveButtonAction(false, OnSettingsListRemoveButtonClicked);

      dataList.itemIndexChanged += OnItemIndexChanged;
#endif

      AddTooltipsAttribute.AddTooltip(typeof(StyleProfile), dataList, "data");
    }

    private VisualElement MakeListItem()
    {
      var root = new VisualElement();
      root.AddToClassList(ITEM_ROOT_CLASS);

      var propertyField = new PropertyField();
      propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
      root.Add(propertyField);

      return root;
    }

    private void BindSettingsListItem(VisualElement elem, int index)
    {
      var data = styleProfile.Data[index];

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
      ////foldout.UnregisterValueChangedCallback(OnSettingHolderFoldoutChange);
      ////foldout.RegisterValueChangedCallback(OnSettingHolderFoldoutChange);

      //BindListItemLabel(elem, holder, index);

      BindPropertyField(elem, data, index);
    }

    private void BindPropertyField(VisualElement elem, StringToGenericDataRelation<StyleProfileData> data, int index)
    {
      var propertyField = elem.Q<PropertyField>(className: ITEM_PROPERTY_CLASS);

      if (propertyField == null)
      {
        propertyField = new PropertyField();
        propertyField.AddToClassList(ITEM_PROPERTY_CLASS);
        elem.Add(propertyField);
      }

      //propertyField.RegisterValueChangeCallback((evt) => OnItemPropertyChanged(elem, data, index));

      propertyField.bindingPath = PathToListItem(index);
      propertyField.Bind(serializedObject);
      //propertyField.OpenPropertyFieldFoldout();

      var foldout = elem.Q<Foldout>();
      foldout?.RegisterValueChangedCallback((evt) => OpenPropertyFoldoutsInHierarchy(evt, elem));
      OpenPropertyFoldoutsInHierarchy(true, elem);
    }

    private void OpenPropertyFoldoutsInHierarchy(ChangeEvent<bool> evt, VisualElement elem)
    {
      if (evt.newValue == true)
      {
        OpenPropertyFoldoutsInHierarchy(true, elem);
      }
    }

    private void OpenPropertyFoldoutsInHierarchy(bool open, VisualElement elem)
    {
      var propertyFields = elem.Query<PropertyField>().ToList();
      foreach (var propertyField in propertyFields)
      {
        propertyField?.SetPropertyFieldFoldoutValue(open);
      }
    }

    private void OnSettingsListAddButtonClicked()
    {
      var data = new StringToGenericDataRelation<StyleProfileData>("", new StyleProfileData_Color());

      Undo.RecordObject(serializedObject.targetObject, "Added style data");

      styleProfile.Data.Add(data);

      serializedObject.Update();
      EditorUtility.SetDirty(serializedObject.targetObject);

      RefreshItems();
    }

    private void RefreshItems()
    {
#if UNITY_2021_1_OR_NEWER
      try
      {
        dataList?.RefreshItems();
      }
      catch (Exception)
      {
        // TODO Should anything happen in this case like another scheduled refresh?
        //throw;
      }
#else
      dataList?.Refresh();
#endif
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

    private void OnSettingsListRemoveButtonClicked()
    {
      if (dataList == null) { return; }
      var dataCount = dataList.itemsSource.Count;
      if (dataCount < 1) { return; }

      var selectedIndices = dataList.selectedIndices.ToList();
      if (selectedIndices.Count == 0)
      {
        selectedIndices.Add(dataList.childCount - 1);
      }

      var data = new List<StringToGenericDataRelation<StyleProfileData>>();

      for (int i = 0; i < selectedIndices.Count; i++)
      {
        var index = selectedIndices[i];
        bool isOutOfBounds = index < 0 || index >= dataCount;
        if (isOutOfBounds && selectedIndices.Count <= 1)
        {
          index = dataCount - 1;
        }

        var instance = styleProfile.Data[index];
        if (instance != null)
        {
          data.AddIfNotContains(instance);
        }
      }

      if (data.Count > 0)
      {
        Undo.RecordObject(serializedObject.targetObject, "Removed data");
        foreach (var item in data)
        {
          styleProfile.Data.Remove(item);
        }
        serializedObject.Update();
        EditorUtility.SetDirty(serializedObject.targetObject);

        dataList.ClearSelection();
        RefreshItems();
      }
    }

    private void OnStyleDataListAddButtonClicked()
    {
      EditorUtilities.GetInstanceOfTypeFromAdvancedDropdown
        (findDerivedTypes: true, (o) => AddStyleData(o), label: "", "Style Data", "", typeof(StyleProfileData));
    }

    private void AddStyleData(object value)
    {
      if (!(value is StyleProfileData styleProfileData))
      {
        ConsoleLogger.LogWarning($"Provided value must be of type {nameof(StyleProfileData)}");
        return;
      }

      Undo.RecordObject(serializedObject.targetObject, "Added style data");

      styleProfile.Data.Add(new StringToGenericDataRelation<StyleProfileData>("", styleProfileData));

      serializedObject.Update();
      EditorUtility.SetDirty(serializedObject.targetObject);

      //RefreshItems();
      dataList.Rebuild();
    }
  }
}