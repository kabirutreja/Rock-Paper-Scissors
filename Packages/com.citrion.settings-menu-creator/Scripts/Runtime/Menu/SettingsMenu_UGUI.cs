using CitrioN.Common;
using CitrioN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscationRename]
  [HeaderInfo("\n\nSettings Menu:\nThe settings menu script is the central control for most " +
              "of the runtime aspects of a UGUI settings menu. Using the referenced 'SettingsCollection' " +
              "it can save & load settings. It also manages the creation and initilization of required " +
              "input elements for the settings in the SettingsCollection. " +
              "The layout/visuals for a settings menu can also be referenced and instantiated at runtime.")]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Settings Menu (UGUI)")]
  public class SettingsMenu_UGUI : CanvasUIPanel
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
    [Tooltip("Whether applied setting values should be saved when the menu is being closed.")]
    protected bool saveOnClose = false;

    [SerializeField]
    [Tooltip("The prefab (for the menu layout) that should be instantiated and attached when this panel is initialized. " +
             "This can be left empty if the menu layout hierarchy is already attached during edit time. The advantage of " +
             "runtime creation and parenting is that the menu prefab " +
             "only needs a different prefab reference to use a completely different layout.")]
    protected GameObject menuTemplate;

    [SerializeField]
    [Tooltip("The SettingsCollection to use for the menu. " +
             "The collection contains all the settings that should be managed by this menu." +
             "It contains all the relevant information on how to create and connect its settings to " +
             "generated or existing input elements of the menu. Saving and loading will also be forwarded to this " +
             "collection.")]
    protected SettingsCollection settingsCollection;

    //[SerializeField]
    protected SettingsCollection runtimeSettings;

    protected RectTransform root;

    protected GameObject menuLayoutInstance = null;

    private List<RectTransform> inputElements = new List<RectTransform>();

    protected Dictionary<RectTransform, SettingHolder> settingObjects = new Dictionary<RectTransform, SettingHolder>();

    protected Dictionary<RectTransform, bool> inputElementWasGenerated = new Dictionary<RectTransform, bool>();

    protected int previousQualityLevel;

    protected Coroutine qualityLevelCoroutine;

    public GameObject MenuTemplate { get => menuTemplate; set => menuTemplate = value; }

    public SettingsCollection SettingsTemplate { get => settingsCollection; set => settingsCollection = value; }

    public RectTransform Root { get => root; protected set => root = value; }
    public List<RectTransform> InputElements { get => inputElements; set => inputElements = value; }
    public SettingsCollection RuntimeSettings { get => runtimeSettings; set => runtimeSettings = value; }
    public Dictionary<RectTransform, SettingHolder> SettingObjects { get => settingObjects; set => settingObjects = value; }
    public Dictionary<RectTransform, bool> InputElementWasGenerated { get => inputElementWasGenerated; set => inputElementWasGenerated = value; }

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("If enabled the menu will be refreshed if settings on the SettingsCollection are modified. " +
             "This includes adding/removing a setting or reordering the list of settings." +
             "\n\nEditor Only")]
    private bool autoRefreshMenu = false;
#endif

#if UNITY_EDITOR
    private int dirtyCount = 0;
    private int currentDirtyCount = 0;
    private float lastRefresh = 0;
