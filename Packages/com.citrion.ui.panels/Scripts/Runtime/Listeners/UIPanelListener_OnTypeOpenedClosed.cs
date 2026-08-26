using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the invokation of action(s) when a panel " +
              "of the correct type (and name) is opened or closed.")]
  public class UIPanelListener_OnTypeOpenedClosed : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Reference to an 'AbstractUIPanel' that defines the panel type to listen for.")]
    private AbstractUIPanel abstractUIPanelTypeReference = null;

    [SerializeField]
    [Tooltip("The name of the panel to listen for. If a name is specified the correct type " +
             "is not sufficient for the action(s) to be invoked.")]
    protected string panelName = string.Empty;

    [Tooltip("Action(s) to invoke when a matching panel is opened.")]
    public UnityEvent onPanelOfTypeOpened = new UnityEvent();
    [Tooltip("Action(s) to invoke when a matching panel is closed.")]
    public UnityEvent onPanelOfTypeClosed = new UnityEvent();

    private void OnEnable()
    {
      AbstractUIPanel.OnPanelOpen += OnPanelOpened;
      AbstractUIPanel.OnPanelClose += OnPanelClosed;
    }

    private void OnDisable()
    {
      AbstractUIPanel.OnPanelOpen -= OnPanelOpened;
      AbstractUIPanel.OnPanelClose -= OnPanelClosed;
    }

    protected bool IsCorrectPanelType(AbstractUIPanel abstractUIPanel)
    {
      if (abstractUIPanel != null && abstractUIPanelTypeReference != null)
      {
        if (abstractUIPanel.GetType() == abstractUIPanelTypeReference.GetType())
        {
          return true;
        }
      }
      return false;
    }

    protected virtual void OnPanelOpened(AbstractUIPanel abstractUIPanel)
    {
      if (IsCorrectPanelType(abstractUIPanel))
      {
        if (!string.IsNullOrEmpty(panelName))
        {
          if (abstractUIPanel.PanelName == panelName)
          {
            onPanelOfTypeOpened?.Invoke();
          }
        }
        else
        {
          onPanelOfTypeOpened?.Invoke();
        }
      }
    }

    protected virtual void OnPanelClosed(AbstractUIPanel abstractUIPanel)
    {
      if (IsCorrectPanelType(abstractUIPanel))
      {
        if (!string.IsNullOrEmpty(panelName))
        {
          if (abstractUIPanel.PanelName == panelName)
          {
            onPanelOfTypeClosed?.Invoke();
          }
        }
        else
        {
          onPanelOfTypeClosed?.Invoke();
        }
      }
    }
  }
}