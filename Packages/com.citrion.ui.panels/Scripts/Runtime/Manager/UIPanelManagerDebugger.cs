using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Script to debug/visualize the static variables of the <see cref="UIPanelManager"/>.
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Debugger for the 'UI Panel Manager' to show enabled and open panels.")]
  public class UIPanelManagerDebugger : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Whether the debugger should synchronize its variables during Update with the UIPanelManager?")]
    private bool synchronizeInUpdate = true;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    [Tooltip("All UIPanels that are currently enabled.")]
    private List<AbstractUIPanel> enabledPanels = new List<AbstractUIPanel>();

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    [Tooltip("All UIPanels that are currently open.")]
    private List<AbstractUIPanel> openPanels = new List<AbstractUIPanel>();

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    [Tooltip("True if any panel is currently open.")]
    private bool isAnyPanelOpen = false;

    private void Update()
    {
      if (synchronizeInUpdate == false) { return; }

      enabledPanels = UIPanelManager.EnabledPanels;
      openPanels = UIPanelManager.OpenPanels;
      isAnyPanelOpen = UIPanelManager.AnyPanelOpen;
    }
  }
}