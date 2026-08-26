using CitrioN.Common;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  [AddTooltips]
  [HeaderInfo("Allows the assignment of a 'StyleProfile' or a style profile identifier to " +
              "'StyleListener' components in this objects hierarchy. Doing this has the " +
              "advantage of being easily replaced with a different style profile which would potentially be " +
              "a lot of manual work if a lot of listeners need to have their profile or identifier changed.")]
  public class AssignStyleProfileToListenersInHierarchy : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The style profile to assign to its hierarchy.")]
    protected StyleProfile styleProfile;

    [SerializeField]
    [Tooltip("The style profile identifier to assign.")]
    protected string styleProfileIdentifier;

    [SerializeField]
    [Tooltip("Whether existing profiles/identifiers on StyleListeners should be overriden.")]
    protected bool overrideExisting = false;

    [SerializeField]
    [Tooltip("Whether the assignment should happen when this script was enabled.")]
    protected bool assignOnEnable = false;

    protected StyleProfile lastAppliedStyleProfile = null;

    protected string lastAppliedStyleProfileIdentifier = string.Empty;

    public StyleProfile StyleProfile
    {
      get => styleProfile;
      set => styleProfile = value;
    }

    protected string StyleProfileIdentifier
    {
      get => styleProfileIdentifier;
      set => styleProfileIdentifier = value;
    }

    protected virtual Transform Root => transform;

    protected virtual void OnEnable()
    {
      if (assignOnEnable && Application.isPlaying)
      {
        Assign(true);
      }
    }

    protected virtual void OnValidate()
    {
      // TODO Should this also check for assignOnEnable or some other variable?
      if (Application.isPlaying)
      {
        Assign(true);
      }
    }

    public void Assign(bool forceAssign)
    {
      if (Root == null) { return; }

      if (StyleProfile != null &&
         (forceAssign || StyleProfile != lastAppliedStyleProfile))
      {
        StyleProfile.AssignStyleProfileInHierarchy(Root, overrideExisting);
        lastAppliedStyleProfile = StyleProfile;
      }

      if (/*!string.IsNullOrEmpty(StyleProfileIdentifier) &&*/
         (forceAssign || StyleProfileIdentifier != lastAppliedStyleProfileIdentifier))
      {
        StyleProfile.AssignStyleProfileIdentifierInHierarchy(Root, overrideExisting, StyleProfileIdentifier);
        lastAppliedStyleProfileIdentifier = StyleProfileIdentifier;
      }
    }

    [ContextMenu("Force Assign And Apply Style Profile In Hierarchy")]
    public void ForceAssign()
    {
      Assign(true);
    }
  }
}