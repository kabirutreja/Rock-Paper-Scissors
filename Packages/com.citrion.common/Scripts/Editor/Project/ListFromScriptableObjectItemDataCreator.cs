using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class ListFromScriptableObjectItemDataCreator<T> where T : ScriptableObjectItemData
  {
    protected string groupName;

    protected VisualTreeAsset itemTemplate;

    protected ListView list = null;
    protected List<T> data = new List<T>();

    protected string itemDisplayNameLabelClass;
    protected string itemDescriptionNameLabelClass;
    protected string refreshButtonClass;

    public ListFromScriptableObjectItemDataCreator(string groupName, VisualTreeAsset itemTemplate, VisualElement root,
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list")
    {
      this.groupName = groupName;
      this.itemTemplate = itemTemplate;
      this.itemDisplayNameLabelClass = itemDisplayNameLabelClass;
      this.itemDescriptionNameLabelClass = itemDescriptionNameLabelClass;

      RefreshList();

      InitList(root);

      var refreshButton = root.Q<Button>(className: refreshButtonClass);
      if (refreshButton == null)
      {
        refreshButton = new Button();
        root.Add(refreshButton);
      }

      refreshButton.text = "Refresh";
      refreshButton.clicked += RefreshList;
    }

    protected virtual void RefreshList()
    {
      data.Clear();
      var allScriptableObjects = AssetUtilities.GetAllAssetsOfType<T>();
      var orderedObjects = allScriptableObjects.OrderBy(i => i.Priority);

      foreach (var item in orderedObjects)
      {
        if (item.Group == groupName || string.IsNullOrEmpty(groupName))
        {
          data.Add(item);
        }
      }
      list?.RefreshItems();
    }

    protected virtual void InitList(VisualElement rootVisualElement)
    {
      list = rootVisualElement.Q<ListView>();
      if (list == null)
      {
        list = new ListView();
        rootVisualElement.Add(list);
      }

#if UNITY_2021_1_OR_NEWER
      //bundleList.headerTitle = "List";
      list.selectionType = SelectionType.None;
      list.reorderable = false;
      list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
      //bundleList.reorderMode = ListViewReorderMode.Animated;
      list.fixedItemHeight = 100;
      list.showAddRemoveFooter = false;
#else
      list.itemHeight = 100;
#endif
      list.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
      list.itemsSource = data;
      list.showBorder = true;

      list.makeItem = MakeListItem;
      list.bindItem = BindListItem;
    }

    private VisualElement MakeListItem()
    {
      VisualElement currentElement = null;
      if (itemTemplate != null)
      {
        currentElement = itemTemplate.Instantiate();
      }
      else
      {
        currentElement = new Label();
      }
      return currentElement;
    }

    protected virtual void BindListItem(VisualElement elem, int index)
    {
      if (data.Count <= index) { return; }

      var item = data[index];

      if (item == null) { return; }

      var displayNameLabel = elem.Q<Label>(className: itemDisplayNameLabelClass);
      displayNameLabel?.SetText(item.DisplayName);

      var descriptionLabel = elem.Q<Label>(className: itemDescriptionNameLabelClass);
      descriptionLabel?.SetText(item.Description);
    }
  }
}