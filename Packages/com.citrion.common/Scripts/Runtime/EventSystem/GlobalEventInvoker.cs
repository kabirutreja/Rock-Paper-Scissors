using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [SkipObfuscation]
  [HeaderInfo("Allows the invokation of an event with a single parameter via the GlobalEventHandler.")]
  public class GlobalEventInvoker<T> : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The name of the event to invoke. " +
             "Event listeners have to register for this exact name and parameter to get notified about this event.")]
    protected string eventName;

    [SerializeField]
    [Tooltip("The event parameter to include when the event is invoked.")]
    protected T argument;

    public string EventName { get => eventName; set => eventName = value; }
    
    public virtual T Argument => argument;

    [Button]
    [ContextMenu("Invoke Event")]
    public void InvokeEvent()
    {
      if (!string.IsNullOrEmpty(EventName))
      {
        GlobalEventHandler.InvokeEvent(EventName, Argument);
      }
    }
  }
}