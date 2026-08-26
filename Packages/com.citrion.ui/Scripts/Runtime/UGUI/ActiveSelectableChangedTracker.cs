using CitrioN.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.UI
{
  public class ActiveSelectableChangedTracker : /*Singleton<ActiveSelectableChangedTracker>*/ MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The EventSystem to use. Will be fetched automatically if left empty.")]
    protected EventSystem system = null;

    //[SerializeField]
    protected GameObject lastSelectable = null;

    protected virtual EventSystem EventSystem
    {
      get
      {
        if (system == null)
        {
          system = EventSystem.current;
        }
        return system;
      }
    }

    //private void Start()
    //{
    //  system = EventSystem.current;
    //}

    private void Update()
    {
      if (EventSystem == null) { return; }

      var currentSelectable = EventSystem.currentSelectedGameObject;

      if (currentSelectable != lastSelectable)
      {

        if (lastSelectable != null)
        {
          var selectedBefore = lastSelectable.GetComponentsInChildren<OnSelectedEventInvoker>().ToList();

          var parent = lastSelectable.transform.parent;
          if (parent != null)
          {
            var eventInvokerOnParent = parent.GetComponent<OnSelectedEventInvoker>();
            if (eventInvokerOnParent != null)
            {
              if (selectedBefore == null)
              {
                selectedBefore = new List<OnSelectedEventInvoker>();
              }
              selectedBefore.Add(eventInvokerOnParent);
            }
          }

          if (selectedBefore != null)
          {
            foreach (var i in selectedBefore)
            {
              i.OnDeselected();
            }
          }
        }

        if (currentSelectable != null)
        {
          var selectedAfter = currentSelectable.GetComponentsInChildren<OnSelectedEventInvoker>().ToList();

          var parent = currentSelectable.transform.parent;
          if (parent != null)
          {
            var eventInvokerOnParent = parent.GetComponent<OnSelectedEventInvoker>();
            if (eventInvokerOnParent != null)
            {
              if (selectedAfter == null)
              {
                selectedAfter = new List<OnSelectedEventInvoker>();
              }
              selectedAfter.Add(eventInvokerOnParent);
            }
          }

          if (selectedAfter != null)
          {
            foreach (var i in selectedAfter)
            {
              i.OnSelected();
            }
          }
        }


        //if (currentSelectable != null)
        //{
        //  var onSelectedAfter = currentSelectable.GetComponentsInChildren<OnSelectedEventInvoker>();
        //  if (onSelectedBefore == null || onSelectedBefore.Length == 0)
        //  {
        //    var parent = lastSelectable.transform.parent;
        //    if (parent != null)
        //    {
        //      var eventInvokerOnParent = parent.GetComponent<OnSelectedEventInvoker>();
        //      if (eventInvokerOnParent != null)
        //      {
        //        onSelectedBefore = new OnSelectedEventInvoker[] { eventInvokerOnParent };
        //      }
        //    }
        //    if (onSelectedBefore != null)
        //    {
        //      foreach (var i in onSelectedBefore)
        //      {
        //        i.OnDeselected();
        //      }
        //    }
        //  }

        //  if (currentSelectable != null)
        //  {
        //    var parent = currentSelectable.transform.parent;
        //    var selectable = parent != null ? parent.gameObject : lastSelectable;
        //    var onSelectedAfter = selectable.GetComponentsInChildren<OnSelectedEventInvoker>();
        //    if (onSelectedAfter != null)
        //    {
        //      foreach (var i in onSelectedAfter)
        //      {
        //        i.OnSelected();
        //      }
        //    }
        //  }
        ConsoleLogger.Log($"Changed selection from {lastSelectable} to {currentSelectable}",
                          Common.LogType.Debug);

        lastSelectable = currentSelectable;
      }
    }
  }
}