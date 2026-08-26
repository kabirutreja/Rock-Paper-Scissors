using System.Collections.Generic;

namespace CitrioN.SettingsMenuCreator
{
  public interface IGenericInputElementProvider<T> : IInputElementProvider
  {
    public IGenericInputElementProvider<T> GetProvider(SettingsCollection settings);

    public T GetInputElement(string settingIdentifier, SettingsCollection settings);

    public T FindInputElement(T root, string settingIdentifier, SettingsCollection settings);

    public bool UpdateInputElement(T elem, string settingIdentifier,
                                   string labelText, SettingsCollection settings,
                                   List<object> values, bool initialize);
  }
}
