using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the invoking of action(s) when a tab content of a 'TabMenu_UGUI' script is selected or deselected. " +
              "Useful for example if visuals should be changed if a tab's content is (de-)selected.")]
  [AddComponentMenu("CitrioN/UI/TabMenu (UGUI)/Tab Content Messages Receiver (UGUI)")]
  public class OnTabContentSelectedReceiver : MonoBehaviour
  {
    [SerializeField]
    protected UnityEvent onTabContentSelected = new UnityEvent();

    [SerializeField]
    protected UnityEvent onTabContentDeselected = new UnityEvent();

    public virtual void OnTabContentSelected()
    {
      onTabContentSelected?.Invoke();
    }

    public virtual void OnTabContentDeselected()
    {
      onTabContentDeselected?.Invoke();
    }
  }
}