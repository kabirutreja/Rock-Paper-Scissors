using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Tracks open <see cref="AbstractUIPanel"/> by saving them in a list.
  /// </summary>
  public static partial class UIPanelManager
  {
    [SerializeField, Tooltip("All UIPanels that are currently enabled")]
    private static List<AbstractUIPanel> enabledPanels = new List<AbstractUIPanel>();
    [SerializeField, Tooltip("All UIPanels that are currently open")]
    private static List<AbstractUIPanel> openPanels = new List<AbstractUIPanel>();
    [SerializeField, Tooltip("All UIPanels that have been initialized")]
    private static List<AbstractUIPanel> initializedPanels = new List<AbstractUIPanel>();

    /// <summary>
    /// List containing all enabled <see cref="AbstractUIPanel"/>
    /// </summary>
    public static List<AbstractUIPanel> EnabledPanels { get { return enabledPanels; } private set { enabledPanels = value; } }

    /// <summary>
    /// List containing all open <see cref="AbstractUIPanel"/>
    /// </summary>
    public static List<AbstractUIPanel> OpenPanels { get { return openPanels; } private set { openPanels = value; } }

    public static List<AbstractUIPanel> InitializedPanels { get => initializedPanels; set => initializedPanels = value; }

    /// <summary>
    /// Determines if any <see cref="AbstractUIPanel"/> is currently open
    /// </summary>
    public static bool AnyPanelOpen => OpenPanels != null && OpenPanels.Count > 0;


    //[RuntimeInitializeOnLoadMethod]
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    /// <summary>
    /// Subsribes to the open/close events of <see cref="AbstractUIPanel"/>
    /// </summary>
    private static void Initialize()
    {
      InitializedPanels = new List<AbstractUIPanel>();
      EnabledPanels = new List<AbstractUIPanel>();
      OpenPanels = new List<AbstractUIPanel>();

      AbstractUIPanel.OnPanelOpen += OnPanelOpened;
      AbstractUIPanel.OnPanelClose += OnPanelClosed;
      AbstractUIPanel.OnPanelEnable += OnPanelEnable;
      AbstractUIPanel.OnPanelDisable += OnPanelDisable;
      AbstractUIPanel.OnPanelInitialize += OnPanelInitialize;

      //AbstractUIPanel.PrintRegisteredEvents();
    }

    /// <summary>
    /// Finds and returns the first enabled <see cref="AbstractUIPanel"/> of the specified type
    /// </summary>
    /// <typeparam name="T">The type inheriting from AbstractUIPanel to get</typeparam>
    /// <returns>First AbstractUIPanel of type T. Null if no match was found.</returns>
    public static T GetPanel<T>() where T : AbstractUIPanel
    {
      Type type = typeof(T);
      var panel = InitializedPanels?.Find(p => p != null && type.IsAssignableFrom(p.GetType()));
      if (panel == null)
      {
        ConsoleLogger.LogWarning($"Could not find UIPanel of type {type}");
      }
      return panel as T;
    }

    public static T GetPanel<T>(string panelName) where T : AbstractUIPanel
    {
      Type type = typeof(T);
      var panel = InitializedPanels?.Find(p => p != null && type.IsAssignableFrom(p.GetType()) && p.PanelName == panelName);
      if (panel == null)
      {
        ConsoleLogger.LogWarning($"Could not find UIPanel of type {type} with name {panelName}");
      }
      return panel as T;
    }

    /// <summary>
    /// Finds and returns the first enabled <see cref="AbstractUIPanel"/> of the specified type
    /// </summary>
    /// <typeparam name="type">The type inheriting from AbstractUIPanel to get</typeparam>
    /// <returns>First AbstractUIPanel of the specified type. Null if no match was found.</returns>
    public static AbstractUIPanel GetPanel(Type type, bool includeDerived = false)
    {
      var panel = InitializedPanels?.Find(p => p != null && p.GetType() == type);
      if (panel == null && includeDerived)
      {
        panel = InitializedPanels?.Find(p => p != null && type.IsAssignableFrom(p.GetType()));
      }

      if (panel == null)
      {
        ConsoleLogger.LogWarning($"Could not find UIPanel of type {type.FullName}");
      }
      return panel;
    }

    /// <summary>
    /// Finds and returns all enabled <see cref="AbstractUIPanel"/>s of the specified type
    /// </summary>
    /// <typeparam name="T">The type inheriting from AbstractUIPanel to get</typeparam>
    /// <returns>List with all AbstractUIPanels of type T. 
    /// Empty list if no match was found.</returns>
    public static List<AbstractUIPanel> GetPanels<T>(bool includeDerived = false) where T : AbstractUIPanel
    {
      List<AbstractUIPanel> panels = new List<AbstractUIPanel>();
      Type type = typeof(T);
      InitializedPanels?.ForEach(p =>
      {
        if (p != null)
        {
          Type panelType = p.GetType();
          if (panelType == type || (includeDerived && type.IsAssignableFrom(panelType)))
          {
            panels.Add(p);
          }
        }
      });
      //Instance?.EnabledPanels?.FindAll(p => p.GetType() == typeof(T))
      //                        .ForEach(p => panels.Add(p as T));
      if (panels == null || panels.Count < 1)
      {
        ConsoleLogger.LogWarning($"Could not find UIPanel of type {type}");
      }
      return panels;
    }

    /// <summary>
    /// Finds and returns all enabled <see cref="AbstractUIPanel"/>s of the specified type
    /// </summary>
    /// <typeparam name="type">The type inheriting from AbstractUIPanel to get</typeparam>
    /// <returns>List with all AbstractUIPanels of type <see cref="type"/>. 
    /// Empty list if no match was found.</returns>
    public static List<AbstractUIPanel> GetPanels(Type type, bool includeDerived = false)
    {
      List<AbstractUIPanel> panels = new List<AbstractUIPanel>();
      InitializedPanels?.ForEach(p =>
      {
        if (p != null)
        {
          Type panelType = p.GetType();
          if (panelType == type || (includeDerived && type.IsAssignableFrom(panelType)))
          {
            panels.Add(p);
          }
        }
      });
      if (panels == null || panels.Count < 1)
      {
        ConsoleLogger.LogWarning($"Could not find UIPanel of type {type.FullName}");
      }
      return panels;
    }

    public static void OpenPanel(string panelName = "") => OpenPanel<AbstractUIPanel>(panelName);

    public static void OpenPanel<T>(string panelName = "") where T : AbstractUIPanel
    {
      var panel = string.IsNullOrEmpty(panelName) ? GetPanel<T>() : GetPanel<T>(panelName);
      panel?.OpenNoParams();
    }

    public static void ClosePanel(string panelName = "") => ClosePanel<AbstractUIPanel>(panelName);

    public static void ClosePanel<T>(string panelName = "") where T : AbstractUIPanel
    {
      var panel = string.IsNullOrEmpty(panelName) ? GetPanel<T>() : GetPanel<T>(panelName);
      panel?.CloseNoParams();
    }

    /// <summary>
    /// Closes all <see cref="AbstractUIPanel"/>s that are in <see cref="OpenPanels"/>.
    /// </summary>
    public static void CloseAllPanels(params AbstractUIPanel[] excludedPanels)
    {
      if (OpenPanels != null)
      {
        AbstractUIPanel panel = null;
        // Create a copy of the open panels
        var openPanelsTemp = new List<AbstractUIPanel>(OpenPanels);
        for (int i = 0; i < openPanelsTemp.Count; i++)
        {
          panel = openPanelsTemp[i];

          if (panel != null && !excludedPanels.Contains(panel))
          {
            panel.CloseNoParams();
          }
        }
      }
    }

    /// <summary>
    /// Removes the <see cref="AbstractUIPanel"/> that was closed from the list of open panels
    /// </summary>
    /// <param name="panel">The <see cref="AbstractUIPanel"/>that was closed</param>
    private static void OnPanelInitialize(AbstractUIPanel panel)
    {
      InitializedPanels.AddIfNotContains(panel);
    }

    /// <summary>
    /// Adds the enabled <see cref="AbstractUIPanel"/> to the list of available panels.
    /// </summary>
    /// <param name="panel">The panel that was enabled</param>
    private static void OnPanelEnable(AbstractUIPanel panel)
    {
      EnabledPanels.AddIfNotContains(panel);
    }

    /// <summary>
    /// Removes the disabled <see cref="AbstractUIPanel"/> from the list of available panels.
    /// </summary>
    /// <param name="panel">The panel that was disabled</param>
    private static void OnPanelDisable(AbstractUIPanel panel)
    {
      EnabledPanels.Remove(panel);
      OpenPanels.Remove(panel);
    }

    /// <summary>
    /// Removes the <see cref="AbstractUIPanel"/> that was closed from the list of open panels
    /// </summary>
    /// <param name="panel">The <see cref="AbstractUIPanel"/>that was closed</param>
    private static void OnPanelOpened(AbstractUIPanel panel)
    {
      OpenPanels.AddIfNotContains(panel);
    }

    /// <summary>
    /// Adds the <see cref="AbstractUIPanel"/> that was opened to the list of open panels
    /// </summary>
    /// <param name="panel">The <see cref="AbstractUIPanel"/> that was opened</param>
    private static void OnPanelClosed(AbstractUIPanel panel)
    {
      OpenPanels.Remove(panel);
    }
  }
}