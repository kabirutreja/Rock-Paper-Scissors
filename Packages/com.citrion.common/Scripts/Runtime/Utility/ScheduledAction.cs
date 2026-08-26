using System;
using System.Collections;
using UnityEngine;

namespace CitrioN.Common
{
  [System.Serializable]
  public class ScheduledAction
  {
    [SerializeField]
    private MonoBehaviour invoker;
    [SerializeField]
    private Coroutine coroutine;
    private Action action;
    private float delay;
    private float interval;

    public ScheduledAction(MonoBehaviour invoker, Action action, float delay = 0, float interval = -1)
    {
      if (invoker == null)
      {
        Debug.LogError($"An {nameof(invoker)} needs to be specified");
      }
      if (action == null)
      {
        Debug.LogError($"An {nameof(action)} needs to be specified");
      }
      this.invoker = invoker;
      this.action = action;
      this.delay = delay;
      this.interval = interval;
      RestartAction();
    }

    public void RestartAction()
    {
      CancelAction();
      coroutine = invoker.StartCoroutine(Schedule(action, delay, interval));
    }

    public void CancelAction()
    {
      if (invoker != null && coroutine != null)
      {
        invoker.StopCoroutine(coroutine);
        coroutine = null;
      }
    }

    public void CancelAction(ref ScheduledAction referenceToClear)
    {
      CancelAction();
      referenceToClear = null;
    }

    private IEnumerator Schedule(Action action, float delay, float interval)
    {
      if (delay > 0)
      {
        yield return new WaitForSeconds(delay);
      }

      action?.Invoke();

      if (interval >= 0)
      {
        if (interval == 0)
        {
          while (true)
          {
            yield return null;
            action?.Invoke();
          }
        }
        else
        {
          var waitForSeconds = new WaitForSeconds(interval);
          while (true)
          {
            yield return waitForSeconds;
            action?.Invoke();
          }
        }
      }
    }
  }
}