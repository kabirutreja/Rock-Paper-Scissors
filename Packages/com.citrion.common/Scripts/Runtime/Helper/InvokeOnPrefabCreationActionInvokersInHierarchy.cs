using UnityEngine;

namespace CitrioN.Common
{
  [HideScriptField]
  [HeaderInfo("Allows the invokation of actions from OnPrefabCreationActionInvokers in this object's hierarchy.")]
  public class InvokeOnPrefabCreationActionInvokersInHierarchy : MonoBehaviour
  {
    [SerializeField]
    protected int frameDelay = 0;

    public int FrameDelay { get => frameDelay; set => frameDelay = value; }

    [ContextMenu("Invoke Action Invokers In Hierarchy")]
    public void InvokeActionInvokers()
    {
      ScheduleUtility.InvokeDelayedByFrames(GetAndInvokeActionInvokers, FrameDelay);
    }

    protected void GetAndInvokeActionInvokers()
    {
      var components = transform.GetComponentsInChildren<OnPrefabCreationActionInvoker>(true, true);
      if (components == null || components.Length < 1) { return; }

      foreach (var c in components)
      {
        if (c == null) { continue; }
        c.InvokeAction();
      }
    }
  } 
}