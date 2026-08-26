using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(300)]
  [System.Serializable]
  [MenuPath("Utility/UI Panel")]
  public abstract class Setting_UIPanel : Setting
  {
    public override string EditorNamePrefix => "[UI Panel]";

    public override bool SkipApplyingDefault => true;

    public override object GetDefaultValue(SettingsCollection settings) => null;
  }
}