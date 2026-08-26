using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [SkipObfuscationRename]
  public class SettingHolder : ISettingHolder
  {
    #region Fields & Properties
    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("Should any changes to this setting\n" +
             "be applied immediately?\n\n" +
             "If disabled any change needs to be\n" +
             "applied manually for example by using\n" +
             "the 'Apply Changes' setting.")]
    protected bool applyImmediately = true;

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("An identifier used to create the connection\n" +
             "between this setting and an input element.")]
    protected string identifier;

#if UNITY_EDITOR
    [SerializeField]
    protected bool expanded = false;
#endif

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("Should a new identifier be generated for\n" +
             "any duplicates of this setting?\n\n" +
             "Useful if you have presets with custom identifiers\n" +
             "that should be copied and not overriden.\n" +
             "This is the case for provided preset collections.\n\n" +
             "Default: true")]
    protected bool overrideIdentifierWhenCopied = true;

    [SerializeField]
    [SerializeReference]
    [SkipObfuscationRename]
    protected Setting setting;

    [SerializeField]
    [SerializeReference]
    [SkipObfuscationRename]
    protected InputElementProviderSettings inputElementProviderSettings;

    public static Dictionary<RectTransform, bool> InitializedInputElements = new Dictionary<RectTransform, bool>();

#if UNITY_EDITOR
    public string MenuName
    {
      get
      {
        if (Setting != null)
        {
          var prefix = Setting.EditorNamePrefix;
          var settingName = Setting.EditorName;
          var label = InputElementProviderSettings?.CustomLabel;

          var hasPrefix = !string.IsNullOrEmpty(prefix);
          var hasSettingName = !string.IsNullOrEmpty(settingName);
          var hasLabel = !string.IsNullOrEmpty(label);

          return $"{prefix}{(hasPrefix ? " " : "")}{settingName}" +
                 $"{(hasSettingName && hasLabel ? "  |  " : "")}{(hasLabel ? "Label: " : "")}{label}";
        }
        else
        {
          return "Missing Menu Name";
        }
      }
    }
#endif

#if UNITY_EDITOR
    public bool Expanded { get => expanded; set => expanded = value; }
    public int CurrentTabMenuIndex { get; set; } = 0;
