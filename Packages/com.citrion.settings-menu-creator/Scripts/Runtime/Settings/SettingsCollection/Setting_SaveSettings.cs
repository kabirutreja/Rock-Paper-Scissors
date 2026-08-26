using System.ComponentModel;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_SaveSettings : Setting_SettingsCollection
  {
    public override string RuntimeName => "Save";

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (settings != null)
      {
        settings.SaveSettings();
      }
      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}