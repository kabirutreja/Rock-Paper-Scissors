using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class ScrollViewListFromScriptableObjectItemDataCreator<T> where T : ScriptableObjectItemData
  {
    protected string groupName;

    protected VisualTreeAsset itemTemplate;

    protected ScrollView itemContainer;
    public List<T> data = new List<T>();

    protected VisualElement root;
    protected ToolbarSearchField searchField;

    protected string itemDisplayNameLabelClass;
    protected string itemDescriptionNameLabelClass;
    protected string refreshButtonClass;
    protected string searchFieldClass;

    public Dictionary<T, VisualElement> items = new Dictionary<T, VisualElement>();

    public ScrollViewListFromScriptableObjectItemDataCreator(string groupName, VisualTreeAsset itemTemplate, VisualElement root, 
      string searchFieldClass,
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list")
    {
      this.searchFieldClass = searchFieldClass;
      this.groupName = groupName;
      this.itemTemplate = itemTemplate;
      this.itemDisplayNameLabelClass = itemDisplayNameLabelClass;
      this.itemDescriptionNameLabelClass = itemDescriptionNameLabelClass;
      this.refreshButtonClass = refreshButtonClass;

      this.root = root;
      ScheduleUtility.InvokeDelayedByFrames(Init);
    }

    protected virtual void Init()
    {
      RefreshList();

      var refreshButton = root.Q<Button>(className: refreshButtonClass);
      if (refreshButton == null)
      {
        refreshButton = new Button();
        root.Add(refreshButton);
      }

      refreshButton.text = "Refresh";
      refreshButton.clicked += RefreshList;

      SetupSearchField();
    }

    private void SetupSearchField()
    {
      searchField = root.Q<ToolbarSearchField>(className: searchFieldClass);

      if (searchField == null) { return; }

      searchField.RegisterValueChangedCallback(OnSearchStringChanged);
    }

    private void OnSearchStringChanged(ChangeEvent<string> evt)
    {
      var searchValue = evt.newValue;
      UpdateSearch(searchValue);
    }

    protected virtual void UpdateSearch(string searchValue)
    {
      bool isEmpty = string.IsNullOrEmpty(searchValue);
      isEmpty = isEmpty || string.IsNullOrWhiteSpace(searchValue);
      var searchValues = searchValue.Split(' ');
      var values = new List<string>();
      foreach (var value in searchValues)
      {
        values.Add(value.ToLowerInvariant());
      }

      for (int i = 0; i < data.Count; i++)
      {
        var item = data[i];

        if (item == null) { continue; }

        bool containsSearchValue = false;
        if (!isEmpty)
        {
          var itemName = item.DisplayName.ToLowerInvariant();

          foreach (var s in values)
          {
            if (string.IsNullOrEmpty(s)) { continue; }
            if (itemName.Contains(s))
            {
              containsSearchValue = true;
              break;
            }
          }
        }

        if (items.TryGetValue(item, out var elem))
        {
          elem.Show(isEmpty || containsSearchValue);
        }
      }
    }

    public void OrderByPriority(bool ascending = true)
    {
      if (ascending) { data.Sort((x, y) => x.Priority.CompareTo(y.Priority)); }
      else { data.Sort((x, y) => y.Priority.CompareTo(x.Priority)); }
      MatchElementOrderToListOrder();
    }

    public void OrderByName(bool ascending = true)
    {
      if (ascending) { data.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName)); }
      else { data.Sort((x, y) => y.DisplayName.CompareTo(x.DisplayName)); }
      MatchElementOrderToListOrder();
    }

    public void MatchElementOrderToListOrder()
    {
      for (int i = 0; i < data.Count; i++)
      {
        var item = data[i];
        if (item == null) { continue; }
        if (items.TryGetValue(item, out var elem))
        {
          elem.BringToFront();
        }
      }
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

      InitListItems();
      if (searchField != null)
      {
        UpdateSearch(searchField.value);
      }
    }

    protected virtual void InitListItems()
    {
      itemContainer = root.Q<ScrollView>();
      if (itemContainer == null)
      {
        itemContainer = new ScrollView();
        root.Add(itemContainer);
      }

      itemContainer.contentContainer.Clear();

      for (int i = 0; i < data.Count; i++)
      {
        var item = data[i];
        var elem = MakeListItem();
        items.AddOrUpdateDictionaryItem(item, elem);
        BindListItem(elem, i);
        itemContainer.Add(elem);
      }
    }

    protected virtual VisualElement MakeListItem()
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