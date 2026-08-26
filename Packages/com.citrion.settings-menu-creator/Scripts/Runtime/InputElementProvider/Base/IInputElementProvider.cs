using System;

namespace CitrioN.SettingsMenuCreator
{
  public interface IInputElementProvider
  {
    public string Name { get; }

    public Type GetInputFieldParameterType(SettingsCollection settings);

    public Type GetInputFieldType(SettingsCollection settings);
  }
}
