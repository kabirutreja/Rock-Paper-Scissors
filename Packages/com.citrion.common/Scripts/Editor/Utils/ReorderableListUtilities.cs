using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.GenericMenu;
using static UnityEditorInternal.ReorderableList;

namespace CitrioN.Common.Editor
{
  /// <summary>
  /// Contains utility methods for <see cref="ReorderableList"/>s.
  /// </summary>
  public static class ReorderableListUtilities
  {
    public static Dictionary<string, ReorderableList> propertyLists = new Dictionary<string, ReorderableList>();

    public static Dictionary<IList, ReorderableList> ilists = new Dictionary<IList, ReorderableList>();

    public static Dictionary<ReorderableList, int> selectedElements = new Dictionary<ReorderableList, int>();

    public static IList GetListInstance(this ReorderableList list)
    {
      // Get the list object
      var listObject = list.GetListTargetObject();

      if (listObject != null)
      {
        // Get the field of the list
        var field = listObject.GetType().GetSerializableField(list.serializedProperty.name);
        // Get the value of the list object
        return (IList)field.GetValue(listObject);
      }

      foreach (var pair in ilists)
      {
        if (pair.Value == list)
        {
          return pair.Key;
        }
      }

      return null;
    }

    public static UnityEngine.Object GetListTargetObject(this ReorderableList list)
    {
      return list.serializedProperty?.serializedObject?.targetObject;
    }

    public static void DrawReorderableList(SerializedProperty property, HeaderCallbackDelegate onDrawHeader,
    ElementCallbackDelegate onDrawElement, SelectCallbackDelegate onSelect, ReorderCallbackDelegateWithDetails onReorder,
    AddCallbackDelegate onAdd, RemoveCallbackDelegate onRemove, ref Dictionary<string, ReorderableList> lists, IList list)
    {
      SerializedObject serializedObject = property.serializedObject;
      //EditorGUI.BeginChangeCheck();
      ReorderableList reorderableList = null;
      lists.TryGetValue(property.propertyPath, out reorderableList);
      if (reorderableList == null)
      {
        reorderableList = new ReorderableList(serializedObject, property, true, true, true, true);
        reorderableList.drawHeaderCallback = onDrawHeader;
        reorderableList.drawElementCallback = onDrawElement;
        reorderableList.onSelectCallback = onSelect;
        reorderableList.onReorderCallbackWithDetails += onReorder;
        reorderableList.onAddCallback = onAdd;
        reorderableList.onRemoveCallback = onRemove;

        var listRect = GUILayoutUtility.GetRect(0, reorderableList.GetHeight());
        reorderableList.DoList(listRect);
        lists.Add(property.propertyPath, reorderableList);
      }
      //else
      {
        reorderableList.DoLayoutList();
      }

      int index = reorderableList.index;
      if (index >= 0 && reorderableList.count > index)
      {
        EditorGUILayout.BeginVertical("box");
        var selectedElement = property.GetArrayElementAtIndex(index);
        //Debug.Log(selectedElement.propertyPath);
        Type objectType = selectedElement.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fieldInfo = objectType.GetSerializableField(selectedElement.propertyPath);
        //Debug.Log(fieldInfo.GetType());

        //EditorGUILayout.LabelField(script.test[index].GetType().Name);

        if (list.Count > index)
        {
          EditorUtilities.DrawSelectedClassFields(list[index]);
        }
        EditorGUILayout.EndVertical();
        //var depth = selectedElement.depth;
        //while (selectedElement.NextVisible(true) && selectedElement.depth > depth)
        //{
        //  // TODO Iterate over fields and show them
        //  //EditorGUILayout.PropertyField(selectedElement, true);

        //}
      }

      //if (EditorGUI.EndChangeCheck())
      //{
      //  // TODO Record undo
      //  serializedObject.ApplyModifiedProperties();
      //}
    }

