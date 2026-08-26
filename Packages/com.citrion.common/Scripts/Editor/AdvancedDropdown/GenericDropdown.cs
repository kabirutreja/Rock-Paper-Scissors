using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public class GenericDropdown<T> : AdvancedDropdown
  {
    [SerializeField]
    private string displayName = string.Empty;
    private List<GenericDropdownItemData<T>> data;
    private Action<GenericDropdownItem<T>> onDropdownItemSelected = null;

    public GenericDropdown(AdvancedDropdownState state, List<GenericDropdownItemData<T>> data, string displayName,
                           Vector2 minimumSize, Action<GenericDropdownItem<T>> onDropdownItemSelected)
      : base(state)
    {
      this.data = data;
      this.displayName = displayName;
      this.onDropdownItemSelected += onDropdownItemSelected;
      this.minimumSize = minimumSize;
    }

    public static void Show(List<GenericDropdownItemData<T>> data, string displayName,
                            Action<GenericDropdownItem<T>> onDropdownItemSelected,
                            Vector2 minimumSize)
    {
      var dropdown = new GenericDropdown<T>(new AdvancedDropdownState(), data, displayName,
                                         minimumSize, onDropdownItemSelected);
      var rect = new Rect(Event.current.mousePosition, new Vector2(0, 0));
      dropdown.Show(rect);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
      var root = new AdvancedDropdownItem(string.IsNullOrEmpty(displayName) ? string.Empty : displayName);
      AddEntries(root);
      return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
      if (item is GenericDropdownItem<T> genericDropdownItem)
      {
        onDropdownItemSelected?.Invoke(genericDropdownItem);
      }
    }

    private void AddEntries(AdvancedDropdownItem rootItem)
    {
      List<AdvancedDropdownItem> dropdownItems = new List<AdvancedDropdownItem>();
      Dictionary<AdvancedDropdownItem, int> itemOrder = new Dictionary<AdvancedDropdownItem, int>();

      T v;
      string path;
      int order;

      foreach (var d in data)
      {
        v = d.value;
        path = d.path;
        order = d.order;

        // Check if a valid path is available
        if (string.IsNullOrEmpty(path) == false)
        {
          var directories = path.Split('/');
          AdvancedDropdownItem item = dropdownItems.Find(i => i.name == directories[0]);
          if (item == null)
          {
            item = new AdvancedDropdownItem(directories[0]);
            dropdownItems.AddIfNotContains(item);
            itemOrder.Add(item, order);
          }
          else
          {
            itemOrder[item] = Mathf.Max(order, itemOrder[item]);
          }
          if (directories.Length > 1)
          {
            for (int j = 1; j < directories.Length; j++)
            {
              if (string.IsNullOrEmpty(directories[j]) == false)
              {
                #if UNITY_6000_5_OR_NEWER
                AdvancedDropdownItem child = item.childList.ToList().Find(c => c.name == directories[j]);
                #else
                AdvancedDropdownItem child = item.children.ToList().Find(c => c.name == directories[j]);
                #endif

                if (child == null)
                {
                  child = new AdvancedDropdownItem(directories[j]);
                  item.AddChild(child);
                }
                item = child;
              }
            }
          }
          item.AddChild(new GenericDropdownItem<T>(d.displayName, d.value));
        }
        else
        {
          var item = new GenericDropdownItem<T>(d.displayName, d.value);
          dropdownItems.AddIfNotContains(item);
          itemOrder.Add(item, order);
        }
      }

      dropdownItems = dropdownItems.OrderByDescending(i => itemOrder[i]).ToList();

      foreach (var item in dropdownItems)
      {
        rootItem.AddChild(item);
      }
    }
  }
}