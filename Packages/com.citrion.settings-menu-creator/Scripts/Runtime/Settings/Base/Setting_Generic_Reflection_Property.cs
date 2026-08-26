using CitrioN.Common;
using System.Collections.Generic;
using System.Reflection;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Generic_Reflection_Property<T1, T2> : Setting_Generic<T1>
  {
    public abstract string PropertyName { get; }

    protected virtual bool IsStatic { get; } = false;

    public PropertyInfo PropertyInfo
    {
      get
      {
        var property = typeof(T2).GetProperty(PropertyName);
        if (property == null)
        {
          ConsoleLogger.LogWarning($"Unable to find property with name " +
                                   $"{PropertyName} for {GetType().Name}",
                                   LogType.Debug);
        }
        return property;
      }
    }

    public abstract object GetObject(SettingsCollection settings);

    public override List<object> GetCurrentValues(SettingsCollection settings)
    {
      var property = PropertyInfo;
      if (property == null) { return null; }

      var obj = GetObject(settings);
      if (obj == null && !IsStatic) { return null; }

      object propertyValue = null;
      try
      {
        propertyValue = property.GetValue(obj);
      }
      catch (System.Exception) { /*throw;*/ }
      return new List<object>() { propertyValue };
    }

    protected override object ApplySettingChangeWithValue(SettingsCollection settings, T1 value)
    {
      var property = PropertyInfo;
      if (property == null) { return null; }

      var obj = GetObject(settings);
      if (obj == null && !IsStatic) { return null; }

      try
      {
        property.SetValue(obj, value);
      }
      catch (System.Exception) { }
      return property.GetValue(obj);
    }
  }
}