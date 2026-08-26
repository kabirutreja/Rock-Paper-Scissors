using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscation]
  public enum LogType
  {
    GlobalSetting,
    Always,
    EditorOnly,
    BuildOnly,
    DevelopmentBuild,
    EditorAndDevelopmentBuild,
    Debug,
    EditorAndDebug,
    EditorAndDevelopmentBuildAndDebug,
    DevelopmentBuildAndDebug,
  }

  public static class ConsoleLogger
  {
    public static bool DisableRegularLogs = true;

    public static bool IsRuntimeDebugMode { get; set; } = false;

    public static bool IsDebugMode
    {
      get
      {
#if IS_DEBUG
        return true;
#else 
        return IsRuntimeDebugMode;
#endif
      }
    }

    static ConsoleLogger()
    {
      IsRuntimeDebugMode = false;
      DisableRegularLogs = false;
    }

    [SkipObfuscationRename]
    public static void Log(string text, LogType logType = LogType.GlobalSetting)
    {
      if (DisableRegularLogs) { return; }

      if (CanLogInput(logType))
      {
        Debug.Log(GetFormattedText(text));
      }
    }

    [SkipObfuscationRename]
    public static void Log(object text, LogType logType = LogType.GlobalSetting)
           => Log(text?.ToString(), logType);

    [SkipObfuscationRename]
    public static void LogWarning(string text, LogType logType = LogType.GlobalSetting)
    {
      if (CanLogInput(logType))
      {
        Debug.LogWarning(GetFormattedText(text));
      }
    }

    [SkipObfuscationRename]
    public static void LogWarning(object text, LogType logType = LogType.GlobalSetting)
           => LogWarning(text?.ToString(), logType);

    [SkipObfuscationRename]
    public static void LogError(string text, LogType logType = LogType.GlobalSetting)
    {
      if (CanLogInput(logType))
      {
        Debug.LogError(GetFormattedText(text));
      }
    }

    [SkipObfuscationRename]
    public static void LogError(object text, LogType logType = LogType.GlobalSetting)
           => LogError(text?.ToString(), logType);

    private static string GetFormattedText(string text)
    {
      return "<b>[DEBUG]</b> " + text;
    }

    private static bool CanLogInput(LogType logType)
    {
      bool shouldLog = false;
      switch (logType)
      {
        case LogType.GlobalSetting:
          shouldLog = true;
          break;
        case LogType.Always:
          shouldLog = true;
          break;
        case LogType.EditorOnly:
#if UNITY_EDITOR
          shouldLog = true;
#endif
          break;
        case LogType.BuildOnly:
#if !UNITY_EDITOR
          shouldLog = true;
#endif
          break;
        case LogType.DevelopmentBuild:
#if DEVELOPMENT_BUILD
          shouldLog = true;
#endif
          break;
        case LogType.EditorAndDevelopmentBuild:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
          shouldLog = true;
#endif
          break;
        case LogType.Debug:
          shouldLog = IsDebugMode;
          break;
        case LogType.EditorAndDebug:
#if UNITY_EDITOR
          shouldLog = true;
#else
          shouldLog = IsDebugMode;
#endif
          break;
        case LogType.EditorAndDevelopmentBuildAndDebug:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
          shouldLog = true;
#else 
          shouldLog = IsDebugMode;
#endif
          break;
        case LogType.DevelopmentBuildAndDebug:
#if DEVELOPMENT_BUILD
          shouldLog = true;
#else 
          shouldLog = IsDebugMode;
#endif
          break;
        default:
          break;
      }
      return shouldLog;
    }
  }
}