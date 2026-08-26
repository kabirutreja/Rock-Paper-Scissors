using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(200)]
  [System.Serializable]
  [MenuPath("Utility/Settings Collection")]
  public abstract class Setting_SettingsCollection : Setting
  {
    public override string EditorNamePrefix => "[Settings]";

    public override bool SkipApplyingDefault => true;

    public override object GetDefaultValue(SettingsCollection settings) => null;
  }
}