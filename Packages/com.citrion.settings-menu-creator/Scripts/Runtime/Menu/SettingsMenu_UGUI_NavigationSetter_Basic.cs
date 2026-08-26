using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [HeaderInfo("\n\nThe navigation will be updated so the next input element " +
              "can be selected with a down input and the previous with an up input.")]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Navigation/Navigation Setter (UGUI) - Basic")]
  public class SettingsMenu_UGUI_NavigationSetter_Basic : SettingsMenu_UGUI_NavigationSetter
  {
    protected override Selectable GetNextInputElement(SettingsMenu_UGUI menu, Selectable origin,
      List<Selectable> inputElements, Dictionary<Selectable, RectTransform> selectableRoots, bool next = true)
    {
      if (origin == null) { return null; }
      var index = inputElements.IndexOf(origin);
      if (index < 0) { return null; }
      Selectable nextElement = null;
      var increment = next ? 1 : -1;
      var elementsCount = inputElements.Count;
      var maxAttempts = inputElements.Count;
      var attempts = 0;
      string settingParent = GetSettingsParent(origin, selectableRoots, menu);
      if (string.IsNullOrEmpty(settingParent)) { return null; }
      bool foundNextElement = false;

      do
      {
        attempts++;
        index = index + increment;

        nextElement = index >= 0 && index < inputElements.Count ? inputElements[index] : null;

        foundNextElement = nextElement != null && nextElement.gameObject.activeInHierarchy &&
                           nextElement.IsInteractable();// &&
                                                        //settingParent == GetSettingsParent(nextElement, selectableRoots, menu);
      } while (attempts <= maxAttempts && !foundNextElement);

      return foundNextElement ? nextElement : null;
    }
  }
}