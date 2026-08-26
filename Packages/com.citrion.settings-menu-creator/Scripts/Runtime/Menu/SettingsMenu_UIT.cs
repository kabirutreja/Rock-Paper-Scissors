using CitrioN.Common;
using CitrioN.UI.UIToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscationRename]
  [HeaderInfo("\n\nSettings Menu:\nThe settings menu script is the central control for most " +
              "of the runtime aspects of a UI Toolkit settings menu. Using the referenced 'SettingsCollection' " +
              "it can save & load settings. It also manages the creation and initilization of required " +
              "UI elements for the settings in the SettingsCollection. " +
              "The layout/visuals for a settings menu can also be referenced and instantiated at runtime.")]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Settings Menu (UI Toolkit)")]
  public class SettingsMenu_UIT : UIT_Panel
  {
    #region Fields & Properties

    [Header("Settings Menu")]
    [Space(5)]

    [SerializeField]
    [Tooltip("Whether the values of settings should be loaded from a save file when the menu elements are added.")]
    protected bool loadOnCreate = true;

    [SerializeField]
    [Tooltip("Whether setting value changes that have not been applied " +
             "should be applied and saved when the menu is being closed.")]
    protected bool applyAndSaveOnClose = true;

    [SerializeField]
    [Tooltip("The SettingsCollection to use for the menu. " +
             "The collection contains all the settings that should be managed by this menu." +
             "It contains all the relevant information on how to create and connect its settings to " +
             "generated or existing input elements of the menu. Saving and loading will also be forwarded to this " +
             "collection.")]
    protected SettingsCollection settingsCollection;

    //[SerializeField]
    protected SettingsCollection runtimeSettings;

    protected List<VisualElement> inputElements = new List<VisualElement>();

    protected Dictionary<VisualElement, SettingHolder> settingObjects = new Dictionary<VisualElement, SettingHolder>();

    protected Dictionary<VisualElement, bool> inputElementWasGenerated = new Dictionary<VisualElement, bool>();

    protected List<VisualElement> selectableInputElements = new List<VisualElement>();

    protected int previousQualityLevel;

    protected Coroutine qualityLevelCoroutine;

    public SettingsCollection SettingsTemplate
    {
      get => settingsCollection;
      set => settingsCollection = value;
    }

    public List<VisualElement> InputElements { get => inputElements; set => inputElements = value; }

    public Dictionary<VisualElement, SettingHolder> SettingObjects
    {
      get => settingObjects;
      set => settingObjects = value;
    }

    public Dictionary<VisualElement, bool> InputElementWasGenerated
    {
      get => inputElementWasGenerated;
      set => inputElementWasGenerated = value;
    }

#if UNITY_EDITOR
    //[SerializeField]
    //private bool autoRefreshMenu = false;
#endif

#if UNITY_EDITOR
    private int dirtyCount = 0;
    private int currentDirtyCount = 0;
#endif

    #endregion

    protected virtual void OnDestroy()
    {
      UnregisterEvents();
    }

    // TODO Enable once live refresh is working
    //    protected virtual void Update()
    //    {
    //#if UNITY_EDITOR
    //      if (autoRefreshMenu)
    //      {
    //        RefreshIfDirtyChanged();
    //      }
    //#endif
    //    }

    protected override void Init()
    {
      RegisterEvents();
      InitializeSettings();
      //if (runtimeSettings != null)
      //{
      //  runtimeSettings.SaveSettings(isDefault: true);
      //}
      //else
      //{
      //  ConsoleLogger.LogWarning($"A {nameof(SettingsCollection)} reference is required for " +
      //                           $"a menu to be created.");
      //  return;
      //}
      InputElements.Clear();
      SettingObjects.Clear();
      InputElementWasGenerated.Clear();
      base.Init();
    }

    protected void RegisterEvents()
    {
      //GlobalEventHandler.AddEventListener<Setting, string, SettingsCollection, object>
      //  (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnAnySettingChanged);
#if UNITY_2022_1_OR_NEWER
      QualitySettings.activeQualityLevelChanged += OnActiveQualityLevelChanged;
#endif
    }

    private void UnregisterEvents()
    {
      //GlobalEventHandler.RemoveEventListener<Setting, string, SettingsCollection, object>
      //  (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnAnySettingChanged);
#if UNITY_2022_1_OR_NEWER
      QualitySettings.activeQualityLevelChanged -= OnActiveQualityLevelChanged;
#endif
    }

    private void OnActiveQualityLevelChanged(int previousLevel, int newLevel)
    {
      UpdateAllSettingFields();
    }

    protected void InitializeSettings()
    {
      if (SettingsTemplate != null)
      {
        runtimeSettings = SettingsTemplate;
        //runtimeSettings = Instantiate(SettingsTemplate);
        runtimeSettings.Initialize();
        runtimeSettings.onSettingUpdated += OnSettingUpdated;
      }
    }

    private void OnSettingUpdated(string settingIdentifier)
    {
      UpdateSettingsField(settingIdentifier);
    }

    private void UpdateSettingsField(string settingIdentifier)
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.UpdateSettingField(Root, settingIdentifier);
    }

    [ContextMenu("Update All Input Elements")]
    private void UpdateAllSettingFields()
    {
      if (runtimeSettings == null || runtimeSettings.Settings == null) { return; }
      runtimeSettings.Settings.ForEach(s => UpdateSettingsField(s.Identifier));
    }

    protected override void AddElements()
    {
      base.AddElements();

      UpdateSettingsFields(true);
      if (runtimeSettings != null && !runtimeSettings.HasDefaultsLoadedOnce)
      {
        LoadDefaultSettings();
      }
      if (loadOnCreate)
      {
        LoadSettings();
      }
    }

    protected override void OnPanelOpened()
    {
      UpdateSettingsFields(false);

      base.OnPanelOpened();

#if !UNITY_2022_1_OR_NEWER
      if (qualityLevelCoroutine != null)
      {
        StopCoroutine(qualityLevelCoroutine);
      }

      qualityLevelCoroutine = StartCoroutine(DetectQualityLevelChange());
#endif

      //SetupMenuNavigation();
    }

    protected override void OnPanelClosed()
    {
      base.OnPanelClosed();

#if !UNITY_2022_1_OR_NEWER
      if (qualityLevelCoroutine != null)
      {
        StopCoroutine(qualityLevelCoroutine);
      }
#endif

      if (applyAndSaveOnClose)
      {
        ApplyPendingSettingChanges();
        SaveSettings();
      }

      //#if UNITY_EDITOR
      //      if (ApplicationQuitListener.isQuitting && runtimeSettings != null)
      //      {
      //        // Restore the original values from before play mode started
      //        // Mostly relevant for post processing settings because they are
      //        // otherwise not reverted on the profile
      //        runtimeSettings.RestoreStartValues();
      //      } 
      //#endif
    }

    protected IEnumerator DetectQualityLevelChange()
    {
      int currentQualityLevel;
      while (true)
      {
        currentQualityLevel = QualitySettings.GetQualityLevel();
        if (currentQualityLevel != previousQualityLevel)
        {
          OnActiveQualityLevelChanged(previousQualityLevel, currentQualityLevel);
          previousQualityLevel = currentQualityLevel;
        }
        yield return null;
      }
    }

    protected void UpdateSettingsFields(bool initialize)
    {
      if (Root != null && runtimeSettings != null)
      {
        if (runtimeSettings.InputElementProviders_UIT == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UIT)} reference assigned!");
          return;
        }
        bool inputElementWasGenerated = false;
        foreach (var s in runtimeSettings.Settings)
        {
          if (s == null || s.Setting == null) { continue; }

          if (!s.Setting.InitializedForRuntime)
          {
            s.Setting.InitializeForRuntime(runtimeSettings);
          }

          inputElementWasGenerated = false;
          var elem = s.FindElement_UIToolkit(Root, runtimeSettings);
          if (elem == null)
          {
            elem = s.CreateElement_UIToolkit(Root, runtimeSettings);
            inputElementWasGenerated = true;
          }
          if (elem != null)
          {
            InputElements.AddIfNotContains(elem);
            if (!InputElementWasGenerated.ContainsKey(elem))
            {
              InputElementWasGenerated.Add(elem, inputElementWasGenerated);
            }
            SettingObjects.AddOrUpdateDictionaryItem(elem, s);
            initialize = initialize || inputElementWasGenerated;
            s.InitializeElement_UIToolkit(elem, runtimeSettings, initialize);
            if (initialize)
            {
              var defaultValue = s.Setting.GetDefaultValue(runtimeSettings);
              bool hasActiveValue = runtimeSettings.activeSettingValues.TryGetValue(s.Identifier, out var value);
              if (!hasActiveValue && !s.Setting.SkipApplyingDefault && defaultValue != null)
              {
                runtimeSettings.ApplySettingChange(s.Identifier, true, true, defaultValue);
              }
            }
          }
          else
          {
            ConsoleLogger.LogWarning($"Unable to find or create an input field for setting: " +
                         $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
          }
        }
      }
    }

    [SkipObfuscationRename]
    [ContextMenu("Apply Settings Changes")]
    public void ApplyPendingSettingChanges()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.ApplyPendingSettingsChanges();
    }

    [SkipObfuscationRename]
    public void PrintCurrentValues()
    {
      foreach (var item in runtimeSettings.Settings)
      {
        item.Setting.GetCurrentValues(runtimeSettings);
      }
    }

    [SkipObfuscationRename]
    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.SaveSettings();
    }

    [SkipObfuscationRename]
    [ContextMenu("Load Default Settings")]
    public void LoadDefaultSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.LoadSettings(isDefault: true, apply: true, forceApply: true);
    }

    [SkipObfuscationRename]
    [ContextMenu("Load Settings")]
    public void LoadSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.LoadSettings(isDefault: false, apply: true, forceApply: true);
    }

    //[ContextMenu("Remove Input Elements")]
    //protected void RemoveElements()
    //{
    //  for (int i = 0; i < inputElements.Count; i++)
    //  {
    //    inputElements[i].RemoveFromHierarchy();
    //  }

    //  inputElements.Clear();
    //  settingObjects.Clear();
    //}

    //[ContextMenu("Refresh Settings")]
    //protected void RefreshSettings()
    //{
    //  RemoveElements();
    //  InitializeSettings();
    //  AddElements();

    //  // TODO Remove/Refactor why OnPanelOpened is called?!
    //  OnPanelOpened();
    //}

    [ContextMenu("Remove Generated Input Elements")]
    protected void RemoveGeneratedInputElements()
    {
      List<VisualElement> elementsToRemove = new List<VisualElement>();

      for (int i = 0; i < InputElements.Count; i++)
      {
        var elem = InputElements[i];
        if (InputElementWasGenerated.TryGetValue(elem, out var wasGenerated))
        {
          if (wasGenerated)
          {
            elementsToRemove.Add(elem);
          }
        }
        else
        {
          ConsoleLogger.LogWarning("Input element not being tracked!");
        }
      }

      for (int i = 0; i < elementsToRemove.Count; i++)
      {
        var e = elementsToRemove[i];

        if (e == null)
        {
          ConsoleLogger.LogWarning("Can't remove an input element that is null!", Common.LogType.EditorOnly);
          continue;
        }

        InputElements.Remove(e);
        SettingObjects.Remove(e);
        InputElementWasGenerated.Remove(e);
        e.RemoveFromHierarchy();
      }

      //InputElements.Clear();
      //settingObjects.Clear();
    }

    [ContextMenu("Refresh Settings")]
    public void RefreshSettings()
    {
      bool load = loadOnCreate && runtimeSettings == null;
      RebuildMenu(load);
    }

    //[ContextMenu("Refresh Settings")]
    //public void RefreshSettings()
    //{
    //  RemoveElements();
    //  bool load = loadOnCreate && runtimeSettings == null;
    //  if (runtimeSettings != null)
    //  {
    //    var currentRuntimeSettings = runtimeSettings;
    //    InitializeSettings();
    //    runtimeSettings.startValues = currentRuntimeSettings.startValues;
    //    runtimeSettings.pendingSettingChanges = currentRuntimeSettings.pendingSettingChanges;
    //    runtimeSettings.activeSettingValues = currentRuntimeSettings.activeSettingValues;
    //  }

    //  #region Add Elements
    //  base.AddElements();
    //  UpdateSettingsFields(true);
    //  LoadDefaultSettings();
    //  if (load)
    //  {
    //    LoadSettings();
    //  }
    //  #endregion

    //  // TODO Remove/Refactor why OnPanelOpened is called?!
    //  OnPanelOpened();
    //}

    [ContextMenu("Delete Save")]
    public void DeleteSave()
    {
      if (runtimeSettings != null)
      {
        runtimeSettings.DeleteSave();
      }
    }

    public void RebuildMenu(bool load)
    {
      RemoveGeneratedInputElements();

      //RemoveMenuHierarchy();
      //CreateAndAttachMenuHierarchy();
      if (runtimeSettings != null)
      {
        var currentRuntimeSettings = runtimeSettings;
        InitializeSettings();
        runtimeSettings.startValues = currentRuntimeSettings.startValues;
        runtimeSettings.pendingSettingChanges = currentRuntimeSettings.pendingSettingChanges;
        runtimeSettings.activeSettingValues = currentRuntimeSettings.activeSettingValues;
      }
      UpdateSettingsFields(false);
      //AddElements(load);

      // TODO Remove/Refactor why is OnPanelOpened called?!
      if (IsOpen)
      {
        OnPanelOpened();
      }
    }

#if UNITY_EDITOR
    private void RefreshIfDirtyChanged()
    {
      if (runtimeSettings == null) { return; }

      dirtyCount = UnityEditor.EditorUtility.GetDirtyCount(runtimeSettings);
      if (currentDirtyCount < dirtyCount)
      {
        currentDirtyCount = dirtyCount;
        RefreshSettings();
      }
    }
#endif
  }
}
