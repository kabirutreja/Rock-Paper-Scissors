using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Allows the invokation of an event without a parameter via the GlobalEventHandler.")]
  public class GlobalEventInvoker_NoParams : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The name of the event to invoke. " +
             "Event listeners have to register for this exact name to get notified about this event.")]
    protected string eventName;

    public string EventName { get => eventName; set => eventName = value; }

    [Button]
    [SkipObfuscationRename]
    public virtual void InvokeEvent()
    {
      if (!string.IsNullOrEmpty(EventName))
      {
        GlobalEventHandler.InvokeEvent(EventName);
      }
    }
  }
}