#endif
    #endregion

    protected virtual void OnDestroy()
    {
      UnregisterEvents();
    }

    protected virtual void Update()
    {
#if UNITY_EDITOR
      if (autoRefreshMenu)
      {
        RefreshIfDirtyChanged();
      }
#endif
    }

    protected override void Init()
    {
      CreateAndAttachMenuHierarchy();
      RegisterEvents();
      InitializeSettings();
      if (runtimeSettings != null)
      {
        //runtimeSettings.SaveSettings(isDefault: true);


        // TODO Is this still required because there is a fallback now?
        //if (runtimeSettings.InputElementProviders_UGUI == null)
        //{
        //  ConsoleLogger.LogWarning($"An {nameof(InputElementProviders_UGUI)} reference is " +
        //                           $"required for input elements to be created.");
        //  return;
        //}
      }
      else
      {
        ConsoleLogger.LogWarning($"A {nameof(SettingsCollection)} reference is required for " +
                                 $"a menu to be created.");
        return;
      }
      InputElements.Clear();
      SettingObjects.Clear();
      InputElementWasGenerated.Clear();
      base.Init();
      if (Canvas != null)
      {
        root = Canvas.GetComponent<RectTransform>();
      }
      //Root = RootObject.GetComponent<RectTransform>();
      AddElements(loadOnCreate);
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

    //private void OnUnitySettingChangedWithValue(string settingType, object newValue)
    //{
    //  ConsoleLogger.Log($"Changed Unity Settings: {settingType} ({newValue})", Common.LogType.Debug);

    //  if (settingType == qualityLevelSettingTypeName)
    //  {
    //    UpdateAllSettingFields();
    //  }

    //  //if (runtimeSettings?.Settings != null)
    //  //{
    //  //  foreach (var settingHolder in runtimeSettings.Settings)
    //  //  {
    //  //    var setting = settingHolder.Setting;
    //  //    if (setting != null && setting is QualitySetting qualitySetting)
    //  //    {
    //  //      qualitySetting.unitySetting?.OnAnyUnitySettingChanged(settingType, newValue);
    //  //    }
    //  //  }
    //  //}
    //}

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

    private void CreateAndAttachMenuHierarchy()
    {
      if (menuTemplate != null && Canvas != null)
      {
        menuLayoutInstance = Instantiate(menuTemplate, Canvas.transform);
        // We need to update the local scale
        // because Unity sets it to 0 for some reason
        menuLayoutInstance.transform.localScale = Vector3.one;
        //Instantiate(menuTemplate, RootObject.transform);
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

    protected void AddElements(bool loadOnCreate)
    {
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
      else if (saveOnClose)
      {
        SaveSettings();
      }

      // TODO Implement with working save/load mechanic not interfering
      //#if UNITY_EDITOR
      //      if (ApplicationQuitListener.isQuitting && runtimeSettings != null)
      //      {
      //        // Restore the original values from before play mode started
      //        // Mostly relevant for post processing settings because they are
      //        // otherwise not reverted on the profile
      //        //EditorApplication.delayCall += runtimeSettings.RestoreStartValues;
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

    protected void SetupMenuNavigation()
    {
      if (InputElements.Count > 0)
      {
        Selectable firstSelectable = null;

        var selectableElements = InputElements.Where(elem => elem.GetComponentInChildren<Selectable>() != null)
                                              .Select(elem => elem.GetComponentInChildren<Selectable>()).ToList();

        //var elementsCount = inputElements.Count;
        var elementsCount = selectableElements.Count;

        for (int i = elementsCount - 1; i >= 0; i--)
        {
          var selectable = selectableElements[i];
          //var element = inputElements[i];
          //var selectable = element != null ? element.GetComponentInChildren<Selectable>() : null;
          //if (selectable != null)
          {
            firstSelectable = selectable;

            //var previousElement = i > 0 ? inputElements[i - 1] : inputElements[elementsCount - 1];
            //var previousSelectable = previousElement.GetComponentInChildren<Selectable>();
            var previousElement = i > 0 ? selectableElements[i - 1] : selectableElements[elementsCount - 1];
            Selectable previousSelectable = previousElement;

            //var nextElement = i < elementsCount - 1 ? inputElements[i + 1] : inputElements[0];
            //var nextSelectable = nextElement.GetComponentInChildren<Selectable>();
            var nextElement = i < elementsCount - 1 ? selectableElements[i + 1] : selectableElements[0];
            var nextSelectable = nextElement;

            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;

            navigation.selectOnLeft = selectable.FindSelectableOnLeft();
            navigation.selectOnRight = selectable.FindSelectableOnRight();
            navigation.selectOnUp = previousSelectable;
            navigation.selectOnDown = nextSelectable;

            selectable.navigation = navigation;
          }
        }

        if (firstSelectable != null)
        {
          EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
          //break;
        }
      }
    }

    protected void UpdateSettingsFields(bool initialize)
    {
      if (Root != null && runtimeSettings != null)
      {
        if (runtimeSettings.InputElementProviders_UGUI == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UGUI)} reference assigned!");
          return;
        }
        bool parented = false;
        bool inputElementWasGenerated = false;
        SettingObject currentParent = null;

        Dictionary<string, List<string>> parentChildrenMapping = new Dictionary<string, List<string>>();
        //var settingsMapping = runtimeSettings.Settings.ToDictionary(x => x.Identifier, x => x);
        var settingsMapping = runtimeSettings.Settings.GroupBy(x => x.Identifier).ToDictionary(g => g.Key, g => g.First());

        foreach (var setting in runtimeSettings.Settings)
        {
          if (setting == null) { continue; }
          string parentIdentifier = setting.InputElementProviderSettings.ParentIdentifier;
          if (string.IsNullOrEmpty(parentIdentifier)) { continue; }
          if (!parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
          {
            children = new List<string>() { setting.Identifier };
            parentChildrenMapping.Add(parentIdentifier, children);
          }
          else
          {
            children.Add(setting.Identifier);
          }
        }

        foreach (var s in runtimeSettings.Settings)
        {
          if (s == null || s.Setting == null) { continue; }
          currentParent = null;

          if (!s.Setting.InitializedForRuntime)
          {
            s.Setting.InitializeForRuntime(runtimeSettings);
          }

          inputElementWasGenerated = false;
          var elem = s.FindElement_UGUI(Root, runtimeSettings);
          if (elem == null)
          {
            elem = s.CreateElement_UGUI(Root, runtimeSettings);
            inputElementWasGenerated = elem != null;
          }
          if (elem != null)
          {
            InputElements.AddIfNotContains(elem);
            if (!InputElementWasGenerated.ContainsKey(elem))
            {
              InputElementWasGenerated.Add(elem, inputElementWasGenerated);
            }
            SettingObjects.AddOrUpdateDictionaryItem(elem, s);

            // TODO Move this into a common/shared method as it is also used in the SettingsCollectionEditorNew?!
            if (inputElementWasGenerated)
            {
              var parentIdentifier = s.InputElementProviderSettings?.ParentIdentifier;
              if (parentIdentifier == null) { continue; }

              parented = false;

              // Unparent element so we can ensure it's not interfering with the index checking.
              // We will reparent it later.
              if (elem != null && elem.parent != null)
              {
                currentParent = elem.parent.GetComponent<SettingObject>();
              }
              elem.SetParent(null);

              if (parentChildrenMapping.TryGetValue(parentIdentifier, out var children))
              {
                if (children != null)
                {
                  // Get current child index
                  var childIndex = children.IndexOf(s.Identifier);

                  if (childIndex != -1 && childIndex >= 0 && childIndex < children.Count)
                  {
                    if (childIndex > 0)
                    {
                      // Iterate over the children before this one and try parenting it after the first one parented correctly
                      for (int j = childIndex - 1; j >= 0; j--)
                      {
                        var indexBefore = j;

                        //var indexBefore = childIndex - 1;
                        if (indexBefore >= children.Count || indexBefore < 0) { continue; }
                        var settingIdentifierBefore = children[indexBefore];
                        if (settingsMapping.TryGetValue(settingIdentifierBefore, out var settingBefore))
                        {
                          var siblingElement = settingBefore?.FindElement_UGUI(root, runtimeSettings);
                          if (siblingElement != null)
                          {
                            var siblingParentObj = siblingElement.parent;
                            if (siblingParentObj != null)
                            {
                              var parentSettingObject = siblingParentObj.GetComponentInParent<SettingObject>();

                              if (parentSettingObject == null) { continue; }

                              bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
                              if (isParentedCorrectly)
                              {
                                var siblingIndex = siblingElement.GetSiblingIndex();
                                if (siblingElement != null && siblingElement.parent != null)
                                {
                                  elem.SetParent(siblingElement.parent.transform);
                                }
                                else
                                {
                                  elem.SetParent(null);
                                }
                                elem.SetSiblingIndex(siblingIndex + 1);
                                parented = true;
                                break;
                              }
                            }
                          }
                        }
                      }
                    }

                    // Check if parenting was successful
                    if (!parented)
                    {
                      // Iterate over the children after this one and try parenting it before the first one parented correctly
                      for (int j = childIndex + 1; j < children.Count; j++)
                      {
                        var indexAfter = j;

                        if (indexAfter >= children.Count || indexAfter < 0) { continue; }
                        var settingIdentifierAfter = children[indexAfter];
                        if (settingsMapping.TryGetValue(settingIdentifierAfter, out var settingBefore))
                        {
                          var siblingElement = settingBefore?.FindElement_UGUI(root, runtimeSettings);
                          if (siblingElement != null)
                          {
                            var siblingParentObj = siblingElement.parent;
                            if (siblingParentObj != null)
                            {
                              var parentSettingObject = siblingParentObj.GetComponent<SettingObject>();

                              if (parentSettingObject == null) { continue; }

                              bool isParentedCorrectly = parentSettingObject.Identifier == settingBefore.InputElementProviderSettings.ParentIdentifier;
                              if (isParentedCorrectly)
                              {
                                var siblingIndex = siblingElement.GetSiblingIndex();
                                if (siblingElement != null && siblingElement.parent != null)
                                {
                                  elem.SetParent(siblingElement.parent.transform);
                                }
                                else
                                {
                                  elem.SetParent(null);
                                }
                                elem.SetSiblingIndex((siblingIndex - 1).ClampLowerTo0());
                                parented = true;
                                break;
                              }
                            }
                          }
                        }
                      }
                    }

                    if (!parented)
                    {
                      var settingIdentifiers = root.GetComponentsInChildren<SettingObject>(true, true);
                      var parentElement = settingIdentifiers?.Find(si => si.Identifier == parentIdentifier);
                      if (parentElement != null)
                      {
                        Transform parentTransform = parentElement.GetContentParent();
                        if (parentTransform != null)
                        {
                          elem.SetParent(parentTransform, false);
                          //continue;
                        }
                      }
                    }

                    //DestroyImmediate(elem.gameObject);
                    //ConsoleLogger.LogWarning($"Unable to parent {s.Identifier}");
                  }
                }
              }
            }

            // TODO Check if initializing the generated input element does interfere with anything
            initialize = initialize || inputElementWasGenerated;
            s.InitializeElement_UGUI(elem, runtimeSettings, initialize);

            if (initialize)
            {
              var defaultValue = s.Setting.GetDefaultValue(runtimeSettings);
              bool hasActiveValue = runtimeSettings.activeSettingValues.TryGetValue(s.Identifier, out var value);
              if (!hasActiveValue)
              {
                hasActiveValue = runtimeSettings.tempSettingValues.TryGetValue(s.Identifier, out value);
              }
              if (!hasActiveValue && !s.Setting.SkipApplyingDefault && defaultValue != null)
              {
                runtimeSettings.ApplySettingChange(s.Identifier, true, true, defaultValue);
              }
            }
          }
          else
          {
            ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
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

    [ContextMenu("Remove Generated Input Elements")]
    protected void RemoveGeneratedInputElements()
    {
      List<RectTransform> elementsToRemove = new List<RectTransform>();

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
        DestroyImmediate(e.gameObject);
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
      if (runtimeSettings != null)
      {
        runtimeSettings.tempSettingValues.Clear();
        foreach (var s in runtimeSettings.Settings)
        {
          if (!s.StoreValueInternally)
          {
            runtimeSettings.tempSettingValues.AddOrUpdateDictionaryItem(s.Identifier, s.Setting.GetCurrentValues(runtimeSettings));
          }
        }
      }

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
        runtimeSettings.tempSettingValues = currentRuntimeSettings.tempSettingValues;
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
        if (Time.time > lastRefresh)
        {
          RefreshSettings();
          lastRefresh = Time.time;
        }
      }
    }
#endif
  }
}