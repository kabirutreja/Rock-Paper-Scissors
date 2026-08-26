using System;
using UnityEditor;

namespace CitrioN.Common
{
  public static class DialogUtilities
  {
#if UNITY_EDITOR
    public static void DisplayDialog(string title, string message,
                                 string buttonString, Action action)
    {
      var isTrue = EditorUtility.DisplayDialog(title, message, buttonString);

      if (isTrue) { action?.Invoke(); }
      else { action?.Invoke(); }
    }

    public static void DisplayDialog(string title, string message,
                                     string okString, string cancelString,
                                     Action okAction, Action cancelAction = null)
    {
      var isTrue = EditorUtility.DisplayDialog(title, message, okString, cancelString);

      if (isTrue) { okAction?.Invoke(); }
      else { cancelAction?.Invoke(); }
    }
#endif
  }
}