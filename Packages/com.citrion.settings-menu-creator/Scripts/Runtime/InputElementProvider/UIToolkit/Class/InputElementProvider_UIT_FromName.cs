using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class InputElementProvider_UIT_FromName : InputElementProvider_UIT
  {
    protected abstract string ProviderName { get; }

    public override IGenericInputElementProvider<VisualElement> GetProvider(SettingsCollection settings)
    {
      if (settings.InputElementProviders_UIT == null) { return null; }
      bool foundProvider = settings.InputElementProviders_UIT.TryGetValue(ProviderName, out var provider);
      if (foundProvider) { return provider; }
      return null;
    }

    public override string Name => $"{ProviderName} (From Name)";

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
