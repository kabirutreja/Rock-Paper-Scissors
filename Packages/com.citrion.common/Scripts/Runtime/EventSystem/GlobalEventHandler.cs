using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CitrioN.Common
{
  public static class GlobalEventHandler
  {
    private static Dictionary<string, List<CustomActionBase>> globalActions = new Dictionary<string, List<CustomActionBase>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      ClearActions();
    }

    static GlobalEventHandler()
    {
      ClearActions();
    }

    private static void ClearActions()
    {
      globalActions.Clear();
    }

    private static void AddGlobalAction(string name, CustomActionBase action)
    {
      if (globalActions.TryGetValue(name, out List<CustomActionBase> list))
      {
        // TODO Should this use AddIfNotContains?
        list.Add(action);
      }
      else
      {
        globalActions.Add(name, new List<CustomActionBase>() { action });
      }
    }

    private static List<CustomActionBase> GetGlobalActions(string name)
    {
      globalActions.TryGetValue(name, out List<CustomActionBase> actions);
      return actions;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/CitrioN/Common/Events/Print Events")]
#endif
    public static void PrintAllActions()
    {
      StringBuilder sb = new StringBuilder();

      if (globalActions.Count == 0)
      {
        ConsoleLogger.Log("No events found!");
        return;
      }

      foreach (var pair in globalActions)
      {
        var eventName = pair.Key;
        var actions = pair.Value;

        foreach (var action in actions)
        {
          sb.AppendLine($"{eventName}: {action.GetType().Name}");
        }
      }
      ConsoleLogger.Log(sb.ToString());
    }

    #region Add Event Listener

    public static void AddEventListener(string name, Action action)
    {
      AddGlobalAction(name, new CustomAction(action));
    }

    public static void AddEventListener<T1>(string name, Action<T1> action)
    {
      AddGlobalAction(name, new GenericAction<T1>(action));
    }

    public static void AddEventListener<T1, T2>(string name, Action<T1, T2> action)
    {
      AddGlobalAction(name, new GenericAction<T1, T2>(action));
    }

    public static void AddEventListener<T1, T2, T3>(string name, Action<T1, T2, T3> action)
    {
      AddGlobalAction(name, new GenericAction<T1, T2, T3>(action));
    }

    public static void AddEventListener<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
    {
      AddGlobalAction(name, new GenericAction<T1, T2, T3, T4>(action));
    }

    #endregion

    #region Remove Event Listener
    private static void RemoveActionAtIndex(List<CustomActionBase> actions, string name, int index)
    {
      if (actions == null) { return; }
      actions.RemoveAt(index);
      if (actions.Count == 0)
      {
        globalActions.Remove(name);
      }
    }

    public static void RemoveEventListener(string name, Action action)
    {
      if (ApplicationQuitListener.isQuitting) return;
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          if (actions[i] is CustomAction customAction &&
              customAction.Action == action)
          {
            RemoveActionAtIndex(actions, name, i);
            return;
          }
        }
      }
    }

    public static void RemoveEventListener<T1>(string name, Action<T1> action)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          if (actions[i] is GenericAction<T1> genericAction &&
              genericAction.Action == action)
          {
            RemoveActionAtIndex(actions, name, i);
            return;
          }
        }
      }
    }

    public static void RemoveEventListener<T1, T2>(string name, Action<T1, T2> action)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          if (actions[i] is GenericAction<T1, T2> genericAction &&
              genericAction.Action == action)
          {
            RemoveActionAtIndex(actions, name, i);
            return;
          }
        }
      }
    }

    public static void RemoveEventListener<T1, T2, T3>(string name, Action<T1, T2, T3> action)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          if (actions[i] is GenericAction<T1, T2, T3> genericAction &&
              genericAction.Action == action)
          {
            RemoveActionAtIndex(actions, name, i);
            return;
          }
        }
      }
    }

    public static void RemoveEventListener<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          if (actions[i] is GenericAction<T1, T2, T3, T4> genericAction &&
              genericAction.Action == action)
          {
            RemoveActionAtIndex(actions, name, i);
            return;
          }
        }
      }
    }

    #endregion

    #region Invoke Event

    public static void InvokeEvent(string name)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          var action = actions[i];
          if (action is CustomAction customAction)
          {
            customAction.Invoke();
          }
        }
      }
    }

    public static void InvokeEvent<T1>(string name, T1 arg1)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          var action = actions[i];
          if (action is GenericAction<T1> genericAction)
          {
            genericAction.Invoke(arg1);
          }
        }
      }
    }

    public static void InvokeEvent<T1, T2>(string name, T1 arg1, T2 arg2)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          var action = actions[i];
          if (action is GenericAction<T1, T2> genericAction)
          {
            genericAction.Invoke(arg1, arg2);
          }
        }
      }
    }

    public static void InvokeEvent<T1, T2, T3>(string name, T1 arg1, T2 arg2, T3 arg3)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          var action = actions[i];
          if (action is GenericAction<T1, T2, T3> genericAction)
          {
            genericAction.Invoke(arg1, arg2, arg3);
          }
        }
      }
    }

    public static void InvokeEvent<T1, T2, T3, T4>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      var actions = GetGlobalActions(name);
      if (actions != null)
      {
        for (int i = 0; i < actions.Count; i++)
        {
          var action = actions[i];
          if (action is GenericAction<T1, T2, T3, T4> genericAction)
          {
            genericAction.Invoke(arg1, arg2, arg3, arg4);
          }
        }
      }
    }

    #endregion
  }
}