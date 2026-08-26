using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  /// <summary>
  /// Contains utility methods for custom inspectors and editors.
  /// </summary>
  public static class EditorUtilities
  {
    /// <summary>
    /// Draws all fields of the provided object instance
    /// TODO Make this error proof such as providing a primite type?
    /// </summary>
    /// <param name="instance">The object instance to draw the fields for</param>
    public static void DrawSelectedClassFields(object instance)
    {
      //EditorGUI.BeginChangeCheck();

      Type type = instance.GetType();
      GUIStyle style = new GUIStyle(GUI.skin.GetStyle("BoldLabel"));

      // Draw a header label for the class name
      EditorGUILayout.LabelField(type.Name.FormatFieldOrClassName(), style);
      var fields = ReflectionUtilities.GetSerializableFields(type);

      // Check if there are no fields to be drawn
      if (fields.Count() < 1)
      {
        style = new GUIStyle(GUI.skin.GetStyle("Label"));
        style.alignment = TextAnchor.MiddleCenter;
        // Draw a centered label with the information that there are not fields to show
        EditorGUILayout.LabelField("No fields to display", style);
      }

      // Draw a field for each field of the class
      foreach (var field in fields)
      {
        //EditorGUI.BeginChangeCheck();
        DrawField(instance, field);
        //if (EditorGUI.EndChangeCheck())
        //{
        //  field.SetValue(instance, value);
        //}
      }

      //if (EditorGUI.EndChangeCheck())
      //{
      //  if (typeof(UnityEngine.Object).IsAssignableFrom(instance.GetType()))
      //    // TODO Add undo support
      //    Undo.RecordObject((UnityEngine.Object)instance, "Changed something");
      //  //EditorUtility.SetDirty(script);
      //}
    }

    /// <summary>
    /// Draws a field for the object and field provided.
    /// Saves the new value back to the object instance.
    /// </summary>
    /// <param name="instance">The object instance the field is part of</param>
    /// <param name="field">The <see cref="FieldInfo"/> to draw the field for</param>
    public static void DrawField(object instance, FieldInfo field, params AttributeType[] ignoredAttributeTypes)
    {
      if (field == null) { return; }

      //DrawField(GUILayoutUtility.GetLastRect(), instance, field, ignoreFieldAttributes);
      //return;
      SerializedObject so = null;
      if (instance is SerializedObject)
      {
        so = (SerializedObject)instance;
        instance = so.targetObject;
      }

      // Do NOT draw const or readonly fields
      if (field.IsConstantOrReadOnly()) { return; }

      // Get the current value of the field
      var value = instance != null ? field.GetValue(instance) : null;
      // Get the type of the field
      Type type = field.FieldType;
      if (value != null)
      {
        // Use the type of the current value instead of the field value
        type = value.GetType();
      }
      // Create a new content with the formatted field name.
      // This is how fields are shown by unity too.
      var guiContent = new GUIContent(field.Name.FormatFieldOrClassName());

      // Get the first allows field attribute
      var fieldAttributes = field.GetCustomAttributes(typeof(FieldAttribute)).ToList();
      FieldAttribute fieldAttribute = null;
      foreach (var attribute in fieldAttributes)
      {
        // Check if the attribute should be ignored
        if (!ignoredAttributeTypes.Contains(((FieldAttribute)attribute).AttributeType))
        {
          fieldAttribute = (FieldAttribute)attribute;
          break;
        }
      }

      // Check if the field attribute should control the field drawing
      if (fieldAttribute != null)
      {
        // Let the field attribute control the drawing of the field
        fieldAttribute.Draw(instance, field);
        return;
      }

      // Start checking for any value changes
      EditorGUI.BeginChangeCheck();

      #region Draw Field
      if (typeof(UnityEngine.Object).IsAssignableFrom(type))
      {
        type = field.FieldType;
        value = EditorGUILayout.ObjectField(guiContent, (UnityEngine.Object)value, type, false);
      }
      else if (typeof(IList).IsAssignableFrom(type) && !(typeof(string).IsAssignableFrom(type)))
      {
        var list = value as IList;

        var listElementType = list != null ? list.GetType().GetGenericArguments()[0] : type.GetGenericArguments()[0];

        // TODO Check for validity
        ReorderableListUtilities.DrawReorderableList(list, listElementType,
          ReorderableListUtilities.OnDrawHeader(field.Name.FormatFieldOrClassName()),
          ReorderableListUtilities.GetDrawElementCallback(list, string.Empty), null, ReorderableListUtilities.OnReorder,
          ReorderableListUtilities.OnAdd, ReorderableListUtilities.OnRemove, ref ReorderableListUtilities.ilists,
          showClickedElementDetails: listElementType.IsValueType ? false : true);
      }
      //else if (typeof(IDictionary).IsAssignableFrom(type))
      //{
      //  var dictionary = value as IList;

      //  var listElementType = dictionary.GetType().GetGenericArguments()[0];

      //  // TODO Check for validity
      //  ReorderableListUtilities.DrawReorderableList(dictionary, listElementType,
      //    ReorderableListUtilities.OnDrawHeader(field.Name.FormatFieldOrClassName()),
      //    ReorderableListUtilities.GetDrawElementCallback(dictionary, string.Empty), null, ReorderableListUtilities.OnReorder,
      //    ReorderableListUtilities.OnAdd, ReorderableListUtilities.OnRemove, ref ReorderableListUtilities.ilists,
      //    showClickedElementDetails: listElementType.IsValueType ? false : true);
      //}
      else if (type == typeof(LayerMask))
      {
        value = GetLayerMask(EditorGUILayout.MaskField(guiContent, GetLayerMaskValue((LayerMask)value), InternalEditorUtility.layers));
      }
      else if (type == typeof(bool))
      {
        value = EditorGUILayout.Toggle(guiContent, (bool)value);
      }
      else if (type == typeof(byte))
      {
        value = EditorGUILayout.IntField(guiContent, (byte)value);
      }
      else if (type == typeof(int))
      {
        value = EditorGUILayout.IntField(guiContent, (int)value);
      }
      else if (type == typeof(float))
      {
        value = EditorGUILayout.FloatField(guiContent, (float)value);
      }
      else if (type == typeof(double))
      {
        value = EditorGUILayout.DoubleField(guiContent, (double)value);
      }
      else if (type == typeof(long))
      {
        value = EditorGUILayout.LongField(guiContent, (long)value);
      }
      else if (type == typeof(Vector2))
      {
        value = EditorGUILayout.Vector2Field(guiContent, (Vector2)value);
      }
      else if (type == typeof(Vector2Int))
      {
        value = EditorGUILayout.Vector2IntField(guiContent, (Vector2Int)value);
      }
      else if (type == typeof(Vector3))
      {
        value = EditorGUILayout.Vector3Field(guiContent, (Vector3)value);
      }
      else if (type == typeof(Vector3Int))
      {
        value = EditorGUILayout.Vector3IntField(guiContent, (Vector3Int)value);
      }
      else if (type == typeof(Vector4))
      {
        value = EditorGUILayout.Vector4Field(guiContent, (Vector4)value);
      }
      else if (type == typeof(Quaternion))
      {
        value = Quaternion.Euler(EditorGUILayout.Vector3Field(guiContent, ((Quaternion)value).eulerAngles));
      }
      else if (type == typeof(Bounds))
      {
        value = EditorGUILayout.BoundsField(guiContent, (Bounds)value);
      }
      else if (type == typeof(string))
      {
        value = EditorGUILayout.TextField(guiContent, (string)value);
      }
      else if (type.IsEnum)
      {
        value = EditorGUILayout.EnumPopup(guiContent, (Enum)Enum.ToObject(type, value));
      }
      else if (type == typeof(Color))
      {
        value = EditorGUILayout.ColorField(guiContent, (Color)value);
      }
      else if (type == typeof(AnimationCurve))
      {
        value = EditorGUILayout.CurveField(guiContent, (AnimationCurve)value);
      }
      else if (typeof(UnityEvent).IsAssignableFrom(type))
      {
        // TODO
        //EditorGUILayout.PropertyField(instance.FindProperty(field.Name), true);
      }
      else if (type.IsClass/* && value != null*/ && type.GetConstructor(Type.EmptyTypes) != null)
      {
        if (value == null)
        {
          value = Activator.CreateInstance(type);
        }
        EditorGUILayout.BeginVertical("box");
        DrawSelectedClassFields(value);
        EditorGUILayout.EndVertical();
      }
      // TODO This will not handle the case in which the interface is not a Unity Object
      else if (type.IsInterface)
      {
        value = EditorGUILayout.ObjectField(guiContent, (UnityEngine.Object)value, type, true);
      }
      #endregion

      //Check for any changes
      if (EditorGUI.EndChangeCheck())
      {
        // Save the new value back into the variable
        field.SetValue(instance, value);
        if (so != null)
        {
          so.ApplyModifiedProperties();
          Undo.RecordObject(so.targetObject, "Changed field value");
          EditorUtility.SetDirty(so.targetObject);
        }
        //if (typeof(UnityEngine.Object).IsAssignableFrom(instance.GetType()))
        //{
        //  Undo.RecordObject((UnityEngine.Object)instance, "Changed something");
        //}
      }
    }

    /// <summary>
    /// Draws a field for the provided type and value. To save the input from the field
    /// back to the variable <see cref="EditorGUI.BeginChangeCheck()"/> needs to be called 
    /// before calling this method and <see cref="EditorGUI.EndChangeCheck()"/> after.
    /// Check the example for a conrete implementation.
    /// </summary>
    /// <example>
    /// <code>
    /// var rect = new Rect();
    /// Vector2 v = new Vector2();
    /// EditorGUI.BeginChangeCheck();
    /// var newValue = DrawField(rect, v, v.GetType(), v, new GUIContent());
    /// if (EditorGUI.EndChangeCheck()) {
    ///   v = newValue;
    /// }
    /// </code>
    /// </example>
    /// <param name="rect">The <see cref="Rect"/> to draw the field within</param>
    /// <param name="type">The <see cref="Type"/> of the object to draw the field for</param>
    /// <param name="value">The value of the object to draw the field for</param>
    /// <param name="guiContent">The label, tooltip of texture to draw with the field/></param>
    /// <returns>The new value of the object</returns>
    public static object DrawField(Rect rect, Type type, object value, GUIContent guiContent)
    {
      // TODO Check if the field is const or readonly

      if (typeof(UnityEngine.Object).IsAssignableFrom(type))
      {
        value = EditorGUI.ObjectField(rect, guiContent, (UnityEngine.Object)value, type, false);
      }
      else if (typeof(IList).IsAssignableFrom(type))
      {
        var list = value as IList;

        var listElementType = list.GetType().GetGenericArguments()[0];

        // TODO Check for validity
        ReorderableListUtilities.DrawReorderableList(list, listElementType,
          null,
          ReorderableListUtilities.GetDrawElementCallback(list, string.Empty), null, ReorderableListUtilities.OnReorder,
          ReorderableListUtilities.OnAdd, ReorderableListUtilities.OnRemove, ref ReorderableListUtilities.ilists,
          showClickedElementDetails: listElementType.IsValueType ? false : true);
      }
      //else if (typeof(IDictionary).IsAssignableFrom(type))
      //{
      //  var dictionary = value as IList;

      //  var listElementType = dictionary.GetType().GetGenericArguments()[0];

      //  // TODO Check for validity
      //  ReorderableListUtilities.DrawReorderableList(dictionary, listElementType, null,
      //    ReorderableListUtilities.GetDrawElementCallback(dictionary, string.Empty), null, ReorderableListUtilities.OnReorder,
      //    ReorderableListUtilities.OnAdd, ReorderableListUtilities.OnRemove, ref ReorderableListUtilities.ilists,
      //    showClickedElementDetails: listElementType.IsValueType ? false : true);
      //}
      else if (type == typeof(bool))
      {
        value = EditorGUI.Toggle(rect, guiContent, (bool)value);
      }
      else if (type == typeof(byte))
      {
        value = EditorGUI.IntField(rect, guiContent, (byte)value);
      }
      else if (type == typeof(int))
      {
        value = EditorGUI.IntField(rect, guiContent, (int)value);
      }
      else if (type == typeof(float))
      {
        value = EditorGUI.FloatField(rect, guiContent, (float)value);
      }
      else if (type == typeof(double))
      {
        value = EditorGUI.DoubleField(rect, guiContent, (double)value);
      }
      else if (type == typeof(long))
      {
        value = EditorGUI.LongField(rect, guiContent, (long)value);
      }
      else if (type == typeof(Vector2))
      {
        value = EditorGUI.Vector2Field(rect, guiContent, (Vector2)value);
      }
      else if (type == typeof(Vector2Int))
      {
        value = EditorGUI.Vector2IntField(rect, guiContent, (Vector2Int)value);
      }
      else if (type == typeof(Vector3))
      {
        value = EditorGUI.Vector3Field(rect, guiContent, (Vector3)value);
      }
      else if (type == typeof(Vector3Int))
      {
        value = EditorGUI.Vector3IntField(rect, guiContent, (Vector3Int)value);
      }
      else if (type == typeof(Vector4))
      {
        value = EditorGUI.Vector4Field(rect, guiContent, (Vector4)value);
      }
      else if (type == typeof(Quaternion))
      {
        value = Quaternion.Euler(EditorGUI.Vector3Field(rect, guiContent, ((Quaternion)value).eulerAngles));
      }
      else if (type == typeof(Bounds))
      {
        value = EditorGUI.BoundsField(rect, guiContent, (Bounds)value);
      }
      else if (type == typeof(string))
      {
        value = EditorGUI.TextField(rect, guiContent, (string)value);
      }
      else if (type.IsEnum)
      {
        value = EditorGUI.EnumPopup(rect, guiContent, (Enum)Enum.ToObject(type, value));
      }
      else if (type == typeof(Color))
      {
        value = EditorGUI.ColorField(rect, guiContent, (Color)value);
      }
      else if (type == typeof(AnimationCurve))
      {
        value = EditorGUI.CurveField(rect, guiContent, (AnimationCurve)value);
      }
      else if (type.IsClass)
      {
        // TODO
      }

      return value;
    }

    private static LayerMask GetLayerMask(int maskValue)
    {
      LayerMask layerMask = 0;
      var layerNames = InternalEditorUtility.layers;
      // Iterate over all layers
      for (int layer = 0; layer < layerNames.Length; layer++)
      {
        // Check if the layer should be in the mask value
        if ((maskValue & (1 << layer)) != 0)
        {
          // Get the value of the current layer
          int currentLayerMaskValue = LayerMask.NameToLayer(layerNames[layer]);

          // Add the layer value to the layer mask
          layerMask |= 1 << currentLayerMaskValue;
        }
      }
      return layerMask;
    }

    private static int GetLayerMaskValue(LayerMask layerMask)
    {
      int maskValue = 0;
      var layerNames = InternalEditorUtility.layers;
      // Iterate over all layers
      for (int layer = 0; layer < layerNames.Length; layer++)
      {
        // Get the mask value of the current layer
        int currentLayerMaskValue = LayerMask.NameToLayer(layerNames[layer]);

        // Check if the current layer should be in the layer mask
        if ((layerMask & (1 << currentLayerMaskValue)) != 0)
        {
          // Add the layer value to the mask value
          maskValue |= 1 << layer;
        }
      }
      return maskValue;
    }

    /// <summary>
    /// Opens a class dropdown menu. Invokes an action for the selected class type when clicked.
    /// </summary>
    /// <param name="findDerivedTypes">Should derived classes be included?</param>
    /// <param name="onClassItemSelected">The action to invoke on the clicked class item</param>
    /// <param name="label">The label for the menu</param>
    /// <param name="types">The types to show in the menu</param>
    public static void OpenAdvancedDropdownToAssignTypeInstance
           (bool findDerivedTypes, Action<ClassDropdownItem> onClassItemSelected, string label = "",
            string stringToReplace = null, string replacementString = "", params Type[] types)
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

      ClassDropdownUtilities.ShowClassDropdown(types, label, onClassItemSelected,
                             new Vector2(280, 260), stringToReplace, replacementString);
    }

    /// <summary>
    /// Opens a class dropdown menu. 
    /// Assigns a new instance of the clicked class to the provided field.
    /// </summary>
    /// <param name="obj">The object containing the field to assign the instance to</param>
    /// <param name="field">The field to assign the instance to</param>
    /// <param name="findDerivedTypes">Should derived classes be included?</param>
    /// <param name="label">The label for the menu</param>
    public static void AssignInstanceOfTypeToFieldFromAdvancedDropdown
          (object obj, FieldInfo field, bool findDerivedTypes, Type type,
           string label = "", string stringToReplace = null, string replacementString = "")
    {
      AssignInstanceOfTypeToFieldFromAdvancedDropdown(obj, field, findDerivedTypes, label,
                                  stringToReplace, replacementString, new Type[] { type });
    }

    /// <summary>
    /// Opens a class dropdown menu. 
    /// Assigns a new instance of the clicked class to the provided field.
    /// </summary>
    /// <param name="obj">The object containing the field to assign the instance to</param>
    /// <param name="field">The field to assign the instance to</param>
    /// <param name="findDerivedTypes">Should derived classes be included?</param>
    /// <param name="label">The label for the menu</param>
    /// <param name="types">The types to show in the menu</param>
    public static void AssignInstanceOfTypeToFieldFromAdvancedDropdown
          (object obj, FieldInfo field, bool findDerivedTypes, string label = "",
           string stringToReplace = null, string replacementString = "", params Type[] types)
    {
      Action<ClassDropdownItem> onClassItemSelected = null;
      onClassItemSelected += (c) =>
      {
        var instance = Activator.CreateInstance(c.Type);

        // Check if a serialized object was provided instead of the actual object

        bool isList = field.FieldType.IsListType();

        if (isList)
        {
          if (obj is SerializedObject so)
          {
            obj = so.targetObject;
          }
          //var enumerator = field.GetValue(obj) as IList
          //object value = field.GetValue(obj);
          //var listType = typeof(List<>).MakeGenericType(new Type[] { c.Type });
          //IList list = (IList)Activator.CreateInstance(listType);
          //var list = (IList)Activator.CreateInstance(c.Type);
          //list.Add(instance);
          //field.SetValue(obj, list);

          //Type type = typeof(CommandData);
          //var listType = typeof(List<>);
          //Type[] listParam = { type };
          //object Result = Activator.CreateInstance(listType.MakeGenericType(listParam));
          //Result.GetType().GetMethod("Add").Invoke(Result, new[] { instance });
        }
        else
        {
          if (obj is SerializedObject so)
          {
            field.SetValue(so.targetObject, instance);
            so.ApplyModifiedProperties();
            Undo.RecordObject(so.targetObject, "Added class");
            EditorUtility.SetDirty(so.targetObject);
          }
          else
          {
            field.SetValue(obj, instance);
          }
        }

        //if (obj is SerializedObject so)
        //{
        //  field.SetValue(so.targetObject, instance);
        //  //EditorUtility.SetDirty(so.targetObject);
        //  //so.Update();
        //  //so.UpdateIfRequiredOrScript();
        //  //so.ApplyModifiedProperties();
        //  //so.SetIsDifferentCacheDirty();
        //  so.ApplyModifiedProperties();
        //  Undo.RecordObject(so.targetObject, "Added class");
        //  EditorUtility.SetDirty(so.targetObject);
        //  //Debug.Log(so.hasModifiedProperties);
        //}
        //else
        //{
        //  field.SetValue(obj, instance);
        //}
        // TODO Check if the object needs to be marked dirty for the change to be saveable
      };

      OpenAdvancedDropdownToAssignTypeInstance(findDerivedTypes, onClassItemSelected, label,
                                               stringToReplace, replacementString, types);
    }

    public static void GetInstanceOfTypeFromAdvancedDropdown
      (bool findDerivedTypes, Action<object> onItemSelected, string label = "",
       string stringToReplace = null, string replacementString = "", params Type[] types)
    {
      Action<ClassDropdownItem> onClassItemSelected = null;
      onClassItemSelected += (c) =>
      {
        if (c.Type.IsAssignableFrom(typeof(ScriptableObject)))
        {
          Debug.Log("SO");
        }
        var instance = Activator.CreateInstance(c.Type);

        if (onItemSelected != null)
        {
          onItemSelected(instance);
        }
      };

      OpenAdvancedDropdownToAssignTypeInstance(findDerivedTypes, onClassItemSelected, label,
                                               stringToReplace, replacementString, types);
    }

