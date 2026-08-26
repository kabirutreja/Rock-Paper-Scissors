using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Helper script to allow the invokation of functionality when this script/object gets enabled or disabled.")]
  public class OnEnableDisableEvents : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The action(s) to be invoked when this script gets enabled. " +
             "This includes getting enabled when this object gets enabled.")]
    private UnityEvent onEnable = new UnityEvent();
    [SerializeField]
    [Tooltip("The action(s) to be invoked when this script gets disabled. " +
             "This includes getting disabled when this object gets disabled.")]
    private UnityEvent onDisable = new UnityEvent();

    public UnityEvent OnEnableEvent { get => onEnable; set => onEnable = value; }
    public UnityEvent OnDisableEvent { get => onDisable; set => onDisable = value; }

    private void OnEnable()
    {
      OnEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
      OnDisableEvent?.Invoke();
    }
  }
}