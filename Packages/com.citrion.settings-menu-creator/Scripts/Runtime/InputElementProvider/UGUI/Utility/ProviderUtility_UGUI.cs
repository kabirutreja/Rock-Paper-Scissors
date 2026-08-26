using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UGUI
  {
    public static ScriptableInputElementProvider_UGUI GetSettingInputElementProvider_UGUI(
      SettingsCollection settings, List<string> parameterTypesNames)
    {
      if (settings == null || settings.InputElementProviders_UGUI == null) { return null; }

      ScriptableInputElementProvider_UGUI provider = null;
      string providerKey = null;

      if (parameterTypesNames == null || parameterTypesNames.Count == 0)
      {
        providerKey = "Void";

        if (settings.InputElementProviders_UGUI.TryGetValue(providerKey, out provider))
        {
          return provider;
        }
        return null;
      }

      var firstParameterString = parameterTypesNames[0];
      var type = Type.GetType(firstParameterString);

      if (type == null) { return null; }

      providerKey = type.Name;

      if (settings.InputElementProviders_UGUI.TryGetValue(providerKey, out provider))
      {
        return provider;
      }
      if (provider == null && typeof(Enum).IsAssignableFrom(type))
      {
        if (!settings.InputElementProviders_UGUI.TryGetValue("Enum", out provider))
        {
          return null;
        }
      }

      return provider;
    }

    public static RectTransform FindInputFieldForParameters_UGUI(
      RectTransform root, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames)
    {
      var provider = GetSettingInputElementProvider_UGUI(settings, parameterTypesNames);
      if (provider != null)
      {
        return provider.FindInputElement(root, settingIdentifier, settings);
      }
      return null;
    }

    public static RectTransform CreateInputFieldForParameters_UGUI(
      string settingIdentifier, SettingsCollection settings,
      string labelText, List<string> parameterTypesNames)
    {
      var provider = GetSettingInputElementProvider_UGUI(settings, parameterTypesNames);
      return provider != null ? provider.GetInputElement(settingIdentifier, settings) : null;
    }

    public static bool UpdateInputFieldForParameters_UGUI(
      RectTransform elem, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames, List<object> values, bool initialize)
    {
      var provider = GetSettingInputElementProvider_UGUI(settings, parameterTypesNames);
      if (provider == null) { return false; }
      return provider.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
    }

    public static bool UpdateInputFieldFallback_UGUI(
      RectTransform elem, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames, List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      ConsoleLogger.LogWarning($"Incompatible input element provider assigned for the '{settingIdentifier}' setting. " +
                               $"Attempting to use fallback initialization.", 
                               Common.LogType.EditorAndDevelopmentBuildAndDebug);

      if (ProviderUtility_UGUI_Slider.UpdateInputElement
         (elem, settingIdentifier, settings, values, initialize, 0, 1, false, default)) { return true; }

      if (ProviderUtility_UGUI_Dropdown.UpdateInputElement
         (elem, settingIdentifier, settings, values, initialize)) { return true; }

      if (ProviderUtility_UGUI_PreviousNextSelector.UpdateInputElement
         (elem, settingIdentifier, settings, values, initialize, true, true)) { return true; }

      if (ProviderUtility_UGUI_Toggle.UpdateInputElement
         (elem, settingIdentifier, settings, values, initialize, default)) { return true; }

      ConsoleLogger.LogWarning($"Fallback initialization for {settingIdentifier} failed. " +
                               $"Couldn't find a valid input element.");

      return false;
    }

    public static RectTransform GetSettingElementInHierarchy<T>(RectTransform root, string settingIdentifier)
    {
      if (root == null) { return null; }
      var objects = root.GetComponentsInChildren<SettingObject>(true, true);
      if (objects == null || objects.Length == 0) { return null; }
      var obj = objects.Find(i => i.GetComponent<T>() != null &&
                                  i.Identifier == settingIdentifier);
      return obj != null ? obj.GetComponent<RectTransform>() : null;
    }

    public static bool IsCorrectInputElement(RectTransform elem, Type inputFieldType)
    {
      if (elem == null) { return false; }
      var field = inputFieldType != null ? elem.GetComponentInChildren(inputFieldType, true) : null;
      if (field == null) { return false; }
      return true;
    }
  }
}