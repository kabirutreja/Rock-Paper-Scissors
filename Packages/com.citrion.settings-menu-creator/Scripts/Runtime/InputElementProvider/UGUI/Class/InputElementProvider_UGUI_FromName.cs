using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class InputElementProvider_UGUI_FromName : InputElementProvider_UGUI
  {
    protected abstract string ProviderName { get; }

    public override IGenericInputElementProvider<RectTransform> GetProvider(SettingsCollection settings)
    {
      if (settings.InputElementProviders_UGUI == null) { return null; }
      bool foundProvider = settings.InputElementProviders_UGUI.TryGetValue(ProviderName, out var provider);
      if (foundProvider) { return provider; }
      return null;
    }

    public override string Name => ProviderName;

    public override Type GetInputFieldParameterType(SettingsCollection settings)
      => GetProvider(settings)?.GetInputFieldParameterType(settings);

    public override Type GetInputFieldType(SettingsCollection settings)
      => GetProvider(settings)?.GetInputFieldType(settings);

    public override RectTransform FindInputElement(RectTransform root, string settingIdentifier, SettingsCollection settings)
      => GetProvider(settings).FindInputElement(root, settingIdentifier, settings);

    public override RectTransform GetInputElement(string settingIdentifier, SettingsCollection settings)
      => GetProvider(settings)?.GetInputElement(settingIdentifier, settings);

    public override bool UpdateInputElement(RectTransform elem, string settingIdentifier,
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