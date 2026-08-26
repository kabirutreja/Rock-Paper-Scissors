using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Allows the listening to an event with no parameters via the GlobalEventHandler.\n\n" +
              "If the matching event is raised the specified action(s) will be invoked.")]
  public class GlobalEventListener_NoParams : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The event name to listen to.")]
    protected string eventName;

    [SerializeField]
    [Tooltip("The action(s) to be invoked when the correct event is raised.")]
    protected UnityEvent action;

    private void OnEnable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.AddEventListener(eventName, OnEventInvoked);
      }
    }

    private void OnDisable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.RemoveEventListener(eventName, OnEventInvoked);
      }
    }

    protected virtual void OnEventInvoked()
    {
      InvokeUnityEvent();
    }

    private void InvokeUnityEvent()
    {
      action?.Invoke();
    }
  }
}