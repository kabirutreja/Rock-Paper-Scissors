using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public class ClassDropdown : AdvancedDropdown
  {
    [SerializeField]
    private string displayName = string.Empty;
    private Type[] types;
    private Action<ClassDropdownItem> onClassItemSelected = null;
    private string stringToReplace = null;
    private string replacementString = string.Empty;

    List<AdvancedDropdownItem> dropdownItems = new List<AdvancedDropdownItem>();
    Dictionary<AdvancedDropdownItem, int> itemOrder = new Dictionary<AdvancedDropdownItem, int>();
    Dictionary<AdvancedDropdownItem, List<AdvancedDropdownItem>> itemHierarchies =
      new Dictionary<AdvancedDropdownItem, List<AdvancedDropdownItem>>();

    public ClassDropdown(AdvancedDropdownState state, Type[] types, string displayName,
                         Vector2 minimumSize, Action<ClassDropdownItem> onClassItemSelected)
      : base(state)
    {
      this.types = types;
      this.displayName = displayName;
      this.onClassItemSelected += onClassItemSelected;
      this.minimumSize = minimumSize;
    }

    public ClassDropdown(AdvancedDropdownState state, Type[] types, string displayName,
                     Vector2 minimumSize, Action<ClassDropdownItem> onClassItemSelected,
                     string stringToReplace)
      : this(state, types, displayName, minimumSize, onClassItemSelected)
    {
      this.stringToReplace = stringToReplace;
    }

    public ClassDropdown(AdvancedDropdownState state, Type[] types, string displayName,
                         Vector2 minimumSize, Action<ClassDropdownItem> onClassItemSelected,
                         string stringToReplace, string replacementString)
      : this(state, types, displayName, minimumSize, onClassItemSelected, stringToReplace)
    {
      this.replacementString = replacementString;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
      var root = new AdvancedDropdownItem(displayName);
      AddClassEntries(root);
      return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
      if (item is ClassDropdownItem classDropdownItem)
      {
        onClassItemSelected?.Invoke(classDropdownItem);
      }
    }

    private void AddChildToHierarchy(AdvancedDropdownItem item, AdvancedDropdownItem child)
    {
      int order = -1;
      if (child is ClassDropdownItem classDropdownItem)
      {
        var orderAttribute = (MenuOrderAttribute)Attribute.GetCustomAttribute(classDropdownItem.Type,
                                                                              typeof(MenuOrderAttribute));
        order = orderAttribute != null ? orderAttribute.Order : -1;
      }
      else
      {
        if (!itemOrder.TryGetValue(item, out order))
        {
          order = -1;
        }
      }

      itemOrder.AddOrUpdateDictionaryItem(child, order);
      // Set the order of the parent item to the child order if it is higher
      itemOrder.AddOrUpdateDictionaryItem(item, Mathf.Max(order, itemOrder[item]));

      if (itemHierarchies.TryGetValue(item, out var children))
      {
        children.Add(child);
      }
      else
      {
        children = new List<AdvancedDropdownItem>() { child };
        itemHierarchies.AddOrUpdateDictionaryItem(item, children);
      }
    }

    private void OrderAndAttachChilds(AdvancedDropdownItem item)
    {
      // Check if the there are child items
      if (itemHierarchies.TryGetValue(item, out var childs))
      {
        if (childs?.Count > 0)
        {
          var orderedChilds = childs.OrderByDescending(i => itemOrder[i]);

          foreach (var child in orderedChilds)
          {
            OrderAndAttachChilds(child);
            item.AddChild(child);
          }
        }
      }
    }

    private void AddClassEntries(AdvancedDropdownItem rootItem)
    {
      //var types = new List<Type>();
      //foreach (var type in parentTypes)
      //{
      //  foreach(var derivedType in type.GetDerivedTypesFromAllAssemblies(false)) {
      //    types.AddIfNotContains(derivedType);
      //  }
      //}
      //itemHierarchies = new Dictionary<AdvancedDropdownItem, List<AdvancedDropdownItem>>();

      Array.ForEach(types, t =>
      {
        string typeDisplayName = t.Name;

        var displayNameAttribute = (DisplayNameAttribute)Attribute.GetCustomAttribute(t, typeof(DisplayNameAttribute));
        if (displayNameAttribute != null)
        {
          typeDisplayName = displayNameAttribute.DisplayName;
        }
        else
        {
          // Check if a valid string to replace was provided
          if (!string.IsNullOrEmpty(stringToReplace))
          {
            typeDisplayName = typeDisplayName.Replace(stringToReplace, replacementString);
          }
          typeDisplayName = typeDisplayName.FormatFieldOrClassName();
        }

        //t.Name.Replace(stringToReplace, replacementString).FormatFieldOrClassName();
        var orderAttribute = (MenuOrderAttribute)Attribute.GetCustomAttribute(t, typeof(MenuOrderAttribute));
        int order = orderAttribute != null ? orderAttribute.Order : -1;

        var menuPathAttribute = (MenuPathAttribute)Attribute.GetCustomAttribute(t, typeof(MenuPathAttribute));
        if (menuPathAttribute != null)
        {
          var directories = menuPathAttribute.Path.Split('/');
          AdvancedDropdownItem item = dropdownItems.Find(i => i.name == directories[0]);
          if (item == null)
          {
            item = new AdvancedDropdownItem(directories[0]);
            dropdownItems.AddIfNotContains(item);
            itemOrder.Add(item, order);
          }
          else
          {
            itemOrder[item] = Mathf.Min(order, itemOrder[item]);
          }
          if (directories.Length > 1)
          {
            for (int i = 1; i < directories.Length; i++)
            {
              if (string.IsNullOrEmpty(directories[i]) == false)
              {
                //AdvancedDropdownItem child = item.children.ToList().Find(c => c.name == directories[i]);
                itemHierarchies.TryGetValue(item, out var childs);
                AdvancedDropdownItem child = childs != null ? childs.Find(c => c.name == directories[i]) : null;

                if (child == null)
                {
                  child = new AdvancedDropdownItem(directories[i]);
                  //itemOrder.Add(child, order);
                  //if (itemHierarchies.TryGetValue(item, out var children))
                  //{
                  //  children.Add(child);
                  //}
                  //else
                  //{
                  //  children = new List<AdvancedDropdownItem>() { child };
                  //  itemHierarchies.AddOrUpdateDictionaryItem(item, children);
                  //}
                  AddChildToHierarchy(item, child);
                  //item.AddChild(child);
                }
                item = child;
              }
            }
          }
          //item.AddChild(new ClassDropdownItem(typeDisplayName, t));
          AddChildToHierarchy(item, new ClassDropdownItem(typeDisplayName, t));
        }
        else
        {
          var item = new ClassDropdownItem(typeDisplayName, t);
          dropdownItems.AddIfNotContains(item);
          itemOrder.Add(item, order);
        }
      });

      dropdownItems = dropdownItems.OrderByDescending(i => itemOrder[i]).ToList();

      foreach (var item in dropdownItems)
      {
        OrderAndAttachChilds(item);
        rootItem.AddChild(item);
      }

      dropdownItems.Clear();
      itemOrder.Clear();
      itemHierarchies.Clear();
    }
  }
}