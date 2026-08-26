using CitrioN.Common;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  /// <summary>
  /// Helper script to register a <see cref="StyleProfile"/> for
  /// runtime queries. Useful to ensure the required 
  /// style profile is registered if using style profile 
  /// identifiers.
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Helper script to register a StyleProfile for runtime queries. " +
              "Useful to ensure the required style profile is registered if using style profile identifiers.")]
  public class RegisterStyleProfile : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The style profile to add for runtime usage.")]
    protected StyleProfile styleProfile;

    private void Awake()
    {
      if (styleProfile == null) { return; }
      styleProfile.TryAddProfile();
    }
  } 
}