namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_RevertChanges : Setting_SettingsCollection
  {
    public override string RuntimeName => "Revert";

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      settings.RevertPendingSettingsChanges();
      return base.ApplySettingChange(settings, null);
    }
  }
}