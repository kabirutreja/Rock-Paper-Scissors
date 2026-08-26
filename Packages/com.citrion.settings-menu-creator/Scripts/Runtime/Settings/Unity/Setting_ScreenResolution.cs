using CitrioN.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(700)]
  [MenuPath("Display")]
  [System.Serializable]
  // TODO Add support to rename the different options?
  // TODO Add support for input types other than string
  public class Setting_ScreenResolution : Setting
  {
    public override string EditorNamePrefix => "[Display]";

    public override List<string> ParameterTypes => new List<string>() { typeof(string).AssemblyQualifiedName };

    public override List<StringToStringRelation> Options
    {
      get
      {
        List<StringToStringRelation> options = new List<StringToStringRelation>();

        var resolutions = Screen.resolutions;
        List<Vector2> resVectors = new List<Vector2>();

        foreach (var res in resolutions)
        {
          resVectors.AddIfNotContains(new Vector2(res.width, res.height));
        }

        foreach (var res in resVectors)
        {
          var resString = $"{res.x} x {res.y}";
          options.Add(new StringToStringRelation(resString, resString));
        }

        return options;
      }
    }

    public override List<object> GetCurrentValues(SettingsCollection settings) =>
      new List<object>() { ScreenUtility.GetCurrentResolutionAsString() };

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      if (args?.Length > 0)
      {
        if (args[0] is string stringValue)
        {
          // Check if the string can be parsed to a resolution
          if (ScreenUtility.GetScreenResolutionFromString(stringValue, out var resolution))
          {
            ScreenUtility.SetScreenResolution(resolution.width, resolution.height, Screen.fullScreen);
            var newResolution = ScreenUtility.GetResolutionAsString(resolution);
            base.ApplySettingChange(settings, newResolution);
            return newResolution;
          }
        }
      }
      base.ApplySettingChange(settings, null);
      return null;
    }

    //public override void OnAnyUnitySettingChanged(string settingType, object newValue)
    //{
    //  base.OnAnyUnitySettingChanged(settingType, newValue);
    //  var monitorSettingType = nameof(MonitorSetting);
    //  if (settingType == monitorSettingType)
    //  {
    //    ConsoleLogger.Log("Changed Monitor");
    //  }
    //}

    public override object GetDefaultValue(SettingsCollection settings)
    {
      var resolutions = Screen.resolutions;
      if (resolutions != null && resolutions.Length > 0)
      {
        var resolution = resolutions.Last();
        return $"{resolution.width} x {resolution.height}";
      }
      return null;
    }
  }
}