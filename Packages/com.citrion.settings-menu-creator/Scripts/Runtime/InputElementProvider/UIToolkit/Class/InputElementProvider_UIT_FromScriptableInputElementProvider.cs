using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(50)]
  [System.Serializable]
  [DisplayName("From Scriptable Provider")]
  public class InputElementProvider_UIT_FromScriptableInputElementProvider : InputElementProvider_UIT
  {
    [SerializeField]
    protected ScriptableInputElementProvider_UIT provider;

    public override IGenericInputElementProvider<VisualElement> GetProvider(SettingsCollection settings) => provider;

    // TODO Make this dynamically update when the reference changes?!
    //public override string Name => provider != null ? provider.Name : "n/a";
    public override string Name => "Scriptable Provider";

    public override Type GetInputFieldParameterType(SettingsCollection settings)
      => GetProvider(settings)?.GetInputFieldParameterType(settings);

    public override Type GetInputFieldType(SettingsCollection settings)
      => GetProvider(settings)?.GetInputFieldType(settings);

    public override VisualElement FindInputElement(VisualElement root, string settingIdentifier, SettingsCollection settings)
      => GetProvider(settings).FindInputElement(root, settingIdentifier, settings);

    public override VisualElement GetInputElement(string settingIdentifier, SettingsCollection settings)
      => GetProvider(settings)?.GetInputElement(settingIdentifier, settings);

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
      var provider = GetProvider(settings);
      if (provider == null) { return false; }
      return success && provider.UpdateInputElement(elem, settingIdentifier, labelText,
                                                    settings, values, initialize);
    }
  }
}