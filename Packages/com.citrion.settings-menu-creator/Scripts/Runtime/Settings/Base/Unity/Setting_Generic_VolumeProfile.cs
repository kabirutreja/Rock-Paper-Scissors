using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_URP || UNITY_HDRP
using UnityEngine.Rendering;
#endif

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class Setting_VolumeProfile : Setting_CustomOptions
  {
    public override bool StoreValueInternally => false;

#if UNITY_URP || UNITY_HDRP
    [Tooltip("Reference to the VolumeProfile to manage.\n\n" +
             "If left blank the VolumeProfile specified in\n" +
             "the SettingsCollection will be used.")]
    public VolumeProfile volumeProfileOverride;

    public VolumeProfile GetVolumeProfile(SettingsCollection settings)
    {
      if (volumeProfileOverride != null) { return volumeProfileOverride; }
      if (settings != null)
      {
        var profile = settings.VolumeProfile;
        if (profile != null) { return profile; }
      }
      ConsoleLogger.LogWarning($"Unable to manage '{GetType().Name}' because " +
                               $"no {nameof(VolumeProfile)} is specified. You " +
                               $"can specify one on the {nameof(SettingsCollection)} " +
                               $"or using this settings {nameof(VolumeProfile)} override.");
      return null;
    }
#endif
  }

  [System.Serializable]
  public abstract class Setting_Generic_VolumeProfile<T> : Setting_Generic<T>
  {
    public override bool StoreValueInternally => false;

#if UNITY_URP || UNITY_HDRP
    [Tooltip("Reference to the VolumeProfile to manage.\n\n" +
             "If left blank the VolumeProfile specified in\n" +
             "the SettingsCollection will be used.")]
    public VolumeProfile volumeProfileOverride;

    public VolumeProfile GetVolumeProfile(SettingsCollection settings)
    {
      if (volumeProfileOverride != null) { return volumeProfileOverride; }
      if (settings != null)
      {
        var profile = settings.VolumeProfile;
        if (profile != null) { return profile; }
      }
      ConsoleLogger.LogWarning($"Unable to manage '{GetType().Name}' because " +
                               $"no {nameof(VolumeProfile)} is specified. You " +
                               $"can specify one on the {nameof(SettingsCollection)} " +
                               $"or using this settings {nameof(VolumeProfile)} override.");
      return null;
    }
#endif
  }

  [ExcludeFromMenuSelection]
  [System.Serializable]
  public abstract class Setting_Generic_VolumeProfile_Extended<T1, T2> : Setting_Generic_VolumeProfile<T1>
#if UNITY_HDRP || UNITY_URP
    where T2 : VolumeComponent
#else
    where T2 : ScriptableObject
#endif
  {
    public abstract string FieldName { get; }

#if UNITY_URP || UNITY_HDRP
    protected override object ApplySettingChangeWithValue(SettingsCollection settings, T1 value)
    {
      var volumeProfile = GetVolumeProfile(settings);
      if (volumeProfile != null)
      {
        if (volumeProfile.TryGet<T2>(out var component))
        {
          var type = component.GetType();
          var field = type.GetField(FieldName);
          var current = field != null ? field.GetValue(component) :
                        type.GetProperty(FieldName)?.GetValue(component);

          if (current != null && current is VolumeParameter)
          {
            var valueProperty = current.GetType().GetProperty("value");
            var overrideStateProperty = current.GetType().GetProperty("overrideState");

            if (valueProperty != null)
            {
              valueProperty.SetValue(current, value);
              overrideStateProperty?.SetValue(current, true);
              var newValue = valueProperty.GetValue(current);
              return newValue;
            }
          }
          else if (field != null)
          {
            field.SetValue(component, value);
            return field.GetValue(component);
          }

          return current;
        }
      }
      return null;
    }

    public override List<object> GetCurrentValues(SettingsCollection settings)
    {
      var volumeProfile = GetVolumeProfile(settings);
      if (volumeProfile != null)
      {
        if (volumeProfile.TryGet<T2>(out var component))
        {
          var type = component.GetType();
          var field = type.GetField(FieldName);
          var current = field != null ? field.GetValue(component) :
                        type.GetProperty(FieldName)?.GetValue(component);

          if (current != null && current is VolumeParameter)
          {
            var value = current.GetType().GetProperty("value")?.GetValue(current);
            if (value != null)
            {
              return new List<object>() { value };
            }
          }
          else if (current != null)
          {
            return new List<object> { current };
          }
          return null;
        }
      }
      return null;
    }
#endif
  }
}

