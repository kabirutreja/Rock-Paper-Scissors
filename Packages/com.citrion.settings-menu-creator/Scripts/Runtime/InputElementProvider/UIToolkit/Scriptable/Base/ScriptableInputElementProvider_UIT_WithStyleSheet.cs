using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class ScriptableInputElementProvider_UIT_WithStyleSheet : ScriptableInputElementProvider_UIT
  {
    [SerializeField]
    [Tooltip("The style sheets to add to the provided element.")]
    protected List<StyleSheet> styleSheets = new List<StyleSheet>();

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                                string labelText, SettingsCollection settings,
                                                List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      if (initialize) { elem?.AddStyleSheets(styleSheets); }
      return success;
    }
  }
}
