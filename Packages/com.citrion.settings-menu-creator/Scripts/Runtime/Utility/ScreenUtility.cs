using CitrioN.Common;
using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.SettingsMenuCreator
{
  public static class ScreenUtility
  {
    public static Vector2Int activeScreenResolution = Vector2Int.zero;

#if UNITY_EDITOR && UNITY_2022_1_OR_NEWER
    private static bool hasChangedGameViewResolution = false;
    private static int selectedGameViewIndex = -1;

    private static string gameViewTypeString = "UnityEditor.GameView, UnityEditor";
    private static Type GameViewType => Type.GetType(gameViewTypeString);
    //private static EditorWindow GameViewWindow => EditorWindow.GetWindow(GameViewType);
    private static EditorWindow GameViewWindow => GetGameViewWithoutFocus();

    static ScreenUtility()
    {
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      activeScreenResolution = Vector2Int.zero;
      hasChangedGameViewResolution = false;
      selectedGameViewIndex = -1;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange change)
    {
      if (change == PlayModeStateChange.EnteredEditMode && 
          hasChangedGameViewResolution && selectedGameViewIndex >= 0)
      {
        ScheduleUtility.InvokeDelayedByFrames(()
          => SetSelectedGameViewSizeIndex(selectedGameViewIndex));
      }
    }

    static EditorWindow GetGameViewWithoutFocus()
    {
      var objects = Resources.FindObjectsOfTypeAll(GameViewType);
      return objects != null && objects.Length > 0 ? (EditorWindow)objects[0] : null;
    }

    static int GetSelectedGameViewSizeIndex()
    {
      var gameViewType = GameViewType;
      var gameViewWindow = GameViewWindow;
      var selectedSizeIndexProperty = gameViewType.GetProperty("selectedSizeIndex",
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      return (int)selectedSizeIndexProperty.GetValue(gameViewWindow);
    }

    static void SetSelectedGameViewSizeIndex(int index)
    {
      var gameViewType = GameViewType;
      var gameViewWindow = GameViewWindow;
      var selectedSizeIndexProperty = gameViewType.GetProperty("selectedSizeIndex",
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      selectedSizeIndexProperty.SetValue(gameViewWindow, index);
      gameViewWindow.Repaint();
      // Scale adjustments need to be delayed by a frame to have an effect
      ScheduleUtility.InvokeDelayedByFrames(FitScaleOfGameWindow);
    }

    private static void FitScaleOfGameWindow()
    {
      var gameViewType = GameViewType;
      var gameViewWindow = GameViewWindow;

      var bindingFlagsInstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
      var zoomField = gameViewType.GetField("m_ZoomArea", bindingFlagsInstanceNonPublic);
      var zoomObj = zoomField?.GetValue(gameViewWindow);
      if (zoomObj == null) { return; }

      var zoomType = zoomObj.GetType();
      var bindingFlagsInstanceNonPublicPublic = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
      var drawRectProperty = zoomType.GetProperty("drawRect", bindingFlagsInstanceNonPublicPublic);
      var targetSizeProperty = gameViewType.GetProperty("targetSize", bindingFlagsInstanceNonPublicPublic);

      if (drawRectProperty == null || targetSizeProperty == null) { return; }

      var drawRect = (Rect)drawRectProperty.GetValue(zoomObj, null);
      var targetSize = (Vector2)targetSizeProperty.GetValue(gameViewWindow, null);

      if (targetSize.x <= 0 || targetSize.y <= 0) { return; }

      float scale = Mathf.Min(drawRect.width / targetSize.x, drawRect.height / targetSize.y);
      SetGameViewScale(scale);
    }

    public static void SetGameViewScale(float scale)
    {
      var gameViewType = GameViewType;
      var gameViewWindow = GameViewWindow;

      var bindingFlagsInstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
      var zoomField = gameViewType.GetField("m_ZoomArea", bindingFlagsInstanceNonPublic);
      var zoomObj = zoomField?.GetValue(gameViewWindow);
      if (zoomObj == null) { return; }

      var zoomType = zoomObj.GetType();
      var setScaleMethod = zoomType.GetMethod("SetScale",
          bindingFlagsInstanceNonPublic,
          binder: null,
          types: new[] { typeof(Vector2), typeof(Vector2) },
          modifiers: null);

      Vector2 pivot = Vector2.zero;
      var bindingFlagsInstanceNonPublicPublic = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
      var drawRectProperty = zoomType.GetProperty("drawRect", bindingFlagsInstanceNonPublicPublic);
      if (drawRectProperty != null)
      {
        var drawRect = (Rect)drawRectProperty.GetValue(zoomObj, null);
        pivot = drawRect.center;
      }

      if (setScaleMethod != null)
      {
        setScaleMethod.Invoke(zoomObj, new object[] { new Vector2(scale, scale), pivot });
        gameViewWindow.Repaint();
        return;
      }

      var scaleField = zoomType.GetField("m_Scale", bindingFlagsInstanceNonPublic);
      if (scaleField != null)
      {
        scaleField.SetValue(zoomObj, new Vector2(scale, scale));
        gameViewWindow.Repaint();
        return;
      }
    }
#endif

    public static void SetScreenResolution(int width, int height, bool fullscreen = true)
    {
      Screen.SetResolution(width, height, fullscreen);
      activeScreenResolution.x = width;
      activeScreenResolution.y = height;

#if UNITY_EDITOR && UNITY_2022_1_OR_NEWER
      var windows = (UnityEditor.EditorWindow[])Resources.FindObjectsOfTypeAll(typeof(UnityEditor.EditorWindow));
      foreach (var window in windows)
      {
        if (window != null)
        {
          var fullName = window.GetType().FullName;
          if (fullName != "UnityEditor.GameView") { continue; }
          var assembly = typeof(UnityEditor.EditorWindow).Assembly;
          var type = assembly?.GetType("UnityEditor.GameView");

          var method = type?.GetPrivateMethod("SetCustomResolution");
          //var method = type?.GetPrivateMethod("SetMainPlayModeViewSize");
          //var method = type?.GetPrivateMethod("set_targetSize");
          //var methods = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          //foreach (var item in methods)
          //{
          //  var parameters = item.GetParameters();
          //  if (parameters?.Length > 0)
          //  {
          //    if (parameters[0].ParameterType == typeof(Vector2))
          //    {
          //      ConsoleLogger.Log($"{item.Name}");
          //    }
          //  }
          //}

          if (!hasChangedGameViewResolution)
          {
            hasChangedGameViewResolution = true;
            selectedGameViewIndex = GetSelectedGameViewSizeIndex();
          }

          method?.Invoke(window, new object[] { new Vector2(width, height), string.Empty });
          break;
        }
      }

      //UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
    }

    public static bool GetScreenResolutionFromString(string resolutionString,
      out Resolution resolution, char splitCharacter = ' ')
    {
      resolution = new Resolution();
      var splitRes = resolutionString.Split(splitCharacter);
      if (splitRes.Length >= 2)
      {
        // Parse the first and last elements
        // to width and height respectively
        if (int.TryParse(splitRes[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width) &&
            int.TryParse(splitRes[splitRes.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
        {
          resolution.width = width;
          resolution.height = height;
          return true;
        }
      }
      return false;
    }

    public static string GetResolutionAsString(Resolution resolution)
      => $"{resolution.width} x {resolution.height}";

    public static string GetCurrentResolutionAsString()
      => GetResolutionAsString(Screen.currentResolution);
  }
}