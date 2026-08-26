using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  // TODO Add remaining variables
  public enum UnityVariableName
  {
    // Button
    Button_Background_Color,
    Button_Border_Color,

    // Input Field
    InputField_Background_Color,
    InputField_Border_Color,

    // Dropdown
    Dropdown_Background_Color,
    Dropdown_Border_Color,

    // Help Box
    Helpbox_Background_Color,
    Helpbox_Border_Color,

    // Toolbar
    Toolbar_Background_Color,
    Toolbar_Border_Color,
  }

  public enum VariableType
  {
    Pro = 0,
    Light = 1,
    Runtime = 2
  }

  /// <summary>
  /// Contains some of Unity's uss builtin variables listed here:
  /// https://docs.unity3d.com/2021.3/Documentation/Manual/UIE-uss-built-in-variable-reference.html
  /// </summary>
#if UNITY_EDITOR
  [UnityEditor.InitializeOnLoad]
#endif
  public static class UnityVariablesUtility
  {
    private static bool dictionaryInitialized = false;

    private static Dictionary<UnityVariableName, List<object>> variables =
      new Dictionary<UnityVariableName, List<object>>();

    private static VariableType GetVariableType
    {
      get
      {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
          return VariableType.Runtime;
        }
        else if (UnityEditor.EditorGUIUtility.isProSkin)
        {
          return VariableType.Pro;
        }
        return VariableType.Light;
#else
        return VariableType.Runtime;;
#endif
      }
    }

    private static VariableType GetVariableTypeForEditor
    {
      get
      {
#if UNITY_EDITOR
        return UnityEditor.EditorGUIUtility.isProSkin ? VariableType.Pro : VariableType.Light;
#else
        return VariableType.Runtime;
#endif
      }
    }

    static UnityVariablesUtility()
    {
      dictionaryInitialized = false;
    }

    private static void InitializeDictionary()
    {
      variables = new Dictionary<UnityVariableName, List<object>>() {
        // Button
        { UnityVariableName.Button_Background_Color, new List<object>() { GetColor("#585858"), GetColor("#E4E4E4"), GetColor("#BCBCBC") } },
        { UnityVariableName.Button_Border_Color, new List<object>() { GetColor("#303030"), GetColor("#B2B2B2"), GetColor("#959595") } },

        // Input Field
        { UnityVariableName.InputField_Background_Color, new List<object>() { GetColor("#2A2A2A"), GetColor("#F0F0F0"), GetColor("#F0F0F0") } },
        { UnityVariableName.InputField_Border_Color, new List<object>() { GetColor("#212121"), GetColor("#B7B7B7"), GetColor("#646464") } },

        // Dropdown
        { UnityVariableName.Dropdown_Background_Color, new List<object>() { GetColor("#515151"), GetColor("#DFDFDF"), GetColor("#DFDFDF") } },
        { UnityVariableName.Dropdown_Border_Color, new List<object>() { GetColor("#303030"), GetColor("#B2B2B2"), GetColor("#999999") } },

        // Help Box
        { UnityVariableName.Helpbox_Background_Color, new List<object>() { new Color(96, 96, 96, 0.2039216f), new Color(235, 235, 235, 0.2039216f), null } },
        { UnityVariableName.Helpbox_Border_Color, new List<object>() { GetColor("#232323"), GetColor("#A9A9A9"), null } },

        // Toolbar
        { UnityVariableName.Toolbar_Background_Color, new List<object>() { GetColor("#3C3C3C"), GetColor("#CBCBCB"), null } },
        { UnityVariableName.Toolbar_Border_Color, new List<object>() { GetColor("#232323"), GetColor("#999999"), null } },
      };

      dictionaryInitialized = true;
    }

    private static Color GetColor(string html)
    {
      if (ColorUtility.TryParseHtmlString(html, out var color))
      {
        return color;
      }
      return Color.white;
    }

    public static T GetVariable<T>(UnityVariableName variableName, VariableType variableType)
    {
      GetVariable(variableName, variableType, out T value);
      return value;
    }

    public static bool GetVariable<T>(UnityVariableName variableName, VariableType variableType, out T value)
    {
      if (!dictionaryInitialized) { InitializeDictionary(); }

      if (variables.TryGetValue(variableName, out var list))
      {
        if (list == null)
        {
          value = default;
          return false;
        }

        int index = (int)variableType;

        if (list.Count <= index)
        {
          value = default;
          return false;
        }

        if (list[index] is T val)
        {
          value = val;
          return true;
        }
      }
      value = default;
      return false;
    }

    public static bool GetVariable<T>(UnityVariableName variableName, out T value)
    {
      return GetVariable(variableName, GetVariableType, out value);
    }

    public static T GetVariable<T>(UnityVariableName variableName, bool isEditorWindow)
    {
      var variableType = isEditorWindow ? GetVariableTypeForEditor : GetVariableType;
      GetVariable(variableName, variableType, out T value);
      return value;
    }

    public static bool GetVariable<T>(UnityVariableName variableName, bool isEditorWindow, out T value)
    {
      var variableType = isEditorWindow ? GetVariableTypeForEditor : GetVariableType;
      return GetVariable(variableName, variableType, out value);
    }
  }
}
