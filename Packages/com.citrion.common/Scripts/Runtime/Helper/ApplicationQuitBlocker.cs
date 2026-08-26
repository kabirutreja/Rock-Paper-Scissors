using UnityEngine;

namespace CitrioN.Common
{
  [HideScriptField]
  [HeaderInfo("Helper script that can be used to prevent the application to quit. " +
              "Useful to ensure certain functionality has finished before allowing the application to quit. " +
              "Does not work in the editor play mode.")]
  public class ApplicationQuitBlocker : MonoBehaviour
  {
    private void OnEnable()
    {
      ApplicationQuitListener.AddApplicationQuitBlocker(this);
    }

    private void OnDisable()
    {
      ApplicationQuitListener.RemoveApplicationQuitBlocker(this);
    }
  }
}