    public static void DrawReorderableList(IList list, Type elementType, HeaderCallbackDelegate onDrawHeader,
           ElementCallbackDelegate onDrawElement, SelectCallbackDelegate onSelect, ReorderCallbackDelegateWithDetails onReorder,
           AddCallbackDelegate onAdd, RemoveCallbackDelegate onRemove, ref Dictionary<IList, ReorderableList> lists,
           bool showClickedElementDetails = true, UnityEngine.Object obj = null,
           bool draggable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
    {
      //EditorGUI.BeginChangeCheck();

      if (list == null)
      {
        EditorGUILayout.HelpBox("The list to display is null!", MessageType.Warning);
        return;
      }

      ReorderableList reorderableList = null;
      lists.TryGetValue(list, out reorderableList);
      if (reorderableList == null)
      {
        reorderableList = new ReorderableList(list, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton);
        reorderableList.drawHeaderCallback = onDrawHeader;
        reorderableList.drawElementCallback = onDrawElement;
        reorderableList.onSelectCallback = onSelect;
        reorderableList.onReorderCallbackWithDetails += onReorder;
        reorderableList.onAddCallback = onAdd;
        reorderableList.onRemoveCallback = onRemove;

        var listRect = GUILayoutUtility.GetRect(0, reorderableList.GetHeight());
        reorderableList.DoList(listRect);
        lists.Add(list, reorderableList);
      }
      //else
      {
        reorderableList.draggable = draggable;
        reorderableList.displayAdd = displayAddButton;
        reorderableList.displayRemove = displayRemoveButton;
        reorderableList.DoLayoutList();
      }

      if (showClickedElementDetails)
      {
        int index = reorderableList.index;
        if (index >= 0 && reorderableList.count > index)
        {
          EditorGUILayout.BeginVertical("box");
          var selectedElement = list[index];
          if (selectedElement != null)
          {
            Type objectType = selectedElement.GetType();

            if (list.Count > index)
            {
              EditorUtilities.DrawSelectedClassFields(list[index]);
            }
          }
          EditorGUILayout.EndVertical();
        }
      }

      //if (EditorGUI.EndChangeCheck() && obj != null)
      //{
      //  Undo.RecordObject(obj, "Changed something");
      //  //serializedObject.ApplyModifiedProperties();
      //}
    }

    public static void OnAdd(ReorderableList list)
    {
      //var menu = CreateAddInstanceOfTypeToListMenu(EditorUtilities.GetDerivedTypesFromAllAssemblies
      //                                            (EditorUtilities.GetSerializedPropertyType(list.serializedProperty), false),
      //                                             list.GetListInstance());

      var listInstance = list.GetListInstance();
      var listElementType = listInstance.GetType().GetGenericArguments()[0];

      if (typeof(UnityEngine.Object).IsAssignableFrom(listElementType))
      {
        //listInstance.Add(null);
        //var instance = Activator.CreateInstance(listElementType);
        listInstance.Add(null);
      }
      else if (listElementType.IsValueType)
      {
        listInstance.Add(Activator.CreateInstance(listElementType));
      }
      else
      {
        var types = listElementType.GetDerivedTypesForSelection();
        if (types.Count == 1)
        {
          var objectInstance = types[0] == typeof(string) ? string.Empty : Activator.CreateInstance(types[0]);
          listInstance.Add(objectInstance);
        }
        else if (types.Count > 0)
        {
          var menu = CreateAddInstanceOfTypeToListMenu(listElementType.GetDerivedTypesForSelection(), listInstance);

          if (menu != null)
          {
            int menuItems = menu.GetItemCount();
            if (menuItems > 0) { menu.ShowAsContext(); }
          }
        }
        else
        {
          if (!listElementType.IsInterface)
          {
            var objectInstance = Activator.CreateInstance(listElementType);
            listInstance.Add(objectInstance);
          }
        }
      }

      var property = list.serializedProperty;
      if (property != null)
      {
        EditorUtility.SetDirty(property.serializedObject.targetObject);
      }
    }

    public static AddCallbackDelegate GetLimitedOnAdd(int itemLimit = 1)
    {
      AddCallbackDelegate addCallback = (ReorderableList l) =>
      {
        if (l.list.Count < itemLimit)
        {
          OnAdd(l);
        }
      };
      return addCallback;
    }

    public static AddCallbackDelegate GetGenericMenuToAddType(IList listInstance, bool findDerivedTypes, params Type[] types)
    {
      AddCallbackDelegate addCallbackDelegate = (ReorderableList list) =>
      {
        var typesToShow = new List<Type>();

        if (findDerivedTypes)
        {
          foreach (var type in types)
          {
            foreach (var t in type.GetDerivedTypesForSelection(false))
            {
              typesToShow.AddIfNotContains(t);
            }
          }
        }
        else
        {
          typesToShow = types.ToList();
        }

        var menu = CreateAddInstanceOfTypeToListMenu(typesToShow, listInstance);

        if (menu != null)
        {
          int menuItems = menu.GetItemCount();
          if (menuItems > 0) { menu.ShowAsContext(); }
        }
      };

      return addCallbackDelegate;
    }

    public static AddCallbackDelegate GetAdvancedDropdownToAddType(IList listInstance, bool findDerivedTypes,
                                                                   string label = "", params Type[] types)
    {
      if (findDerivedTypes)
      {
        var derivedTypes = new List<Type>();
        Array.ForEach(types, t =>
        {
          t.GetDerivedTypesForSelection(false).ForEach(d => derivedTypes.AddIfNotContains(d));
        });
        types = derivedTypes.ToArray();
      }

      AddCallbackDelegate addCallbackDelegate = (ReorderableList list) =>
      {
        Action<ClassDropdownItem> onClassItemSelected = null;
        onClassItemSelected += (c) =>
        {
          if (c.Type.IsSubclassOf(typeof(ScriptableObject)))
          {
            var instance = ScriptableObject.CreateInstance(c.Type);
            listInstance.Add(instance);
            GameObject.DestroyImmediate(instance);
          }
          else
          {
            var instance = Activator.CreateInstance(c.Type);
            listInstance.Add(instance);
          }
        };
        ClassDropdownUtilities.ShowClassDropdown(types, label, onClassItemSelected, new Vector2(280, 260));
      };

      return addCallbackDelegate;
    }

    public static void OnRemove(ReorderableList list)
    {
      // Remove the entry for the removed index
      list.GetListInstance()?.RemoveAt(list.index);
    }

    public static void OnReorder(ReorderableList list, int oldIndex, int newIndex)
    {
      var listInstance = list.list;

      if (list.list == null)
      {
        listInstance = list.GetListInstance();
        Debug.Log("list is null");
      }

      if (listInstance == null)
      {
        Debug.LogWarning($"Unable to find Ilist for {list}");
        return;
      }
      //for (int i = 0; i < list.count; i++)
      //{
      //  Debug.Log(listInstance[i].ToString());
      //  foreach (var field in listInstance[i].GetType().GetFields())
      //  {
      //    Debug.Log(field.Name + ": " + field.GetValue(listInstance[i]));
      //  }
      //}

      var oldListElementCopy = listInstance[oldIndex];
      //Debug.Log(oldListElementCopy.GetType().Name);
      var newElementCopy = listInstance[newIndex];
      //Debug.Log(newElementCopy.GetType().Name);
      //var shiftedElementCopy = listInstance[newIndex];
      if (oldIndex > newIndex)
      {
        //  for (int i = newIndex; i < oldIndex; i++)
        //  {
        //    // Get the element to move
        //    shiftedElementCopy = MakeCopy(listInstance[i]);
        //    // Cache the element that will be overriden
        //    var tempCopy = MakeCopy(listInstance[i + 1]);
        //    // Override the element
        //    listInstance[i + 1] = shiftedElementCopy;
        //    // Save the next element
        //    shiftedElementCopy = tempCopy;
        //  }
        //  listInstance[newIndex] = oldListElementCopy;

        //listInstance.Insert(newIndex, oldListElementCopy);
        //listInstance.RemoveAt(oldIndex + 1);
      }
      else
      {
        //listInstance.Insert(newIndex + 1, oldListElementCopy);
        //listInstance.RemoveAt(oldIndex);
      }

      var property = list.serializedProperty;
      if (property != null)
      {
        EditorUtility.SetDirty(property.serializedObject.targetObject);
      }
    }

    public static HeaderCallbackDelegate OnDrawHeader(string label)
    {
      HeaderCallbackDelegate action = (rect) =>
      {
        EditorGUI.LabelField(rect, label);
      };
      return action;
    }

    public static object MakeCopy(object source)
    {
      Type type = source.GetType();
      var copy = Activator.CreateInstance(type);
      foreach (var field in ReflectionUtilities.GetSerializableFields(type))
      {
        field.SetValue(copy, field.GetValue(source));
      }
      return copy;
    }

    public static void OnDrawElement(this IList list, Rect rect, int index, bool isActive,
                                     bool isFocused, string labelOverride = "")
    {
      var elementDrawn = list[index];
      Type elementType = list.GetType().GetGenericArguments()[0];
      //Type type = elementDrawn.GetType();

      rect.y += 1.0f;
      rect.x += 10.0f;
      rect.width -= 10.0f;
      var height = EditorGUIUtility.singleLineHeight;
      rect.height = height;

      string displayName = elementType.Name.FormatFieldOrClassName();
      if (elementDrawn != null)
      {
        elementType = elementDrawn.GetType();
        displayName = elementDrawn.GetType().Name.FormatFieldOrClassName();
      }

      #region Class Description Attribute
      var classDescriptionAttribute = (ClassDescriptionAttribute)Attribute.GetCustomAttribute(elementType, typeof(ClassDescriptionAttribute));
      if (classDescriptionAttribute != null)
      {
        displayName = classDescriptionAttribute.GetDescription(elementDrawn);
      }
      #endregion

      if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
      {
        EditorGUI.BeginChangeCheck();
        //var allowSceneObjectsAttribute = (AllowSceneObjectsAttribute)Attribute.GetCustomAttribute(elementType, typeof(AllowSceneObjectsAttribute));
        //bool allowSceneObjects = allowSceneObjectsAttribute != null ? allowSceneObjectsAttribute.allowSceneObjects : false;
        //bool allowSceneObjects = typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(elementType) ? true :
        //                         typeof(UnityEngine.Transform).IsAssignableFrom(elementType) ? true :
        //                         false;
        elementDrawn = EditorGUI.ObjectField(/*new Rect(rect.x, rect.y, rect.width, height)*/rect, string.Empty,
                                            (UnityEngine.Object)elementDrawn, elementType, true);
        if (EditorGUI.EndChangeCheck())
        {
          list[index] = elementDrawn;
        }
      }
      else if (elementType.IsValueType)
      {
        EditorGUI.BeginChangeCheck();
        elementDrawn = EditorUtilities.DrawField(rect, elementType, /*list[index]*/elementDrawn, new GUIContent());
        if (EditorGUI.EndChangeCheck())
        {
          list[index] = elementDrawn;
        }
      }
      else if (typeof(string).IsAssignableFrom(elementType))
      {
        EditorGUI.BeginChangeCheck();
        elementDrawn = EditorGUI.TextField(rect, (string)elementDrawn);
        if (EditorGUI.EndChangeCheck())
        {
          list[index] = elementDrawn;
        }
      }
      else if (elementType.IsClass)
      {
        //var drawElementInlineAttribute = (DrawElementInlineAttribute)Attribute.GetCustomAttribute(elementType, 
        //                                              typeof(DrawElementInlineAttribute));
        //if (drawElementInlineAttribute != null)
        //{
        //  EditorGUILayout.BeginHorizontal();
        //  EditorUtilities.DrawSelectedClassFields(elementDrawn);
        //  EditorGUILayout.EndHorizontal();
        //}
        //else
        {
          EditorGUI.LabelField(/*new Rect(rect.x, rect.y, rect.width, height)*/rect,
                               string.IsNullOrEmpty(labelOverride) ? displayName : labelOverride);
        }
      }
      //else
      //{
      //  EditorUtilities.DrawField(elementDrawn, type, list[index], new GUIContent());
      //}
    }

    public static ElementCallbackDelegate GetDrawElementCallback(SerializedProperty serializedProperty)
    {
      ElementCallbackDelegate elementCallbackDelegate = (Rect rect, int index, bool isActive, bool isFocused) =>
      {
        string propertyName = serializedProperty.name;
        var obj = serializedProperty.serializedObject.targetObject;
        var instance = obj.GetType().GetSerializableField(propertyName).GetValue(obj);
        Type listType = instance.GetType();
        if (listType.IsListType())
        {
          ((IList)instance).OnDrawElement(rect, index, isActive, isFocused);
        }
      };
      return elementCallbackDelegate;
    }

    /// <summary>
    /// Draws fields in line TODO
    /// </summary>
    /// <param name="list"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    public static ElementCallbackDelegate GetDrawElementCallback(IList list, string label)
    {
      ElementCallbackDelegate elementCallbackDelegate = (Rect rect, int index, bool isActive, bool isFocused) =>
      {
        list.OnDrawElement(rect, index, isActive, isFocused, label);
      };
      return elementCallbackDelegate;
    }

    public static GenericMenu CreateAddInstanceOfTypeToListMenu(List<Type> types, IList list)
    {
      GenericMenu menu = new GenericMenu();
      MenuFunction2 onElementClick = (userData) =>
      {
        var data = userData as object[];
        Type type = data[0] as Type;
        IList l = data[1] as IList;
        var instance = Activator.CreateInstance(type);
        l.Add(instance);
      };

      Dictionary<Type, int> order = new Dictionary<Type, int>();

      types.ForEach(t =>
      {
        var menuOrderAttribute = (MenuOrderAttribute)Attribute.GetCustomAttribute(t, typeof(MenuOrderAttribute));
        order.Add(t, menuOrderAttribute != null ? menuOrderAttribute.Order : 0);
      });

      types = types.OrderByDescending(t => order[t]).ToList();

      types.ForEach(t =>
      {
        string menuItemName = string.Empty;
        var menuPathAttribute = (MenuPathAttribute)Attribute.GetCustomAttribute(t, typeof(MenuPathAttribute));
        if (menuPathAttribute != null)
        {
          menuItemName += menuPathAttribute.Path;
        }
        menuItemName += t.Name.FormatFieldOrClassName();
        menu.AddItem(new GUIContent(menuItemName), on: false,
                     onElementClick, new object[] { t, list });
      });
      return menu;
    }
  }
}