using UnityEngine;

namespace CitrioN.Common
{
  public class GlobalEventHandlerDebugger : MonoBehaviour
  {
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    protected int eventNameCount = 0;

    //private void Update()
    //{
    //  eventNameCount = GlobalEventHandler.globalEvents.Count;
    //}
  }
}