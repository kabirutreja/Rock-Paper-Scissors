using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Allows the invokation of functionality when a prefab is created. " +
              "The code that creates the prefab needs to specifically check for " +
              "this script and call the 'InvokeAction' method.")]
  public class OnPrefabCreationActionInvoker : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The functionality to invoke when the prefab is created.")]
    protected UnityEvent onPrefabCreation = new UnityEvent();

    public void InvokeAction()
    {
      onPrefabCreation?.Invoke();
    }
  } 
}