using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  [SkipObfuscation]
  [AddTooltips]
  [HeaderInfo("Allows the listening to an event with two parameter via the GlobalEventHandler.\n\n" +
              "If the matching event is raised the specified action(s) will be invoked.")]
  public class GlobalEventListener<T1, T2> : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The event name to listen to.")]
    protected string eventName;

    [SerializeField]
    [Tooltip("The action(s) to be invoked when the correct event is raised.")]
    protected UnityEvent<T1, T2> action;

    private void OnEnable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.AddEventListener<T1, T2>(eventName, OnEventInvoked);
      }
    }

    private void OnDisable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.RemoveEventListener<T1, T2>(eventName, OnEventInvoked);
      }
    }

    protected virtual void OnEventInvoked(T1 arg1, T2 arg2)
    {
      InvokeUnityEvent(arg1, arg2);
    }

    private void InvokeUnityEvent(T1 arg1, T2 arg2)
    {
      action?.Invoke(arg1, arg2);
    }
  }

  [SkipObfuscation]
  [AddTooltips]
  [HeaderInfo("Allows the listening to an event with a single parameter via the GlobalEventHandler.\n\n" +
              "If the matching event is raised the specified action(s) will be invoked.")]
  public class GlobalEventListener<T> : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The event name to listen to.")]
    protected string eventName;

    [SerializeField]
    [Tooltip("The action(s) to be invoked when the correct event is raised.")]
    protected UnityEvent<T> action;

    private void OnEnable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.AddEventListener<T>(eventName, OnEventInvoked);
      }
    }

    private void OnDisable()
    {
      if (!string.IsNullOrEmpty(eventName))
      {
        GlobalEventHandler.RemoveEventListener<T>(eventName, OnEventInvoked);
      }
    }

    protected virtual void OnEventInvoked(T arg)
    {
      InvokeUnityEvent(arg);
    }

    private void InvokeUnityEvent(T arg)
    {
      action?.Invoke(arg);
    }
  }
}