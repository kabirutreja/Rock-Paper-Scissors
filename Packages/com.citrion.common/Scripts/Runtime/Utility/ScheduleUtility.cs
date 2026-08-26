using System;

namespace CitrioN.Common
{
  public static class ScheduleUtility
  {
    public static void InvokeDelayedByFrames(Action action, int frames = 1)
    {
      if (frames < 1)
      {
        action?.Invoke();
        return;
      }

#if UNITY_EDITOR
      if (UnityEditor.EditorApplication.isPlaying)
      {
        if (CoroutineRunner.Instance != null)
        {
          CoroutineRunner.Instance.InvokeDelayedByFrames(action, frames);
        }
      }
      else
      {
        if (frames > 1)
        {
          DelayCallActionInvokation(action, frames);
        }
        else
        {
          // TODO
          // Should this also be invoked delayed by the actual frame count?
          UnityEditor.EditorApplication.delayCall += action.Invoke;
        }
      }
#else
      if (CoroutineRunner.Instance != null)
      {
        CoroutineRunner.Instance.InvokeDelayedByFrames(action, frames);
      }
#endif
    }

#if UNITY_EDITOR
    private static void DelayCallActionInvokation(Action action, int framesRemaining)
    {
      if (framesRemaining <= 0)
      {
        action?.Invoke();
        return;
      }

      UnityEditor.EditorApplication.delayCall += () => DelayCallActionInvokation(action, --framesRemaining);
    }
#endif
  }
}