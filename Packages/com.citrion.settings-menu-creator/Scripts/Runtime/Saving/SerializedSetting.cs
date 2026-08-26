using CitrioN.Common;
using System;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscation]
  [System.Serializable]
  public class SerializedSetting
  {
    private string key;
    private string type;
    private string value;

    public string Key { get => key; set => key = value; }

    public string Value { get => value; set => this.value = value; }

    public string Type { get => type; set => type = value; }

    public SerializedSetting() { }

    public SerializedSetting(string key, Type type, string value)
    {
      this.key = key;
      this.type = type.AssemblyQualifiedName;
      this.value = value;
    }
  }
}