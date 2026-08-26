using CitrioN.UI;
using System.ComponentModel;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [DisplayName("Close Panel (Name)")]
  public class Setting_ClosePanelWithName : Setting_UIPanel
  {
    [SerializeField]
    [Tooltip("The name of the panel to close")]
    protected string panelName;

    public override string EditorName => $"Close Panel With Name: {panelName}";

    public override string RuntimeName => "Close";

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (!string.IsNullOrEmpty(panelName))
      {
        UIPanelManager.ClosePanel(panelName);
      }
      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}