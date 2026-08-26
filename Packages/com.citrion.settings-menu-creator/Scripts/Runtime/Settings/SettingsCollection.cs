using CitrioN.Common;
using CitrioN.StyleProfileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [CreateAssetMenu(fileName = "SettingsCollection_",
                   menuName = "CitrioN/Settings Menu Creator/Settings Collection/Default",
                   order = 2)]
  [SkipObfuscationRename]
  [AddTooltips]
  [HeaderInfo("The 'SettingsCollection' is where you add new settings to your menu. " +
              "It also contains references to many other systems responsible for the creation of your menu. " +
              "These include provider collections for both of Unity's UI systems (UGUI & UI Toolkit), a 'SettingsSaver', " +
              "AudioMixer and more depending on your version and installed addons.")]
  public partial class SettingsCollection : ScriptableObject
  {
    protected static Dictionary<SettingsCollection, bool> initializedCollections =
      new Dictionary<SettingsCollection, bool>();

    /// <summary>
    /// The collections for which the default values have been loaded before
    /// </summary>
    public static Dictionary<SettingsCollection, bool> defaultsLoadedCollections =
      new Dictionary<SettingsCollection, bool>();

    [SerializeField]
    [Tooltip("The identifier for the collection. " +
             "It can be used for a 'OnSettingValueChangeListener' to only react to collections with that identifier." +
             "Most people will only ever use one SettingsCollection in which case the identifier can simply be left empty.")]
    protected string identifier;

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("This is your list of settings for your menu.\n\n" +
             "Add setting: Click the '+' button and navigate or search for your setting to add.\n\n" +
             "Remove setting(s): Click the '-' button remove the currently selected setting(s) or the last setting from the list.\n\n" +
             "Multi Selection: You can select multiple settings by holding 'Ctrl/Command' while selecting a setting. " +
             "To select all setting between two settings you can hold the 'Shift' key.\n\n" +
             "Utility Functions: You can access utility functions such as duplicating a selected setting " +
             "via the context menu (3 vertical dots) at the top.")]
    protected List<SettingHolder> settings = new List<SettingHolder>();

    [SerializeField]
    [Tooltip("A reference to the SettingsSaver which handles " +
             "saving & loading for this SettingsCollection.")]
    protected SettingsSaver settingsSaver;

    [SerializeField]
    [Tooltip("The collection of input element providers used for UI Toolkit")]
    protected InputElementProviderCollection_UIT inputTemplatesUIToolkit;

    [SerializeField]
    [Tooltip("The collection of input element providers used for UGUI")]
    protected InputElementProviderCollection_UGUI inputTemplatesUGUI;

    [SerializeField]
    [Tooltip("The reference to an AudioMixer. This AudioMixer will be used for " +
             "AudioMixer specific settings if the setting itself has no override mixer referenced.")]
    protected AudioMixer audioMixer;

    [SerializeField]
    [Tooltip("The LocalizationProvider to use for localization of LocalizationBehaviour scripts " +
             "if no provider is already assigned.")]
    protected LocalizationProvider localizationProvider;

#if UNITY_POST_PROCESSING
    [SerializeField]
    [Tooltip("The reference to a post processing profile. " +
             "Used for post processing profile related settings " +
             "when no setting specific profile override is set.")]
    protected UnityEngine.Rendering.PostProcessing.PostProcessProfile postProcessProfile;
#endif

#if UNITY_HDRP || UNITY_URP
    [SerializeField]
    [Tooltip("The reference to a VolumeProfile. " +
             "Used for some HDRP specific settings when no setting specific profile override is set.")]
    protected UnityEngine.Rendering.VolumeProfile volumeProfile;
#endif

    [SerializeField]
    [Tooltip("The prefab to use for the menu layout. If specified this prefab will be assigned to the SettingsMenu_UGUI. " +
             "If not the one referenced on the SettingsMenu_UGUI will be used instead.")]
    protected GameObject uguiLayoutPrefab;

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("The StyleProfile to assign to the ApplyStyleProfile script if it is attached. " +
             "This StyleProfile is not necessarily the one used at runtime!")]
    protected StyleProfile styleProfile;

    [SerializeField]
    [Tooltip("The UGUI ProviderCollection to use for the UI element creation in the prefab. " +
             "If left empty the default ProviderCollection will be used. " +
             "Only use this if you want to mix multiple ProviderCollections in your menu." +
             "Not used for the runtime UI element creation!")]
    protected InputElementProviderCollection_UGUI providerCollectionOverride;

    [SerializeField]
    [Tooltip("Whether the 'Register StyleListeners In Hierarchy (Edit Mode Only)' script should be attached to the prefab. " +
         "This will enable all StyleListeners on the prefab to react to StyleProfile changes and allows proper visualization " +
         "of the menu's visuals.")]
    protected bool addEditModeStyleProfileListening = true;

    [SerializeField]
    [Tooltip("Whether the 'Apply Style Profile' script should be attached to the prefab. " +
             "This will automatically apply the referenced StyleProfile to prompt all " +
             "StyleListeners that are enabled during edit mode to apply their modifications.")]
    protected bool addApplyStyleProfileScript = true;
