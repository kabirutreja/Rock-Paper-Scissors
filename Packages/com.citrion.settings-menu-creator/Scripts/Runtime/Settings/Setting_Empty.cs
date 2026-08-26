using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(0)]
  [System.Serializable]
  public class Setting_Empty : Setting
  {
    public override object GetDefaultValue(SettingsCollection settings) => null;
  }
}