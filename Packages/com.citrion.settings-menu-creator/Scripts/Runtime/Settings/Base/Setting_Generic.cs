using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Generic<T> : Setting_CustomOptions
  {
    [Tooltip("The default value for this setting.")]
    public T defaultValue;

    protected Setting_Generic()
    {
      options = new List<StringToStringRelation>();
      var optionsList = SettingsUtility.GetOptions<T>();
      if (optionsList != null)
      {
        foreach (var option in optionsList)
        {
          // Add the default options and make the visible name more readable
          options.Add(new StringToStringRelation(option, option.SplitCamelCase()));
        }
      }

      SetDefaultValue();
    }

    protected virtual void SetDefaultValue()
    {
      if (options != null && options.Count > 0)
      {
        if (SettingsUtility.ConvertValue<T>(options[0].Key, null, options, out var value))
        {
          defaultValue = value;
          return;
        }
      }

      defaultValue = default(T);
    }

    public override List<string> ParameterTypes => new List<string>() { typeof(T).AssemblyQualifiedName };

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      //TODO Finish implementation
      if (args?.Length > 0)
      {
        if (SettingsUtility.ConvertValue<T>(args[0], settings, options, out var value))
        {
          var newValue = ApplySettingChangeWithValue(settings, value);
          base.ApplySettingChange(settings, newValue);
          return newValue;
        }
      }

      base.ApplySettingChange(settings, null);
      return null;
    }

    protected virtual object ApplySettingChangeWithValue(SettingsCollection settings, T value)
    {
      return value;
    }

    public override object GetDefaultValue(SettingsCollection settings)
    {
      return defaultValue;
    }

    public override List<object> GetCurrentValues(SettingsCollection settings)
    {
      if (settings != null)
      {
        var settingHolder = settings.Settings?.Find(s => s.Setting == this);
        if (settingHolder != null && settings.activeSettingValues != null &&
            settings.activeSettingValues.TryGetValue(settingHolder.Identifier, out var value))
        {
          return new List<object> { value };
        }
      }
      return new List<object>() { defaultValue };
    }
  }
}
