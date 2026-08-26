using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("UI Panel:\nA panel represents a part of the UI that can be opened and closed. " +
              "There are many options to affect the opening and closing capabilities. " +
              "Multiple helper scripts are available to open a panel without the need to " +
              "directly reference it such as by using the panel name. This is possible " +
              "with the help of the 'UIPanelManager' which tracks all panels and comes " +
              "with a lot of ways to find the correct panels based on various criteria.")]
  /// <summary>
  /// Abstract base class for UI panels that can be opened and closed, features
  /// various callbacks and has other useful functionalities.
  /// </summary>
  [SkipObfuscationRename]
  public abstract class AbstractUIPanel : MonoBehaviour, IUIPanel
  {
    [Header("Base Panel")]
    [Space(5)]

    [SerializeField]
    [Tooltip("The name of the panel. Useful if multiple panels of the same type " +
             "need to be distinguishable.")]
    protected string panelName = string.Empty;

    [SerializeField]
    [Tooltip("Whether this panel should be opened when the Start method is called.")]
    protected bool openOnStart = false;

    [SerializeField]
    [Tooltip("Whether this panel should be opened when the OnEnable method is called.")]
    protected bool openOnEnable = false;

    [SerializeField]
    [Tooltip("Whether to prevent this panel from being opened. " +
             "Useful if this panel should only be openable " +
             "if a certain condition is met.")]
    protected bool preventOpening = false;

    [SerializeField]
    [SkipObfuscation]
    [Tooltip("Whether this panel should not be closable.")]
    protected bool keepOpen = false;

    [SerializeField]
    [Tooltip("Whether this panel should close all other panels when this panel has opened. " +
             "There are situations in which a panel might not be closed such as when having 'Keep Open' set to true.")]
    protected bool closeOtherPanelsOnOpen = false;

    [SerializeField]
    [Tooltip("Whether panels that were closed from 'Close Other Panels On Open' " +
             "should be opened again when this panel has closed.")]
    protected bool openPreviouslyClosedPanelsOnClose = true;

    [SerializeField]
    [Tooltip("The delay after which the panel will be opened if 'Open On Start' is enabled.")]
    protected float delayOnStart = 0;

    /// <summary>
    /// List of <see cref="AbstractUIPanel"/>s that when any is open 
    /// block this panel from being opened.
    /// </summary>
    [SerializeField]
    [Tooltip("A list of panels that if any number of them are open " +
             "will prevent this panel from being opened.")]
    protected List<AbstractUIPanel> blockingPanels = new List<AbstractUIPanel>();

    protected List<AbstractUIPanel> panelsToOpenOnClose = new List<AbstractUIPanel>();

    #region Events
    [Header("Events")]
    [Space(5)]

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.FoldoutGroup("Events", Expanded = false)]
#endif
    [Tooltip("Action(s) to invoke when this panel was initialized.")]
    public UnityEvent OnInitialize = new UnityEvent();

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.FoldoutGroup("Events", Expanded = false)]
#endif
    [Tooltip("Action(s) to invoke when this panel was opened.")]
    public UnityEvent OnOpen = new UnityEvent();

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.FoldoutGroup("Events", Expanded = false)]
#endif
    [Tooltip("Action(s) to invoke when this panel was closed.")]
    public UnityEvent OnClose = new UnityEvent();

    /// <summary>
    /// Action that gets invoked by an <see cref="AbstractUIPanel"/> when it gets enabled
    /// </summary>
    public static event Action<AbstractUIPanel> OnPanelEnable;

    /// <summary>
    /// Action that gets invoked by an <see cref="AbstractUIPanel"/> when it gets disabled
    /// </summary>
    public static event Action<AbstractUIPanel> OnPanelDisable;

    /// <summary>
    /// Action that gets invoked by an <see cref="AbstractUIPanel"/> when it gets opened
    /// </summary>
    public static event Action<AbstractUIPanel> OnPanelOpen;

    /// <summary>
    /// Action that gets invoked by an <see cref="AbstractUIPanel"/> when it gets closed
    /// </summary>
    public static event Action<AbstractUIPanel> OnPanelClose;

    /// <summary>
    /// Action that gets invoked by an <see cref="AbstractUIPanel"/> when it gets initialized
    /// </summary>
    public static event Action<AbstractUIPanel> OnPanelInitialize;
    #endregion

    #region Properties
    /// <summary>
    /// The name of the panel. Useful if multiple panels
    /// of the same type need to be distinguishable.
    /// </summary>
    public virtual string PanelName { get => panelName; set => panelName = value; }

    /// <summary>
    /// Determines if this panel is currently open.
    /// Various panel implementations may have different criterias
    /// that specify if a panel is open. By default this will check
    /// if the panel is registered as open in the <see cref="UIPanelManager.OpenPanels"/>
    /// </summary>
    public virtual bool IsOpen
    {
      get
      {
        if (UIPanelManager.OpenPanels != null)
        {
          // The panel is open if it is in the list of open panels of the UIPanelManager
          return UIPanelManager.OpenPanels.Contains(this);
        }
        // Return false by default
        return false;
      }
    }

    /// <summary>
    /// Determines if this panel can currently be opened.
    /// Things that can prevent this panel from being opened could be
    /// that this panel is already open or that another <see cref="AbstractUIPanel"/> 
    /// is currently that is in this panels <see cref="BlockingPanels"/>.
    /// </summary>
    public virtual bool CanOpen
    {
      get
      {
        if (IsOpen) { return false; }
        if (BlockingPanels == null || BlockingPanels.Count < 1)
        {
          return true;
        }
        return BlockingPanels.Find(p => p.IsOpen) == null;
      }
    }

    /// <summary>
    /// List of <see cref="AbstractUIPanel"/>s that when any is open 
    /// block this panel from being opened.
    /// </summary>
    public virtual List<AbstractUIPanel> BlockingPanels
    {
      get { return blockingPanels; }
      protected set { blockingPanels = value; }
    }

    [SkipObfuscation]
    public bool PreventOpening { get => preventOpening; set => preventOpening = value; }

    public bool KeepOpen
    {
      get
      {
        if (Application.isPlaying && ApplicationQuitListener.isQuitting)
        {
          return false;
        }
        return keepOpen;
      }
      set => keepOpen = value;
    }
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
      Init();
      // This show is required for some panels
      // to have their data properly initialized
      Show(true);
      CloseNoParams();

      // Check if the panel is still open despite the 
      // attempt to close it
      if (IsOpen)
      {
        if (KeepOpen)
        {
          var cachedCloseOtherPanels = closeOtherPanelsOnOpen;
          var cachedOpenPreviouslyClosedPanels = openPreviouslyClosedPanelsOnClose;
          closeOtherPanelsOnOpen = false;
          openPreviouslyClosedPanelsOnClose = false;

          // In the case the panel can not be closed we
          // invoke the OnPanelOpened event because the
          // open method was not used to show/open the
          // panel which would normally invoke the event.
          OnPanelOpened();

          closeOtherPanelsOnOpen = cachedCloseOtherPanels;
          openPreviouslyClosedPanelsOnClose = cachedOpenPreviouslyClosedPanels;
        }
        else
        {
          // In the case the panel is still open but
          // should not, we simply hide it.
          Show(false);
        }
      }
    }

    protected virtual void Start()
    {
      if (openOnStart)
      {
        if (delayOnStart > 0)
        {
          // If it is the start of the application we add 2 frames delay to make
          // sure all system are properly initialized.
          var delay = Mathf.Clamp(delayOnStart, Time.fixedDeltaTime * 2, delayOnStart);
          Invoke(nameof(OpenNoParams), delay);
        }
        else
        {
          // If it is the start of the application we add 2 frames delay to make
          // sure all system are properly initialized.
          if (Time.time == 0)
          {
            ScheduleUtility.InvokeDelayedByFrames(OpenNoParams, 2);
          }
          else
          {
            OpenNoParams();
          }
        }
      }
    }

    protected virtual void OnEnable()
    {
      OnPanelEnabled();

      if (openOnEnable)
      {
        // If it is the start of the application we add 2 frames delay to make
        // sure all system are properly initialized.
        if (Time.time == 0)
        {
          ScheduleUtility.InvokeDelayedByFrames(OpenNoParams, 2);
        }
        else
        {
          OpenNoParams();
        }
      }

      GlobalEventHandler.AddEventListener("OnApplicationQuit", CloseNoParams);
    }

    protected virtual void OnDisable()
    {
      CloseNoParams();
      OnPanelDisabled();
      GlobalEventHandler.RemoveEventListener("OnApplicationQuit", CloseNoParams);
    }
    #endregion

    #region Panel Methods

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void StaticInit()
    {
      // Null the static events so all listeners are reset/removed
      OnPanelInitialize = null;

      OnPanelEnable = null;
      OnPanelDisable = null;

      OnPanelOpen = null;
      OnPanelClose = null;

      //PrintRegisteredEvents();
    }

    //public static void PrintRegisteredEvents()
    //{
    //  var c1 = OnPanelOpen?.GetInvocationList()?.Count();
    //  ConsoleLogger.Log($"Open: {(c1 != null ? (int)c1 : 0)} events registered");

    //  var c2 = OnPanelClose?.GetInvocationList()?.Count();
    //  ConsoleLogger.Log($"Close: {(c2 != null ? (int)c2 : 0)} events registered");
    //}

    protected virtual void Init()
    {
      OnInitialized();
    }

    [SkipObfuscation]
    public void SetPreventOpening(bool preventOpening)
    {
      this.preventOpening = preventOpening;
    }

    /// <summary>
    /// Opens this panel.
    /// Optional input parameter(s) can be provided.
    /// If no parameters are needed the use 
    /// of <see cref="OpenNoParams"/> is recommended as
    /// it can also be subscribed to events or <see cref="UnityEvent"/>s
    /// and calls this method internally.
    /// Invokes all events related to opening a panel.
    /// </summary>
    /// <param name="input">Optional input parameter(s)</param>
    public virtual void Open(params object[] input)
    {
      if (CanOpen && !PreventOpening)
      {
        Show(true);
        OnPanelOpened();
        //this.InvokeDelayedByFrames(() => Show(true), 2);
      }
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ButtonGroup("Open & Close")]
#endif
    [Button]
    [ContextMenu("Open")]
    [SkipObfuscation]
    /// <summary>
    /// Equivalent to <see cref="Open(object[])"/> without any parameters.
    /// Invokes all events related to opening a panel.
    /// </summary>
    public virtual void OpenNoParams()
    {
      Open();
    }

    /// <summary>
    /// Closes this panel.
    /// Optional input parameter(s) can be provided.
    /// If no parameters are needed the use 
    /// of <see cref="CloseNoParams"/> is recommended as
    /// it can also be subscribed to events or <see cref="UnityEvent"/>s
    /// and calls this method internally.
    /// Invokes all events related to closing a panel.
    /// </summary>
    /// <param name="input">Optional input parameter(s)</param>
    public virtual void Close(params object[] input)
    {
      if (IsOpen && !KeepOpen)
      {
        OnPanelClosed();
        Show(false);
      }
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ButtonGroup("Open & Close")]
#endif
    [Button]
    [ContextMenu("Close")]
    [SkipObfuscation]
    /// <summary>
    /// Equivalent to <see cref="Close(object[])"/> without any parameters.
    /// Invokes all events related to closing a panel.
    /// </summary>
    public virtual void CloseNoParams()
    {
      Close();
    }

    [Button]
    [ContextMenu("Toggle")]
    [SkipObfuscationRename]
    /// <summary>
    /// Opens or closes the panel based on the current open state.
    /// </summary>
    public virtual void Toggle()
    {
      if (IsOpen) { CloseNoParams(); }
      else { OpenNoParams(); }
    }

    [Button]
    [SkipObfuscation]
    public virtual void ChangeOpenState(bool open)
    {
      if (open) { OpenNoParams(); }
      else { CloseNoParams(); }
    }

    /// <summary>
    /// Shows or hides this panel.
    /// </summary>
    /// <param name="show">Whether this panel should be shown or hidden</param>
    protected abstract void Show(bool show);

    /// <summary>
    /// Invoked when this panel is enabled
    /// </summary>
    protected virtual void OnPanelEnabled()
    {
      OnPanelEnable?.Invoke(this);
    }

    /// <summary>
    /// Invoked when this panel is disabled
    /// </summary>
    protected virtual void OnPanelDisabled()
    {
      OnPanelDisable?.Invoke(this);
    }

    /// <summary>
    /// Invoked when this panel is opened
    /// </summary>
    protected virtual void OnPanelOpened()
    {
      OnPanelOpen?.Invoke(this);
      OnOpen?.Invoke();
      GlobalEventHandler.InvokeEvent("OnPanelOpened", this);
      if (closeOtherPanelsOnOpen)
      {
        if (openPreviouslyClosedPanelsOnClose)
        {
          panelsToOpenOnClose.AddRange(UIPanelManager.OpenPanels);
        }
        UIPanelManager.CloseAllPanels(this);
      }
      //this.InvokeDelayedByFrames(() => ShowCursorIfOpen(true));
    }

    /// <summary>
    /// Invoked when this panel is closed
    /// </summary>
    protected virtual void OnPanelClosed()
    {
      OnPanelClose?.Invoke(this);
      OnClose?.Invoke();
      GlobalEventHandler.InvokeEvent("OnPanelClosed", this);

      if (openPreviouslyClosedPanelsOnClose)
      {
        panelsToOpenOnClose.ForEach(p =>
        {
          if (p != null)
          {
            p.OpenNoParams();
          }
        });
      }
      panelsToOpenOnClose.Clear();
    }

    /// <summary>
    /// Invoked when this panel is initialized
    /// </summary>
    protected virtual void OnInitialized()
    {
      OnPanelInitialize?.Invoke(this);
      OnInitialize?.Invoke();
      GlobalEventHandler.InvokeEvent("OnPanelInitialized", this);
    }

    //protected void ShowCursorIfOpen(bool show)
    //{
    //  if (IsOpen)
    //  {
    //    CursorUtility.ShowCursor(show);
    //  }
    //}
    #endregion
  }
}