using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class ScriptableInputElementProvider_UIT_WithStyleSheet_ClassList : ScriptableInputElementProvider_UIT_WithStyleSheet
  {
    [SerializeField]
    [Tooltip("The class names to add to the provided element.")]
    protected List<string> classNames = new List<string>();

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                                string labelText, SettingsCollection settings,
                                                List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      if (initialize && elem != null)
      {
        classNames.ForEach(c => elem.AddToClassList(c));
      }
      return success;
    }
  }
}
