using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the invokation of action(s) when the specified panel " +
              "is opened or closed.")]
  public class UIPanelListener_OnOpenedClosed : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The panel reference for which to listen for events. " +
             "If left empty a panel on a parent will be searched.")]
    protected AbstractUIPanel panel = null;

    [SerializeField]
    [Tooltip("Action(s) to invoke when the OnPanelOpen event " +
             "of the specified panel is invoked.")]
    protected UnityEvent onPanelOpened = new UnityEvent();
    [SerializeField]
    [Tooltip("Action(s) to invoke when the OnPanelClose event " +
             "of the specified panel is invoked.")]
    protected UnityEvent onPanelClosed = new UnityEvent();

    private void CachePanel()
    {
      if (panel == null)
      {
        // TODO Should this also check in children?
        panel = GetComponentInParent<AbstractUIPanel>();
      }
    }

    private void OnEnable()
    {
      CachePanel();
      AbstractUIPanel.OnPanelOpen += OnPanelOpened;
      AbstractUIPanel.OnPanelClose += OnPanelClosed;
    }

    private void OnDisable()
    {
      AbstractUIPanel.OnPanelOpen -= OnPanelOpened;
      AbstractUIPanel.OnPanelClose -= OnPanelClosed;
    }

    protected bool IsCorrectPanel(AbstractUIPanel panel)
    {
      return panel != null && panel == this.panel;
    }

    protected virtual void OnPanelOpened(AbstractUIPanel panel)
    {
      if (IsCorrectPanel(panel))
      {
        onPanelOpened?.Invoke();
      }
    }

    protected virtual void OnPanelClosed(AbstractUIPanel panel)
    {
      if (IsCorrectPanel(panel))
      {
        onPanelClosed?.Invoke();
      }
    }
  }
}