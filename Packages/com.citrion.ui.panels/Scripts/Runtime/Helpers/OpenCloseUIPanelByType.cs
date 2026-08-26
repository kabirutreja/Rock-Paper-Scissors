using CitrioN.Common;
using System;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Allows the opening or closing of an <see cref="AbstractUIPanel"/> with the
  /// type of <see cref="panelTypeReference"/> and an optional panel name.
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Helper script that allows the opening or closing or a panel with the specified type (and name). " +
              "The 'OpenPanel' and 'ClosePanel' methods can either be called in code or by using Unity events.")]
  public class OpenCloseUIPanelByType : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField]
    [Tooltip("Represents the type of the panel to open or close. " +
             "The referenced panel is not necessarily the panel that will be opened or closed. " +
             "Implemented like this to allow the referencing of prefabs " +
             "to be used to determine the type instead of using " +
             "the fully qualified class name.")]
    protected AbstractUIPanel panelTypeReference;

    [SerializeField]
    [Tooltip("Whether derived types of the specified type should also be valid.")]
    protected bool includeDerivedTypes = false;

    [SerializeField]
    [Tooltip("Name of the panel to open. If specified only a panel " +
             "matching the type and name will be valid.")]
    protected string panelName = string.Empty;

    protected AbstractUIPanel GetPanel()
    {
      Type panelType = panelTypeReference == null ? 
                       typeof(AbstractUIPanel) : panelTypeReference.GetType();

      AbstractUIPanel panel = null;

      // Check if no panel name was specified
      if (string.IsNullOrEmpty(panelName))
      {
        // Get the first panel of the desired type
        panel = UIPanelManager.GetPanel(panelType, includeDerivedTypes);
      }
      else
      {
        // Get all panels of the desired type
        var panels = UIPanelManager.GetPanels(panelType, includeDerivedTypes);
        // Check if no valid panels where found
        if (panels == null || panels.Count < 1) { return null; }
        // Get the first panel with the desired panel name
        panel = panels.Find(p => p.PanelName == panelName);
      }

      return panel;
    }

    public void OpenPanel()
    {
      var panel = GetPanel();
      if (panel != null) { panel.OpenNoParams(); }
    }

    public void ClosePanel()
    {
      var panel = GetPanel();
      if (panel != null) { panel.CloseNoParams(); }
    }
  }
}