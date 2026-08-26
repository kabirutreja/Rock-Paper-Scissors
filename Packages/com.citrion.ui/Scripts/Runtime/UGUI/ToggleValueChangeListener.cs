using CitrioN.Common;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CitrioN.UI
{
  /// <summary>
  /// Listens to <see cref="Toggle.onValueChanged"/> and invokes
  /// events based on the new value of <see cref="Toggle.isOn"/>
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Allows action(s) to be invoked when a 'Toggle' component's value has changed.")]
  [RequireComponent(typeof(Toggle))]
  public class ToggleValueChangeListener : MonoBehaviour
  {
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    [Tooltip("The toggle component to listen to for value changes.")]
    protected Toggle toggle;

    [Tooltip("The action(s) to invoke when the toggle value became on.")]
    public UnityEvent onBecameOn = new UnityEvent();
    [Tooltip("The action(s) to invoke when the toggle value became off.")]
    public UnityEvent onBecameOff = new UnityEvent();

    public event Action onBecameOnAction;
    public event Action onBecameOffAction;

    private void Reset()
    {
      toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
      if (toggle == null)
      {
        toggle = GetComponent<Toggle>();
      }
    }

    private void OnEnable()
    {
      toggle.onValueChanged.AddListener(OnValueChanged);
      //OnValueChanged(toggle.isOn);
      // We delay the visual updating of the current state to allow
      // Animator components to set their states immediately.
      // Directly applying the value would result in an animation playing.
      ScheduleUtility.InvokeDelayedByFrames(() => OnValueChanged(toggle.isOn));
    }

    private void OnDisable()
    {
      toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
      if (isOn)
      {
        onBecameOn?.Invoke();
        onBecameOnAction?.Invoke();
      }
      else
      {
        onBecameOff?.Invoke();
        onBecameOffAction?.Invoke();
      }
    }
  }
}