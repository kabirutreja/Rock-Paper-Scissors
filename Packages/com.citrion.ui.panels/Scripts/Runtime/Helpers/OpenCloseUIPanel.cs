using CitrioN.Common;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Allows the opening or closing of a specified <see cref="AbstractUIPanel"/>.
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Helper script that allows the opening or closing of the referenced panel. " +
              "The 'OpenPanel' and 'ClosePanel' methods can either be called in code or by using Unity events.")]
  public class OpenCloseUIPanel : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The panel to open/close.")]
    protected AbstractUIPanel panel;

    public void OpenPanel()
    {
      if (panel != null) { panel.OpenNoParams(); }
    }

    public void ClosePanel()
    {
      if (panel != null) { panel.CloseNoParams(); }
    }
  }
}