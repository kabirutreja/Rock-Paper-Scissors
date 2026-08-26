using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Utility script that allows functionality to be invoked this object is selected or deselected. " +
              "Based on the ISelectHandler and IDeselectHandler interfaces.")]
  public class OnSelectedEventInvoker : MonoBehaviour, ISelectHandler, IDeselectHandler
  {
    [SerializeField]
    [Tooltip("Action to invoke when this object is selected.")]
    protected UnityEvent onSelected = new UnityEvent();

    [SerializeField]
    [Tooltip("Action to invoke when this object is deselected.")]
    protected UnityEvent onDeselected = new UnityEvent();

    [SerializeField]
    [Tooltip("Action to invoke when this object is selected. Uses this GameObject as a parameter.")]
    protected UnityEvent<GameObject> onSelectedGameObject = new UnityEvent<GameObject>();

    [SerializeField]
    [Tooltip("Action to invoke when this object is deselected. Uses this GameObject as a parameter.")]
    protected UnityEvent<GameObject> onDeselectedGameObject = new UnityEvent<GameObject>();

    public UnityEvent OnSelectedEvent { get => onSelected; protected set => onSelected = value; }

    public UnityEvent OnDeselectedEvent { get => onDeselected; protected set => onDeselected = value; }

    public UnityEvent<GameObject> OnSelectedGameObject { get => onSelectedGameObject; protected set => onSelectedGameObject = value; }

    public UnityEvent<GameObject> OnDeselectedGameObject { get => onDeselectedGameObject; protected set => onDeselectedGameObject = value; }

    public virtual void OnSelected()
    {
      OnSelectedEvent?.Invoke();
      OnSelectedGameObject?.Invoke(gameObject);
    }

    public virtual void OnDeselected()
    {
      OnDeselectedEvent?.Invoke();
      OnDeselectedGameObject?.Invoke(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
      OnSelected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
      OnDeselected();
    }
  }
}