#endif

    public bool ApplyImmediately { get => applyImmediately; set => applyImmediately = value; }

    public string Identifier { get => identifier; set => identifier = value; }

    public Setting Setting { get => setting; set => setting = value; }

    public InputElementProviderSettings InputElementProviderSettings
    {
      get => inputElementProviderSettings;
      set => inputElementProviderSettings = value;
    }

    public string InputElementLabelText => string.IsNullOrEmpty(InputElementProviderSettings?.CustomLabel) ?
                                           Setting?.RuntimeName : InputElementProviderSettings.CustomLabel;

    public List<string> ParameterTypes => Setting?.ParameterTypes;

    public bool StoreValueInternally => Setting != null ? Setting.StoreValueInternally : false;

    public List<StringToStringRelation> Options
      => Setting != null ? Setting.Options : new List<StringToStringRelation>();

    public List<string> DisplayOptions
    {
      get
      {
        var options = Options;
        if (options == null || options.Count == 0) { return null; }
        var optionsList = new List<string>();
        foreach (var option in options)
        {
          optionsList.Add(option.Value);
        }
        return optionsList;
      }
    }

    public bool OverrideIdentifierWhenCopied
    {
      get => overrideIdentifierWhenCopied;
      set => overrideIdentifierWhenCopied = value;
    }
    #endregion

    public SettingHolder()
    {
      CreateNewIdentifier();
      Setting = new Setting_Empty();
      InputElementProviderSettings = new InputElementProviderSettings();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      InitializedInputElements = new Dictionary<RectTransform, bool>();
    }

    public static SettingHolder GetCopy(SettingHolder original)
    {
      if (original == null) { return null; }
      var serializedHolder = JsonUtility.ToJson(original);
      var duplicate = JsonUtility.FromJson(serializedHolder, typeof(SettingHolder)) as SettingHolder;
      if (original.OverrideIdentifierWhenCopied)
      {
        duplicate.CreateNewIdentifier();
      }
#if UNITY_EDITOR
      // Reset the expanded value
      duplicate.expanded = false;
#endif
      return duplicate;
    }

    [SkipObfuscationRename]
    public void CreateNewIdentifier()
    {
      Identifier = Guid.NewGuid().ToString();
    }

    [SkipObfuscationRename]
    public void MatchIdentifierToSettingName()
    {
      var settingName = Setting?.RuntimeName;

      if (!string.IsNullOrEmpty(settingName))
      {
        Identifier = settingName;
      }
    }

    [SkipObfuscationRename]
    public VisualElement CreateElement_UIToolkit(VisualElement root, SettingsCollection settings)
    {
      if (root == null) { return null; }
      VisualElement elem = null;
      elem = InputElementProviderSettings?.InputElementProvider_UIToolkit?.GetProvider(settings)?.GetInputElement(Identifier, settings);

      if (elem == null)
      {
        elem = ProviderUtility_UIT.CreateInputFieldForParameters_UIToolkit(Identifier, settings, InputElementLabelText, ParameterTypes);
      }

      if (elem != null)
      {
        string parentIdentifier = InputElementProviderSettings.ParentIdentifier;
        elem.AddToClassList(Identifier);
        bool attachToParent = !string.IsNullOrEmpty(parentIdentifier);
        VisualElement spacer = null;

        if (InputElementProviderSettings.AddSpacer)
        {
          spacer = new VisualElement();
          if (!string.IsNullOrEmpty(inputElementProviderSettings.SpacerElementClass))
          {
            spacer.AddToClassList(inputElementProviderSettings.SpacerElementClass);
          }
        }

        if (attachToParent)
        {
          var parentElement = root.Q(className: parentIdentifier);
          if (parentElement == null) { parentElement = root; }
          parentElement.Add(elem);
          if (spacer != null) { parentElement.Add(spacer); }
        }
      }
      return elem;
    }

    [SkipObfuscationRename]
    public VisualElement FindElement_UIToolkit(VisualElement root, SettingsCollection settings)
    {
      if (root == null) { return null; }
      if (InputElementProviderSettings?.InputElementProvider_UIToolkit?.GetProvider(settings) != null)
      {
        return InputElementProviderSettings.InputElementProvider_UIToolkit.GetProvider(settings).FindInputElement(root, Identifier, settings);
      }
      else
      {
        return ProviderUtility_UIT.FindInputFieldForParameters_UIToolkit(root, settings, Identifier, InputElementLabelText, ParameterTypes);
      }
    }

    [SkipObfuscationRename]
    public void InitializeElement_UIToolkit(VisualElement elem, SettingsCollection settings, bool initialize)
    {
      if (elem == null) { return; }
      var values = new List<object> { };
      if (settings.activeSettingValues.TryGetValue(identifier, out var val))
      {
        values.Add(val);
      }
      else
      {
        values = Setting.GetCurrentValues(settings);
      }

      var provider = InputElementProviderSettings?.InputElementProvider_UIToolkit?.GetProvider(settings);
      // TODO Refactor slighly for better readability
      if (provider != null && provider.UpdateInputElement
         (elem, Identifier, InputElementLabelText, settings, values, initialize))
      {
        return;
      }

      if (ProviderUtility_UIT.UpdateInputFieldForParameters_UIToolkit
         (elem, settings, Identifier, InputElementLabelText, ParameterTypes, values, initialize))
      {
        return;
      }

      ProviderUtility_UIT.UpdateInputFieldFallback_UIToolkit
        (elem, settings, Identifier, InputElementLabelText, ParameterTypes, values, initialize);
    }

    [SkipObfuscationRename]
    public object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      var newValue = Setting?.ApplySettingChange(settings, args);
      // We need to invoke the event a frame delayed to make sure all listeners
      // have registered their callbacks. This is only because Unity's script execution order
      // has a bug that seems to ignore the order in some cases.
      ScheduleUtility.InvokeDelayedByFrames(() => GlobalEventHandler.InvokeEvent(SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, this.Setting, Identifier, settings, newValue));
      //ConsoleLogger.Log($"Changed settings value of {Identifier} in {settings.name} to {newValue}");
      return newValue;
    }

    [SkipObfuscationRename]
    public RectTransform CreateElement_UGUI(RectTransform root, SettingsCollection settings)
    {
      if (root == null) { return null; }
      RectTransform elem = null;
      elem = InputElementProviderSettings?.InputElementProvider_UGUI?.GetProvider(settings)?.GetInputElement(Identifier, settings);

      if (elem == null)
      {
        elem = ProviderUtility_UGUI.CreateInputFieldForParameters_UGUI(Identifier, settings, InputElementLabelText, ParameterTypes);
      }

      if (elem != null)
      {
        // Assign the identifier for the setting
        var settingIdentifier = elem.gameObject.AddOrGetComponent<SettingObject>();
        settingIdentifier.Identifier = Identifier;

        string parentIdentifier = InputElementProviderSettings.ParentIdentifier;
        bool attachToParent = !string.IsNullOrEmpty(parentIdentifier);

        if (attachToParent)
        {
          var settingIdentifiers = root.GetComponentsInChildren<SettingObject>(true, true);
          var parentElement = settingIdentifiers?.Find(s => s.Identifier == parentIdentifier);
          //?.GetComponent<RectTransform>();

          if (parentElement == null)
          {
            ConsoleLogger.LogWarning($"Unable to find setting parent with name '{parentIdentifier}' for " +
                                     $"'{Identifier}'. Attaching the input element to the root. " +
                                     $"If this is not desired make sure that a '{nameof(SettingObject)}' " +
                                     $"component is present in your hierarchy with the identifier being " +
                                     $"'{parentIdentifier}'.");
            GameObject.DestroyImmediate(elem.gameObject);
            return null;
          }

          if (parentElement != null)
          {
            Transform parentTransform = parentElement.GetContentParent();
            elem.SetParent(parentTransform != null ? parentTransform : root, false);
          }
        }
      }
      return elem;
    }

    [SkipObfuscationRename]
    public RectTransform FindElement_UGUI(RectTransform root, SettingsCollection settings)
    {
      if (root == null) { return null; }
      RectTransform element = null;
      var provider = InputElementProviderSettings?.InputElementProvider_UGUI?.GetProvider(settings);
      if (provider != null)
      {
        element = provider.FindInputElement(root, Identifier, settings);
      }

      if (element == null)
      {
        element = ProviderUtility_UGUI.FindInputFieldForParameters_UGUI(root, settings, Identifier, InputElementLabelText, ParameterTypes);
      }

      return element;
    }

    [SkipObfuscationRename]
    public void InitializeElement_UGUI(RectTransform elem, SettingsCollection settings, bool initialize)
    {
      if (elem == null) { return; }

      bool isInitialized = InitializedInputElements.ContainsKey(elem);
      if (initialize && isInitialized)
      {
        //ConsoleLogger.LogWarning("An input element should only be initialized once!");
        // TODO Force to not initialize?
      }

      var values = new List<object> { };
      if (settings.activeSettingValues.TryGetValue(identifier, out var val))
      {
        values.Add(val);
      }
      else
      {
        values = Setting.GetCurrentValues(settings);
      }

      var provider = InputElementProviderSettings?.InputElementProvider_UGUI?.GetProvider(settings);
      if (provider != null && provider.UpdateInputElement
         (elem, Identifier, InputElementLabelText, settings, values, initialize))
      {
        if (initialize) { MarkElementInitialized(elem); }
        return;
      }
      if (ProviderUtility_UGUI.UpdateInputFieldForParameters_UGUI
         (elem, settings, Identifier, InputElementLabelText, ParameterTypes, values, initialize))
      {
        if (initialize) { MarkElementInitialized(elem); }
        return;
      }

      if (ProviderUtility_UGUI.UpdateInputFieldFallback_UGUI
         (elem, settings, Identifier, InputElementLabelText, ParameterTypes, values, initialize))
      {
        if (initialize) { MarkElementInitialized(elem); }
      }
    }

    public static void MarkElementInitialized(RectTransform elem)
    {
      InitializedInputElements.AddOrUpdateDictionaryItem(elem, true);
    }

    [SkipObfuscationRename]
    public StringToStringRelation GetOptionForKey(object key)
    {
      if (key == null) { return null; }
      var options = Options;
      var keyString = key.ToString();
      // TODO Make this more efficient cause it doesn't scale well with large
      // collections or are they never large enough for this to matter?
      var relation = options.Find(o => o.Key == keyString);
      return relation;
    }

    [SkipObfuscationRename]
    public string GetOptionValueForKey(object key)
    {
      if (key == null) { return null; }
      var relation = GetOptionForKey(key);

      if (relation == null)
      {
        // We return the key because no relation was found.
        // This can be the case if the initialization of the
        // input element modified the available options/values.
        // The input element itself must have a check in case
        // the returned value which is in this case the key
        // will not break anything.
        return key.ToString();
      }

      return relation.Value;
    }

    //[SkipObfuscationRename]
    //public int GetOptionIndexForKey(object key)
    //{
    //  var relation = GetOptionForKey(key);
    //  if (relation != null)
    //  {
    //    return Options.IndexOf(relation);
    //  }
    //  return -1;
    //}
  }
}
