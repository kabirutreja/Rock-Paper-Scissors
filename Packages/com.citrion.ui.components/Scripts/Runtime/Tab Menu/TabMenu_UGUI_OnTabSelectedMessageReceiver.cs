using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Utility script that allows functionality to be invoked when a tab is selected or deselected in the hierarchy of this object. " +
              "The TabMenu_UGUI script broadcasts the respective messages when a tab is selected or deselected. " +
              "An example use case would be to change the visuals of a tab when it is (de)selected.")]
  [AddComponentMenu("CitrioN/UI/TabMenu (UGUI)/Tab Messages Receiver (UGUI)")]
  public class TabMenu_UGUI_OnTabSelectedMessageReceiver : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Action(s) to invoke when the tab of this hierarchy is selected.")]
    protected UnityEvent onTabSelected = new UnityEvent();

    [SerializeField]
    [Tooltip("Action(s) to invoke when the tab of this hierarchy is deselected.")]
    protected UnityEvent onTabDeselected = new UnityEvent();

    public virtual void OnTabSelected()
    {
      onTabSelected?.Invoke();
    }

    public virtual void OnTabDeselected()
    {
      onTabDeselected?.Invoke();
    }
  } 
}