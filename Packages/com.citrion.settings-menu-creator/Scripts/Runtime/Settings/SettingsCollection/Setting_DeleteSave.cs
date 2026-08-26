namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_DeleteSave : Setting_SettingsCollection
  {
    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (settings != null)
      {
        settings.DeleteSave();
      }

      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}