#endif

    public Dictionary<string, object[]> pendingSettingChanges = new Dictionary<string, object[]>();

    public Dictionary<string, object> activeSettingValues = new Dictionary<string, object>();

    public Dictionary<string, object> tempSettingValues = new Dictionary<string, object>();

    public Dictionary<Setting, object> startValues = new Dictionary<Setting, object>();

    public Action<string> onSettingUpdated;

#if UNITY_EDITOR
    public const string SETTINGS_COLLECTION_CHANGED_EVENT_NAME = "SettingsCollection Changed";
#endif

    public string Identifier
    {
      get => identifier;
      set => identifier = value;
    }

    public SettingsSaver SettingsSaver
    {
      get => settingsSaver;
      set => settingsSaver = value;
    }

    public List<SettingHolder> Settings
    {
      get => settings;
      set => settings = value;
    }

    public InputElementProviderCollection_UIT InputElementProviders_UIT
    {
      get => inputTemplatesUIToolkit;
      set => inputTemplatesUIToolkit = value;
    }

    public InputElementProviderCollection_UGUI InputElementProviders_UGUI
    {
      get => inputTemplatesUGUI;
      set => inputTemplatesUGUI = value;
    }

    public LocalizationProvider LocalizationProvider
    {
      get => localizationProvider;
      set => localizationProvider = value;
    }

#if UNITY_POST_PROCESSING
    public UnityEngine.Rendering.PostProcessing.PostProcessProfile PostProcessProfile
    {
      get => postProcessProfile;
      set => postProcessProfile = value;
    }
#endif

#if UNITY_HDRP || UNITY_URP
    public UnityEngine.Rendering.VolumeProfile VolumeProfile
    {
      get => volumeProfile;
      set => volumeProfile = value;
    }
#endif

    public AudioMixer AudioMixer
    {
      get => audioMixer;
      set => audioMixer = value;
    }

    public GameObject UguiLayoutPrefab
    {
      get => uguiLayoutPrefab;
      set => uguiLayoutPrefab = value;
    }

#if UNITY_EDITOR
    public StyleProfile StyleProfile
    {
      get => styleProfile;
      set => styleProfile = value;
    }

    public bool AddEditModeStyleProfileListening
    {
      get => addEditModeStyleProfileListening;
      set => addEditModeStyleProfileListening = value;
    }

    public bool AddApplyStyleProfileScript
    {
      get => addApplyStyleProfileScript;
      set => addApplyStyleProfileScript = value;
    }

    public InputElementProviderCollection_UGUI ProviderCollectionOverride
    {
      get => providerCollectionOverride;
      set => providerCollectionOverride = value;
    }
