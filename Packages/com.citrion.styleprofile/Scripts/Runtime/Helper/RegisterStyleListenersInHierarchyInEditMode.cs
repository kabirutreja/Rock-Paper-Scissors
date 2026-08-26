using CitrioN.Common;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  /// <summary>
  /// Utility script that registers all <see cref="StyleListener"/> scripts in a hierarchy during edit mode.
  /// A registered <see cref="StyleListener"/> will react to <see cref="StyleProfile"/> changes.
  /// </summary>
  [HideScriptField]
  [MethodButton("GetAndRegisterStyleListenersFromHierarchy", "Register Style Listeners", 
              "Registers all StyleListener scripts currently in this objects hierarchy.", "button__register-style-listeners")]
  [HeaderInfo("Allows 'StyleListeners' in this object's hierarchy to react to style changes during edit mode. " +
              "This is useful if you prefer to see changes to your prefabs during edit mode rather than runtime. " +
              "The StyleListeners will be enabled when this script gets enabled. StyleListeners added after the script " +
              "was enabled will NOT automatically be enabled for edit mode." +
              "To enable them you can disable and then enable this script or use the 'Register Listeners' context menu item.")]
  [ExecuteInEditMode]
  [AddComponentMenu("CitrioN/Style Profile/Utility/Register StyleListeners In Hierarchy (Edit Mode Only)")]
  public class RegisterStyleListenersInHierarchyInEditMode : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Whether inactive StyleListeners should be registered too.")]
    protected bool includeInactive = true;

#if UNITY_EDITOR
    private StyleListener[] styleListeners = null;

    private float lastQuery = -1f;

    #region Unity Methods
    private void Awake() => GetAndRegisterStyleListenersFromHierarchy();

    private void OnEnable() => GetAndRegisterStyleListenersFromHierarchy();

    private void OnDisable() => UnregisterStyleListeners();

    private void OnDestroy() => UnregisterStyleListeners();

    private void OnValidate() => ScheduleUtility.InvokeDelayedByFrames(GetAndRegisterStyleListenersFromHierarchy);
    #endregion

    #region Other Methods
    private void RegisterStyleListeners()
    {
      if (Application.isPlaying || styleListeners == null) { return; }

      StyleListener listener;

      for (int i = 0; i < styleListeners.Length; i++)
      {
        listener = styleListeners[i];
        if (listener != null)
        {
          listener.RegisterEvents();
        }
      }
    }

    private void UnregisterStyleListeners()
    {
      if (Application.isPlaying || styleListeners == null) { return; }

      StyleListener listener;

      for (int i = 0; i < styleListeners.Length; i++)
      {
        listener = styleListeners[i];
        if (listener != null)
        {
          listener.UnregisterEvents();
        }
      }
    }

    [ContextMenu("Register Listeners")]
    public void GetAndRegisterStyleListenersFromHierarchy()
    {
      try
      {
        if (Application.isPlaying || Time.time == lastQuery || !isActiveAndEnabled) { return; }
      }
      catch (System.Exception)
      {
        return;
      }

      UnregisterStyleListeners();

      styleListeners = GetComponentsInChildren<StyleListener>(includeInactive);

      RegisterStyleListeners();

      lastQuery = Time.time;
    }
    #endregion

    //[ContextMenu("Apply Style Profile")]
    //public void ApplyStyleProfile()
    //{
    //  if (Application.isPlaying) { return; }
    //  if (styleProfile == null) { return; }
    //  if (styleListeners == null) { FetchListeners(); }

    //  StyleListener listener;

    //  for (int i = 0; i < styleListeners.Length; i++)
    //  {
    //    listener = styleListeners[i];

    //    if (listener == null) { continue; }

    //    if (styleProfile.GetValue(listener.Key, out var value))
    //    {
    //      listener.ApplyChange(value);
    //    }
    //  }
    //}

#endif
  }
}
