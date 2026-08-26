using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_LoadSettings : Setting_SettingsCollection
  {
    public override string RuntimeName => "Load";

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (settings != null)
      {
        settings.LoadSettings(isDefault: false, apply: true, forceApply: true);
      }

      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}