#endif
    public bool HasDefaultsLoadedOnce
    {
      get
      {
        if (defaultsLoadedCollections != null &&
            defaultsLoadedCollections.TryGetValue(this, out var defaultsLoaded))
        {
          return defaultsLoaded;
        }
        return false;
      }
    }

    protected bool debugMode = false;

    private void Reset()
    {
      ClearList();
    }

    [ContextMenu("Clear All")]
    public void ClearList()
    {
      Settings.Clear();
#if UNITY_EDITOR
      GlobalEventHandler.InvokeEvent(SETTINGS_COLLECTION_CHANGED_EVENT_NAME, this);
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    protected static void StaticInit()
    {
      initializedCollections?.Clear();
      defaultsLoadedCollections?.Clear();
    }

    [SkipObfuscationRename]
    public void Initialize()
    {
      if (initializedCollections != null &&
          initializedCollections.TryGetValue(this, out var isInitialized))
      {
        if (isInitialized) { return; }
      }

      pendingSettingChanges.Clear();
      activeSettingValues.Clear();
      if (InputElementProviders_UGUI != null)
      {
        InputElementProviders_UGUI.RefreshDictionary();
      }
      if (InputElementProviders_UIT != null)
      {
        InputElementProviders_UIT.RefreshDictionary();
      }

      // Cache the currently active values so they can later be used to reset to them
      startValues.Clear();
      onSettingUpdated = default;
      foreach (var holder in settings)
      {
        if (holder == null) { continue; }
        var setting = holder.Setting;
        if (setting == null) { continue; }
        setting.InitializeForRuntime(this);
        //var defaultValue = setting.GetDefaultValue(this);
        //setting.ApplySettingChange(this, defaultValue);
        var startValueList = setting.GetCurrentValues(this);
        var startValue = startValueList?.Count > 0 ? startValueList[0] : null;
        startValues.AddOrUpdateDictionaryItem(setting, startValue);
      }

      if (debugMode)
      {
        ConsoleLogger.Log("Initialized settings collection");
      }

      initializedCollections.AddOrUpdateDictionaryItem(this, true);
    }

    [SkipObfuscationRename]
    public SettingHolder GetSettingHolder(string settingIdentifier)
    {
      return Settings?.Find(s => s.Identifier == settingIdentifier);
    }

    [SkipObfuscationRename]
    public void UpdateSettingField(VisualElement root, string identifier)
    {
      var s = Settings.Find(s => s.Identifier == identifier);

      if (s != null)
      {
        var inputElement = s.FindElement_UIToolkit(root, this);
        s.InitializeElement_UIToolkit(inputElement, this, initialize: false);
        //ConsoleLogger.Log($"Setting {identifier} is of type '{typeString}' with value '{value}'");
      }
    }

    [SkipObfuscationRename]
    public void UpdateSettingField(RectTransform root, string identifier)
    {
      var s = Settings.Find(s => s.Identifier == identifier);

      if (s != null)
      {
        var inputElement = s.FindElement_UGUI(root, this);
        if (inputElement != null)
        {
          s.InitializeElement_UGUI(inputElement, this, initialize: !SettingHolder.InitializedInputElements.ContainsKey(inputElement));
        }
        //ConsoleLogger.Log($"Setting {identifier} is of type '{typeString}' with value '{value}'");
      }
    }

    [SkipObfuscationRename]
    public void ApplySettingChange(string settingIdentifier, bool forceApply, bool updateInputElement, params object[] args)
    {
      var settings = Settings.FindAll(s => s.Identifier == settingIdentifier);

      for (int i = 0; i < settings.Count; i++)
      {
        var setting = settings[i];

        if (setting == null) { return; }
        if (/*setting != null && */(forceApply /*|| applyImmediatelyMode == ApplyImmediatelyMode.Always*/ ||
           (/*applyImmediatelyMode == ApplyImmediatelyMode.PerSetting &&*/ setting.ApplyImmediately == true))
          /*(forceApply || setting.ApplyImmediately == true)*/)
        {
          var newValue = setting.ApplySettingChange(this, args);

          if (Application.isPlaying)
          {
            if (setting.StoreValueInternally)
            {
              activeSettingValues.AddOrUpdateDictionaryItem(settingIdentifier, newValue);
            }

            if (updateInputElement)
            {
              onSettingUpdated?.Invoke(settingIdentifier);
            }
          }
        }
        else
        {
          pendingSettingChanges.AddOrUpdateDictionaryItem(settingIdentifier, args);
        }
      }
    }

    public void RevertPendingSettingsChanges()
    {
      var keys = pendingSettingChanges.Keys.ToList();
      for (int i = 0; i < keys.Count; i++)
      {
        var key = keys[i];
        if (string.IsNullOrEmpty(key)) { continue; }
        onSettingUpdated?.Invoke(key);
      }

      pendingSettingChanges.Clear();
    }

    [SkipObfuscationRename]
    //[ContextMenu("Apply Settings Changes")]
    public void ApplyPendingSettingsChanges()
    {
      // TODO Bug?
      //for (int i = 0; i < pendingSettingChanges.Count; i++)
      foreach (var item in pendingSettingChanges)
      {
        var identifier = item.Key;
        var args = item.Value;

        ApplySettingChange(identifier, forceApply: true, true, args);

        if (debugMode)
        {
          string parameters = string.Empty;
          if (args?.Length > 0)
          {
            var sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
              sb.AppendLine(args[i].ToString());
            }
            parameters = sb.ToString();
          }

          ConsoleLogger.Log($"Applying settings change: {identifier} - {parameters}", Common.LogType.Debug);
        }
      }

      pendingSettingChanges.Clear();
    }

    public void ApplySettingValues(Dictionary<string, object> data, bool forceApply)
    {
      if (data == null || data.Count == 0) { return; }
      foreach (var item in data)
      {
        ApplySettingChange(item.Key, forceApply, true, item.Value);
      }
    }

    [SkipObfuscationRename]
    //[ContextMenu("Print Setting Values")]
    public void PrintActiveSettingValues()
    {
      foreach (var item in activeSettingValues)
      {
        var identifier = item.Key;
        var value = item.Value;
        string typeString = value != null ? value.GetType().Name : "NULL";

        ConsoleLogger.Log($"{identifier}: Type: {typeString} - Value: {value}", Common.LogType.Debug);
      }
    }

    public void SaveSettings()
    {
      if (SettingsSaver != null)
      {
        SettingsSaver.SaveSettings(this);
      }
    }

    [ContextMenu("Reset Settings To Default")]
    public void ResetToDefaultSettings()
    {
      activeSettingValues.Clear();
      LoadSettings(true);
      SaveSettings();
    }

    public void LoadSettings(bool isDefault)
    {
      LoadSettings(isDefault, true, true);
    }

    public void LoadSettings(bool isDefault, bool apply, bool forceApply)
    {
      if (SettingsSaver != null)
      {
        if (isDefault)
        {
          if (apply)
          {
            foreach (var h in settings)
            {
              if (h == null || h.Setting == null) { continue; }
              var defaultValue = h.Setting.GetDefaultValue(this);
              if (!h.Setting.SkipApplyingDefault && defaultValue != null)
              {
                ApplySettingChange(h.Identifier, true, true, defaultValue);
              }
              //else
              //{
              //  ConsoleLogger.Log($"Not applying default value for {h.Setting.RuntimeName}");
              //}
            }

            defaultsLoadedCollections.AddOrUpdateDictionaryItem(this, true);
          }
        }
        else
        {
          var data = SettingsSaver.LoadSettings();
          if (apply)
          {
            ApplySettingValues(data, forceApply);
          }
        }
      }
    }

    public void RestoreStartValues()
    {
      foreach (var item in startValues)
      {
        var setting = item.Key;
        var value = item.Value;

        setting?.ApplySettingChange(this, value);
      }
    }

    [ContextMenu("Delete Save")]
    public void DeleteSave()
    {
      if (SettingsSaver != null)
      {
        SettingsSaver.DeleteSave();
      }
    }

    public bool GetCurrentSettingValue<T>(string settingIdentifier, out T value)
    {
      if (string.IsNullOrEmpty(settingIdentifier))
      {
        value = default(T);
        return false;
      }

      if (activeSettingValues.TryGetValue(settingIdentifier, out var val1))
      {
        if (val1 is T value1)
        {
          value = value1;
          return true;
        }
        else
        {
          ConsoleLogger.LogWarning($"The requested setting value is of type {val1.GetType()} " +
                                   $"and does not match the requested type of {typeof(T)}.");
          value = default(T);
          return false;
        }
      }

      var settingHolder = Settings.Find(h => h.Identifier == settingIdentifier);
      if (settingHolder != null && settingHolder.Setting != null)
      {
        var values = settingHolder.Setting.GetCurrentValues(this);
        if (values != null && values.Count > 0)
        {
          if (values[0] is T value1)
          {
            value = value1;
            return true;
          }
          else
          {
            ConsoleLogger.LogWarning($"The requested setting value is of type {values[0].GetType()} " +
                                     $"and does not match the requested type of {typeof(T)}.");
            value = default(T);
            return false;
          }
        }
      }

      value = default(T);
      return false;
    }

    public bool GetCurrentSettingValues(string settingIdentifier, out object value)
    {
      if (string.IsNullOrEmpty(settingIdentifier))
      {
        value = null;
        return false;
      }

      if (activeSettingValues.TryGetValue(settingIdentifier, out value))
      {
        return true;
      }

      var settingHolder = Settings.Find(h => h.Identifier == settingIdentifier);
      if (settingHolder != null && settingHolder.Setting != null)
      {
        value = settingHolder.Setting.GetCurrentValues(this);
        return true;
      }

      value = null;
      return false;
    }
  }
}