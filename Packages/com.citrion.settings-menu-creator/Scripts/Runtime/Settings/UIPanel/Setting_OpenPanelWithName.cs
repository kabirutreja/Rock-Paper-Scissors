using CitrioN.UI;
using System.ComponentModel;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [DisplayName("Open Panel (Name)")]
  public class Setting_OpenPanelWithName : Setting_UIPanel
  {
    [SerializeField]
    [Tooltip("The name of the panel to open")]
    protected string panelName;

    public override string EditorName => $"Open Panel With Name: {panelName}";

    public override string RuntimeName => panelName;

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (!string.IsNullOrEmpty(panelName))
      {
        UIPanelManager.OpenPanel(panelName);
      }
      base.ApplySettingChange(settings, null);
      return null;
    }
  }
}