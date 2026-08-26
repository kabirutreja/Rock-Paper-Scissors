using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Helper script that allows action(s) to be invoked when the children of this transform change. " +
              "This includes adding or removing children.")]
  public class OnTransformChildrenChangedListener : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The action(s) to be invoked when this transform's children have changed.")]
    protected UnityEvent onChildrenChanged = new();

    private void OnTransformChildrenChanged()
    {
      onChildrenChanged?.Invoke();
    }
  } 
}