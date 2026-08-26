namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_ResetToDefault : Setting_SettingsCollection
  {
    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (settings != null)
      {
        settings.ResetToDefaultSettings();
      }

      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}