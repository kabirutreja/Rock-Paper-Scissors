using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UIT_Toggle
  {
    private static Dictionary<Toggle, EventCallback<ChangeEvent<bool>>> onValueChangedListeners = new();

    public static Type InputFieldType => typeof(Toggle);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onValueChangedListeners.Clear();
    }

    public static VisualElement GetInputElement(string settingIdentifier)
    {
      var toggle = new Toggle();
      toggle.AddToClassList(settingIdentifier);
      return toggle;
    }

    public static bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                          SettingsCollection settings, string labelText,
                                          List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      if (!ProviderUtility_UIT.IsCorrectInputElementType<Toggle>(elem))
      {
        return false;
      }

      //var toggle = elem as Toggle;
      var toggle = elem.Q<Toggle>();
      if (toggle == null) { return false; }
      // TODO Add localization
      toggle.label = labelText;

      if (initialize)
      {
        toggle.AddToClassList(ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);
      }

      if (ProviderUtility.GetValueFromValues<bool>(values, out var value))
      {
        toggle.SetValueWithoutNotify(value);
      }
      else if (values?.Count > 0)
      {
        var settingsHolder = settings != null ? settings.GetSettingHolder(settingIdentifier) : null;
        var option = settingsHolder?.GetOptionForKey(values[0]);
        if (option != null)
        {
          if (option.Value.ToLowerInvariant() == "true" ||
              option.Value.ToLowerInvariant() == "1")
          {
            toggle.SetValueWithoutNotify(true);
          }
          else if (option.Value.ToLowerInvariant() == "false" ||
                   option.Value.ToLowerInvariant() == "0")
          {
            toggle.SetValueWithoutNotify(false);
          }
          else
          {
            var index = settingsHolder.Options.IndexOf(option);
            if (index == 0) { toggle.SetValueWithoutNotify(false); }
            else if (index == 1) { toggle.SetValueWithoutNotify(true); }
          }
        }
      }

      //if (initialize && settings != null)
      //{
      //  settings.ApplySettingChange(settingIdentifier, false, false, toggle.value);
      //}

      onValueChangedListeners.TryGetValue(toggle, out var eventCallback);
      toggle.TryRegisterValueChangedCallbackForSetting(settingIdentifier, settings, initialize, eventCallback);
      if (eventCallback != null)
      {
        onValueChangedListeners.AddOrUpdateDictionaryItem(toggle, eventCallback);
      }
      return true;
    }
  }
}
