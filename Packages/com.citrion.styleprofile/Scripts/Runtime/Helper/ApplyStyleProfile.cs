using CitrioN.Common;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  [AddTooltips]
  [MethodButton("ApplyProfile", "Apply Profile", "Applies the referenced StyleProfile.", "button__apply-styleprofile")]
  [HeaderInfo("Allows the referenced StyleProfile to be applied both at runtime or edit mode. " +
              "To enable StyleListeners to react to style changes during edit mode the " +
              "'Register StyleListeners In Hierarchy (Edit Mode Only)' script also need to be attached. " +
              "This opt in approach avoids StyleListeners to react to changes when it is undesired " +
              "and only when you explicitely enable them using the script.")]
  [ExecuteInEditMode]
  [AddComponentMenu("CitrioN/Style Profile/Utility/Apply Style Profile")]
  public class ApplyStyleProfile : MonoBehaviour
  {

    [SerializeField]
    [Tooltip("The StyleProfile to apply during runtime, edit time or both.")]
    protected StyleProfile styleProfile;

    [Header("Header")]
    [SerializeField]
    [Tooltip("Whether the StyleProfile should be applied at runtime.")]
    protected bool applyAtRuntime = false;

    [SerializeField]
    [Tooltip("Whether the StyleProfile should be applied in edit mode.")]
    protected bool applyAtEditTime = true;

    private float last = -1f;

    public StyleProfile StyleProfile { get => styleProfile; set => styleProfile = value; }

    private void Awake() => ApplyProfile();

    private void OnEnable() => ApplyProfile();

    private void OnValidate() => ApplyProfile();


    [ContextMenu("Apply Style Profile")]
    public void ApplyProfile()
    {
      if (this == null) { return; }
      if (!isActiveAndEnabled || Time.time == last) { return; }

      if (StyleProfile == null) { return; }

      if (Application.isPlaying)
      {
        if (applyAtRuntime)
        {
          StyleProfile.ApplyProfile();
        }
      }
      else if (applyAtEditTime)
      {
        ScheduleUtility.InvokeDelayedByFrames(StyleProfile.ForceApplyProfile);
        //StyleProfile.ForceApplyProfile();
      }

      last = Time.time;
    }
  }
}
