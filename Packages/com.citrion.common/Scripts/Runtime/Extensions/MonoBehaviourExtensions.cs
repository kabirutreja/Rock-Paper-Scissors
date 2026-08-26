using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Extension methods for <see cref="MonoBehaviour"/>s.
  /// </summary>
  public static class MonoBehaviourExtensions
  {
    public static void InvokeDelayed(this MonoBehaviour monoBehaviour, Action action, float delay)
    {
      if (monoBehaviour.CanStartCoroutine())
      {
        monoBehaviour.StartCoroutine(DelayedInvokation(action, delay));
      }
    }

    public static Coroutine InvokeDelayedByFrames(this MonoBehaviour monoBehaviour, Action action, float delayInFrames = 1)
    {
      if (monoBehaviour.CanStartCoroutine())
      {
        return monoBehaviour.StartCoroutine(DelayedByFramesInvokation(action, delayInFrames));
      }
      return null;
    }

    private static bool CanStartCoroutine(this MonoBehaviour monoBehaviour)
    {
      if (ApplicationQuitListener.isQuitting) { return false; }
      if (/*!monoBehaviour.enabled || */monoBehaviour.gameObject.activeInHierarchy == false)
      {
        ConsoleLogger.LogWarning("Can't start coroutine because " + monoBehaviour.name + " is not active.");
        return false;
      }
      return true;
    }

    private static IEnumerator DelayedInvokation(Action action, float delay)
    {
      if (delay > 0)
      {
        yield return new WaitForSeconds(delay);
      }
      action?.Invoke();
    }

    private static IEnumerator DelayedByFramesInvokation(Action action, float delayInFrames)
    {
      while (delayInFrames > 0)
      {
        yield return null;
        delayInFrames--;
      }
      action?.Invoke();
    }

    /// <summary>
    /// Activates/Deactivates all the gameobjects the behaviours 
    /// of the given list are attached to.
    /// </summary>
    /// <typeparam name="T">The class derived from MonoBehaviour</typeparam>
    /// <param name="list">The list to set the active state for</param>
    /// <param name="active">The active state information</param>
    public static void SetActiveForMonoBehaviourList<T>(this List<T> list, bool active = true) where T : MonoBehaviour
    {
      list.ForEach(behaviour => behaviour.gameObject.SetActive(active));
    }

    public static ScheduledAction InvokeActionRepeating(this MonoBehaviour behaviour, Action action, float startDelay = 0, float interval = -1)
    {
      return new ScheduledAction(behaviour, action, startDelay, interval);
    }

    public static void StopCoroutineSafe(this MonoBehaviour behaviour, Coroutine coroutine)
    {
      if (behaviour != null && coroutine != null)
      {
        behaviour.StopCoroutine(coroutine);
      }
    }
  }
}