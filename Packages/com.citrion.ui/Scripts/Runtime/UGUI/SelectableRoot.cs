using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Serves as the root of a selectable such as a dropdown or slider. " +
              "Any selectables that have the 'OnSelectedEventInvoker' script will invoke the required events " +
              "to inform a 'SelectableRoot' about them being selected or deselected.")]
  public class SelectableRoot : MonoBehaviour
  {
    [Tooltip("Action(s) to invoke when an 'OnSelectedEventInvoker' invoked its 'onSelected' event.")]
    public UnityEvent onSelected = new UnityEvent();

    [Tooltip("Action(s) to invoke when an 'OnSelectedEventInvoker' invoked its 'onDeselected' event.")]
    public UnityEvent onDeselected = new UnityEvent();

    [SerializeReference]
    [Tooltip("All event invokers to listen to.")]
    protected List<OnSelectedEventInvoker> onSelectedEventInvokers = new List<OnSelectedEventInvoker>();

    protected virtual void Start()
    {
      InitInvokers();
    }

    public void InitInvokers()
    {
      RemoveListeners();
      onSelectedEventInvokers.Clear();

      var invokers = GetComponentsInChildren<OnSelectedEventInvoker>();
      foreach (var i in invokers)
      {
        onSelectedEventInvokers.AddIfNotContains(i);
      }

      AddListeners();
    }

    protected virtual void OnEnable()
    {
      AddListeners();
    }

    private void AddListeners()
    {
      foreach (var i in onSelectedEventInvokers)
      {
        i.OnSelectedEvent.AddListener(OnSelected);
        i.OnDeselectedEvent.AddListener(OnDeselected);
      }
    }

    private void OnDisable()
    {
      RemoveListeners();
    }

    protected void RemoveListeners()
    {
      foreach (var i in onSelectedEventInvokers)
      {
        i.OnSelectedEvent.RemoveListener(OnSelected);
        i.OnDeselectedEvent.RemoveListener(OnDeselected);
      }
    }

    protected virtual void OnSelected()
    {
      onSelected?.Invoke();
    }

    protected virtual void OnDeselected()
    {
      onDeselected?.Invoke();
    }
  }
}