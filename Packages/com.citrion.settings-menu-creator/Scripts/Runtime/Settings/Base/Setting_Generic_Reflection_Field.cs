using CitrioN.Common;
using System.Collections.Generic;
using System.Reflection;

namespace CitrioN.SettingsMenuCreator
{
  [ExcludeFromMenuSelection]
  [System.Serializable]
  // TODO Should there be a base class for field and property reflection?
  public abstract class Setting_Generic_Reflection_Field<T1, T2> : Setting_Generic<T1>
  {
    public abstract string FieldName { get; }

    protected virtual bool IsStatic { get; } = false;

    // Make sure to do a null check when using getter/setter
    public FieldInfo FieldInfo
    {
      get
      {
        var field = typeof(T2).GetField(FieldName);
        if (field == null)
        {
          field = typeof(T2).GetPrivateField(FieldName);
        }
        if (field == null)
        {
          ConsoleLogger.LogWarning($"Unable to find field with name " +
                                   $"{FieldName} for {GetType().Name}");
        }
        return field;
      }
    }

    public abstract object GetObject(SettingsCollection settings);

    public override List<object> GetCurrentValues(SettingsCollection settings)
    {
      var field = FieldInfo;
      if (field == null) { return null; }

      var obj = GetObject(settings);
      if (obj == null && !IsStatic) { return null; }

      object fieldValue = null;
      try
      {
        fieldValue = field.GetValue(obj);
      }
      catch (System.Exception) { /*throw;*/ }
      return new List<object>() { fieldValue };
    }

    protected override object ApplySettingChangeWithValue(SettingsCollection settings, T1 value)
    {
      var field = FieldInfo;
      if (field == null) { return null; }

      var obj = GetObject(settings);
      if (obj == null && !IsStatic) { return null; }

      field.SetValue(obj, value);
      return field.GetValue(obj);
    }
  }
}

