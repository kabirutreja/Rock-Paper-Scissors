using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Listens to the <see cref="Application.quitting"/> event and caches
  /// it in <see cref="isQuitting"/> for other scripts to retrieve that information.
  /// This is useful because some functionality may not be needed to invoke when an object
  /// is being destroyed due to the application quitting.
  /// </summary>
  public static class ApplicationQuitListener
  {
    public static bool isQuitting = false;

    private static List<ApplicationQuitBlocker> quitBlockers = new List<ApplicationQuitBlocker>();

    private static bool PreventQuitting
    {
      get
      {
        if (quitBlockers == null) { return false; }
        return quitBlockers.Count > 0;
      }
    }

    private static void OnQuit()
    {
      GlobalEventHandler.InvokeEvent("OnApplicationQuit");
      isQuitting = true;
      ConsoleLogger.Log("Quitting Application".Colorize(Color.yellow), LogType.EditorAndDebug);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      isQuitting = false;
      quitBlockers = new List<ApplicationQuitBlocker>();
      Application.quitting -= OnQuit;
      Application.quitting += OnQuit;
      Application.wantsToQuit -= OnWantsToQuit;
      Application.wantsToQuit += OnWantsToQuit;
    }

    private static bool OnWantsToQuit() => !PreventQuitting;

    public static void AddApplicationQuitBlocker(ApplicationQuitBlocker blocker)
    {
      quitBlockers.AddIfNotContains(blocker);
    }

    public static void RemoveApplicationQuitBlocker(ApplicationQuitBlocker blocker)
    {
      quitBlockers.Remove(blocker);
    }

    public static void TryQuitApplication()
    {
      GlobalEventHandler.InvokeEvent("OnBeforeApplicationQuit");
      if (CoroutineRunner.Instance != null && PreventQuitting)
      {
        CoroutineRunner.Instance.StartCoroutine(WaitForApplicationCanQuit());
      }
      else
      {
        QuitApplication();
      }
    }

    public static IEnumerator WaitForApplicationCanQuit()
    {
      while (PreventQuitting) { yield return null; }
      QuitApplication();
    }

    public static void QuitApplication()
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}