#if UNITY_2021_1_OR_NEWER
    [MenuItem("Tools/CitrioN/Utility/Recompile Scripts")]
    public static void RecompileScripts()
    {
      CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
    }
#endif

    [MenuItem("Tools/CitrioN/Utility/Lock Assemblies Reload")]
    public static void LockReloadAssemblies()
    {
      EditorApplication.LockReloadAssemblies();
    }

    [MenuItem("Tools/CitrioN/Utility/Unlock Assemblies Reload")]
    public static void UnlockReloadAssemblies()
    {
      EditorApplication.UnlockReloadAssemblies();
    }

    [MenuItem("Tools/CitrioN/Utility/Restart Editor")]
    public static void RestartEditor()
    {
      string projectPath = Application.dataPath.Replace("/Assets", "");

      EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
      AssetDatabase.SaveAssets();
      EditorApplication.OpenProject(projectPath);
    }

    //public static void DrawField(Rect position, object instance, FieldInfo field, bool ignoreFieldAttributes = false)
    //{
    //  // Start checking for any value changes
    //  EditorGUI.BeginChangeCheck();

    //  // Get the current value of the field
    //  var value = field.GetValue(instance);
    //  // Get the type of the field
    //  Type type = field.FieldType;
    //  // Create a new content with the formatted field name.
    //  // This is how fields are shown by unity too.
    //  var guiContent = new GUIContent(FormatFieldOrClassName(field.Name));

    //  #region Draw Field
    //  // Get the first FieldAttribute
    //  FieldAttribute fieldAttribute = ignoreFieldAttributes ? null : (FieldAttribute)field.GetCustomAttribute(typeof(FieldAttribute));

    //  // Check if the field attribute should control the field drawing
    //  if (fieldAttribute != null)
    //  {
    //    // Let the field attribute control the drawing of the field
    //    fieldAttribute.Draw(instance, field);
    //  }
    //  else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
    //  {
    //    value = EditorGUI.ObjectField(position, guiContent, (UnityEngine.Object)value, type, false);
    //  }
    //  else if (typeof(IList).IsAssignableFrom(type))
    //  {
    //    var list = value as IList;

    //    var listElementType = list.GetType().GetGenericArguments()[0];

    //    // TODO Check for validity
    //    ReorderableListUtilities.DrawReorderableList(list, listElementType,
    //      ReorderableListUtilities.OnDrawHeader(FormatFieldOrClassName(field.Name)),
    //      ReorderableListUtilities.GetDrawElementCallback(list, string.Empty), null, ReorderableListUtilities.OnReorder,
    //      ReorderableListUtilities.OnAdd, ReorderableListUtilities.OnRemove, ref ReorderableListUtilities.ilists, null,
    //      showClickedElementDetails: listElementType.IsValueType ? false : true);
    //  }
    //  else if (type == typeof(bool))
    //  {
    //    value = EditorGUI.Toggle(position, guiContent, (bool)value);
    //  }
    //  else if (type == typeof(byte))
    //  {
    //    value = EditorGUI.IntField(position, guiContent, (byte)value);
    //  }
    //  else if (type == typeof(int))
    //  {
    //    value = EditorGUI.IntField(position, guiContent, (int)value);
    //  }
    //  else if (type == typeof(float))
    //  {
    //    value = EditorGUI.FloatField(position, guiContent, (float)value);
    //  }
    //  else if (type == typeof(double))
    //  {
    //    value = EditorGUI.DoubleField(position, guiContent, (double)value);
    //  }
    //  else if (type == typeof(long))
    //  {
    //    value = EditorGUI.LongField(position, guiContent, (long)value);
    //  }
    //  else if (type == typeof(Vector2))
    //  {
    //    value = EditorGUI.Vector2Field(position, guiContent, (Vector2)value);
    //  }
    //  else if (type == typeof(Vector2Int))
    //  {
    //    value = EditorGUI.Vector2IntField(position, guiContent, (Vector2Int)value);
    //  }
    //  else if (type == typeof(Vector3))
    //  {
    //    value = EditorGUI.Vector3Field(position, guiContent, (Vector3)value);
    //  }
    //  else if (type == typeof(Vector3Int))
    //  {
    //    value = EditorGUI.Vector3IntField(position, guiContent, (Vector3Int)value);
    //  }
    //  else if (type == typeof(Vector4))
    //  {
    //    value = EditorGUI.Vector4Field(position, guiContent, (Vector4)value);
    //  }
    //  else if (type == typeof(string))
    //  {
    //    value = EditorGUI.TextField(position, guiContent, (string)value);
    //  }
    //  else if (type.IsEnum)
    //  {
    //    value = EditorGUI.EnumPopup(position, guiContent, (Enum)Enum.ToObject(type, value));
    //  }
    //  else if (type.IsClass)
    //  {
    //    // TODO
    //  }
    //  #endregion

    //  // Check for any changes
    //  if (EditorGUI.EndChangeCheck())
    //  {
    //    // Save the new value back into the variable
    //    field.SetValue(instance, value);
    //  }

    //  return;

    //  //// Start checking for any value changes
    //  //EditorGUI.BeginChangeCheck();

    //  //// Get the current value of the field
    //  //var value = field.GetValue(instance);
    //  //// Get the type of the field
    //  //Type type = field.FieldType;
    //  //// Create a new content with the formatted field name.
    //  //// This is how fields are shown by unity too.
    //  //var guiContent = new GUIContent(FormatFieldOrClassName(field.Name));

    //  ////var guiContent = new GUIContent(field.Name);
    //  //position.y += EditorGUIUtility.singleLineHeight;
    //  //if (typeof(UnityEngine.Object).IsAssignableFrom(type))
    //  //{
    //  //  return EditorGUI.ObjectField(position, guiContent, (UnityEngine.Object)value, type, false);
    //  //}
    //  //if (type == typeof(GameObject) || typeof(ScriptableObject).IsAssignableFrom(type))
    //  //{
    //  //  return EditorGUI.ObjectField(position, guiContent, (UnityEngine.Object)value, type, false);
    //  //}
    //  //if (type == typeof(bool))
    //  //{
    //  //  return EditorGUI.Toggle(position, guiContent, (bool)value);
    //  //}
    //  //if (type == typeof(byte))
    //  //{
    //  //  return EditorGUI.IntField(position, guiContent, (byte)value);
    //  //}
    //  //if (type == typeof(int))
    //  //{
    //  //  return EditorGUI.IntField(position, guiContent, (int)value);
    //  //}
    //  //if (type == typeof(float))
    //  //{
    //  //  return EditorGUI.FloatField(position, guiContent, (float)value);
    //  //}
    //  //if (type == typeof(double))
    //  //{
    //  //  return EditorGUI.DoubleField(position, guiContent, (double)value);
    //  //}
    //  //if (type == typeof(long))
    //  //{
    //  //  return EditorGUI.LongField(position, guiContent, (long)value);
    //  //}
    //  //if (type == typeof(Vector2))
    //  //{
    //  //  return EditorGUI.Vector2Field(position, guiContent, (Vector2)value);
    //  //}
    //  //if (type == typeof(Vector2Int))
    //  //{
    //  //  return EditorGUI.Vector2IntField(position, guiContent, (Vector2Int)value);
    //  //}
    //  //if (type == typeof(Vector3))
    //  //{
    //  //  return EditorGUI.Vector3Field(position, guiContent, (Vector3)value);
    //  //}
    //  //if (type == typeof(Vector3Int))
    //  //{
    //  //  return EditorGUI.Vector3IntField(position, guiContent, (Vector3Int)value);
    //  //}
    //  //if (type == typeof(Vector4))
    //  //{
    //  //  return EditorGUI.Vector4Field(position, guiContent, (Vector4)value);
    //  //}
    //  //if (type == typeof(string))
    //  //{
    //  //  return EditorGUI.TextField(position, guiContent, (string)value);
    //  //}
    //  //if (type.IsEnum)
    //  //{
    //  //  return EditorGUI.EnumPopup(position, guiContent, (Enum)Enum.ToObject(type, value));
    //  //}
    //  //if (type == typeof(GameObject))
    //  //{
    //  //  return EditorGUI.ObjectField(position, guiContent, (GameObject)value, type, false);
    //  //}
    //  //if (typeof(ScriptableObject).IsAssignableFrom(type))
    //  //{
    //  //  return EditorGUI.ObjectField(position, guiContent, (ScriptableObject)value, type, false);
    //  //}
    //  //return value;
    //}

    //public static void DrawSelectedClassFields(ref Rect startPosition, object instance, UnityEngine.Object script)
    //{
    //  EditorGUI.BeginChangeCheck();
    //  EditorGUILayout.LabelField(instance.GetType().Name);
    //  var fields = instance.GetType().GetFields();
    //  foreach (var field in fields)
    //  {
    //    EditorGUI.BeginChangeCheck();
    //    var value = field.GetValue(instance);
    //    Type type = field.FieldType;
    //    if (typeof(ScriptableObject).IsAssignableFrom(type) || type == typeof(GameObject))
    //    {
    //      //value = EditorGUILayout.ObjectField(new GUIContent(field.Name), (UnityEngine.Object)value, type, false);
    //      value = DrawField(startPosition, instance, field,);
    //    }
    //    else
    //    {
    //      value = DrawField(startPosition, instance, field);
    //    }

    //    if (EditorGUI.EndChangeCheck())
    //    {
    //      field.SetValue(instance, value);
    //    }
    //  }

    //  if (EditorGUI.EndChangeCheck())
    //  {
    //    //Undo.RecordObject(target, "Changed");
    //    EditorUtility.SetDirty(script);
    //  }
    //}
    //public static void SetFieldValue(object instance, FieldInfo field, string value)
    //{
    //  Type fieldType = field.FieldType;

    //  if ((typeof(ScriptableObject).IsAssignableFrom(fieldType)))
    //  {
    //    field.SetValue(instance, value);
    //  }
    //  else if (fieldType == typeof(GameObject))
    //  {
    //    field.SetValue(instance, value);
    //  }
    //  if (fieldType == typeof(bool))
    //  {
    //    field.SetValue(instance, Convert.ToBoolean(value));
    //  }
    //  else if (fieldType == typeof(byte))
    //  {
    //    field.SetValue(instance, Convert.ToByte(value));
    //  }
    //  else if (fieldType == typeof(int))
    //  {
    //    field.SetValue(instance, Convert.ToInt32(value));
    //  }
    //  else if (fieldType == typeof(float))
    //  {
    //    field.SetValue(instance, Convert.ToSingle(value));
    //  }
    //  else if (fieldType == typeof(double))
    //  {
    //    field.SetValue(instance, Convert.ToDouble(value));
    //  }
    //  else if (fieldType == typeof(long))
    //  {
    //    // TODO
    //  }

    //  //else if (fieldType == typeof(Vector2))
    //  //{
    //  //  field.SetValue(instance, (Vector2)value);
    //  //}
    //  //else if (fieldType == typeof(Vector2Int))
    //  //{
    //  //  return EditorGUILayout.Vector2IntField(guiContent, (Vector2Int)value);
    //  //}
    //  //else if (fieldType == typeof(Vector3))
    //  //{
    //  //  return EditorGUILayout.Vector3Field(guiContent, (Vector3)value);
    //  //}
    //  //else if (fieldType == typeof(Vector3Int))
    //  //{
    //  //  return EditorGUILayout.Vector3IntField(guiContent, (Vector3Int)value);
    //  //}
    //  //else if (fieldType == typeof(Vector4))
    //  //{
    //  //  return EditorGUILayout.Vector4Field(guiContent, (Vector4)value);
    //  //}

    //  else if (fieldType.IsEnum)
    //  {
    //    field.SetValue(instance, Enum.Parse(fieldType, value));
    //  }
    //}
    //public static FieldInfo GetFieldFromProperty(SerializedProperty serializedProperty)
    //{
    //  var targetObject = serializedProperty.serializedObject.targetObject;
    //  return targetObject.GetType().GetField(serializedProperty.name);
    //}

    public static FieldInfo GetPropertyField(SerializedProperty property)
    {
      Type objectType = property.serializedObject.targetObject.GetType();
      string fieldName = property.propertyPath.Split('.')[0];
      return objectType.GetSerializableField(fieldName);
    }

    public static object GetPropertyValue(SerializedProperty property)
    {
      object obj = property.serializedObject.targetObject;
      FieldInfo field = null;

      //"Array.data"
      var splitPaths = property.propertyPath.Split('.');

      for (int i = 0; i < splitPaths.Length; i++)
      {
        var path = splitPaths[i];
        var type = obj.GetType();

        if (path == "Array" && splitPaths.Length > i + 1 && splitPaths[i + 1].StartsWith("data["))
        {
          //field = type.GetField(path);
          //obj = field.GetValue(obj);
          var list = obj as IList;
          //ConsoleLogger.Log(field.FieldType.ToString());
          var indexString = splitPaths[i + 1];
          var truncatedIndexString = indexString.Substring(5, indexString.Length - 6);
          if (int.TryParse(truncatedIndexString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
          {
            obj = list[index];
            i += 1;
          }
        }
        //if (field.FieldType.IsArray)
        //{

        //  else
        //  {
        //    return null;
        //  }
        //}
        else
        {
          //field = type.GetField(path);
          field = type.GetSerializableField(path);
          obj = field.GetValue(obj);
        }
      }
      return obj;
    }

    public static bool GetPropertyValue<T>(SerializedProperty property, out T value)
    {
      object objValue = GetPropertyValue(property);

      // TODO Check if the null check is required
      if (/*objValue != null && */objValue is T)
      {
        value = (T)objValue;
        return true;
      }

      value = default(T);
      return false;
    }

    public static Type GetSerializedPropertyType(SerializedProperty property)
    {
      return GetPropertyField(property).FieldType;
      //if (fieldType.IsArray)
      //{
      //  fieldType = fieldType.GetElementType();
      //}
      //else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
      //{
      //  fieldType = fieldType.GetGenericArguments()[0];
      //}
    }

    public static string GetActiveFolderPath()
    {
      Type projectWindowUtilType = typeof(ProjectWindowUtil);
      var path = Convert.ToString(projectWindowUtilType.
        GetMethod("GetActiveFolderPath",
        BindingFlags.NonPublic | BindingFlags.Static).
        Invoke(null, null));
      // We need this additional check with the selected object
      // because the code above does not properly work for the
      // 'One Column Layout' of the project window.
      if (path == "Assets")
      {
        var selectedObject = Selection.activeObject;
        if (selectedObject != null)
        {
#if UNITY_6000_3_OR_NEWER
          path = AssetDatabase.GetAssetPath(selectedObject);
#else
          path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
#endif
          bool isAsset = Path.HasExtension(path);
          if (isAsset)
          {
            var lastSlash = path.LastIndexOf('/');
            path = path.Substring(0, lastSlash);
          }
        }
      }
      return path;
    }

    public static void PingObject(UnityEngine.Object obj)
    {
      EditorGUIUtility.PingObject(obj);
    }

    public static void PingObjectAtPath(string path)
    {
      if (Directory.Exists(path))
      {
        var obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
      }
    }

    public static void CopyStringToClipboard(string input)
    {
      EditorGUIUtility.systemCopyBuffer = input;
    }

    public static bool IsDarkSkin => EditorGUIUtility.isProSkin;
  }
}