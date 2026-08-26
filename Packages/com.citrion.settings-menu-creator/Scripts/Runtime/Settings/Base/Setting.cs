using CitrioN.Common;
using System.Collections.Generic;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [SkipObfuscationRename]
  public abstract class Setting
  {
    private bool initializedForRuntime = false;

    public virtual string EditorNamePrefix { get; } = null;

    public virtual string EditorName => GetType().Name.Replace("Setting_", "").Replace("_", "").SplitCamelCase();

    public virtual string RuntimeName => GetType().Name.Replace("Setting_", "").Replace("_", "").SplitCamelCase();

    public virtual List<string> ParameterTypes => null;

    public virtual List<StringToStringRelation> Options { get; set; } = new List<StringToStringRelation>();

    public virtual bool StoreValueInternally => true;

    /// <summary>
    /// Defines if the default value should be applied.
    /// Should be disabled for things like button specific actions.
    /// Otherwise their functionality will be invoked when the default
    /// value is applied.
    /// </summary>
    public virtual bool SkipApplyingDefault => false;

    public bool InitializedForRuntime { get => initializedForRuntime; private set => initializedForRuntime = value; }

    [SkipObfuscationRename]
    public virtual List<object> GetCurrentValues(SettingsCollection settings) => null;

    [SkipObfuscationRename]
    public abstract object GetDefaultValue(SettingsCollection settings);

    [SkipObfuscationRename]
    public virtual object ApplySettingChange(SettingsCollection settings, params object[] args) => null;

    public virtual void InitializeForRuntime(SettingsCollection settings)
    {
      InitializedForRuntime = true;
      GlobalEventHandler.RemoveEventListener("OnApplicationQuit", OnApplicationQuit);
      GlobalEventHandler.AddEventListener("OnApplicationQuit", OnApplicationQuit);
    }

    /// <summary>
    /// Method called when the application quits/stops.
    /// Main use is in the editor to restore values/variables etc.
    /// An example is to restore overrides that Unity does not automatically
    /// restore when existing play mode.
    /// </summary>
    protected virtual void OnApplicationQuit() { }
  }
}
