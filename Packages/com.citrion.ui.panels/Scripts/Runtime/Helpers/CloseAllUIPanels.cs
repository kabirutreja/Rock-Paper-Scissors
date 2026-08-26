using CitrioN.Common;
using UnityEngine;

namespace CitrioN.UI
{
  [HideScriptField]
  [HeaderInfo("Helper script that can close all panels via the 'UI Panel Manager'. " +
              "The 'CloseAllPanels' method can be called in code or Unity event.")]
  public class CloseAllUIPanels : MonoBehaviour
  {
    [SkipObfuscationRename]
    public void CloseAllPanels()
    {
      UIPanelManager.CloseAllPanels();
    }
  }
}