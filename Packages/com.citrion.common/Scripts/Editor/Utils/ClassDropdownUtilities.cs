using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public static class ClassDropdownUtilities
  {
    public static void ShowClassDropdown(Type[] parentTypes, string displayName,
                                         Action<ClassDropdownItem> onClassItemSelected, Vector2 minimumSize,
                                         string stringToReplace = null, string replacementString = "")
    {
      var dropdown = new ClassDropdown(new AdvancedDropdownState(), parentTypes, displayName,
                                       minimumSize, onClassItemSelected, stringToReplace, replacementString);

      //Rect controlRect = EditorGUILayout.GetControlRect();
      //Rect buttonRect = controlRect;
      //controlRect.width -= 30;
      //buttonRect.xMin = controlRect.xMax + 4;
      //buttonRect.height -= 1;

      var rect = new Rect(Event.current.mousePosition, new Vector2(0, 0));
      //rect.width = 300;
      ////rect.height = 1000;
      ////rect.y -= rect.height;
      //rect.yMax = rect.y + 500;
      dropdown.Show(rect);
    }
  }
}