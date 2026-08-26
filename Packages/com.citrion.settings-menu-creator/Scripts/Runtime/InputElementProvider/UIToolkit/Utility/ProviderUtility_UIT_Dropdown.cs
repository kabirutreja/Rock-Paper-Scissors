using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UIT_Dropdown
  {
    public static Type InputFieldType => typeof(DropdownField);

    private static Dictionary<DropdownField, EventCallback<ChangeEvent<string>>> onValueChangedListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onValueChangedListeners.Clear();
    }

    public static VisualElement GetInputElement(string settingIdentifier,
      SettingsCollection settings, List<VisualTreeAsset> templates)
    {
      //var dropdown = new DropdownField();
      //dropdown.AddToClassList(settingIdentifier);
      //return dropdown;
      return ProviderUtility_UIT.GetInputElementBase<DropdownField>(settingIdentifier, settings, templates);
    }

    public static bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                          SettingsCollection settings, string labelText,
                                          List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      if (!ProviderUtility_UIT.IsCorrectInputElementType<DropdownField>(elem))
      {
        return false;
      }

      var dropdown = elem.Q<DropdownField>();
      if (dropdown == null) { return false; }
      dropdown.label = labelText;

      var settingsHolder = settings.GetSettingHolder(settingIdentifier);

      if (initialize)
      {
        dropdown.AddToClassList(ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);

        var optionsList = settingsHolder?.DisplayOptions;

        #region Check For Variables
        var options = settingsHolder?.Options;
        optionsList = ProviderUtility.GetOptionsList(options, optionsList);

        //float minValue = 0;
        //float maxValue = 1;
        //bool minValueFound = false;
        //bool maxValueFound = false;
        //float stepSize = 1f;

        //// MIN RANGE VALUE
        //var minValueData = options.Find(o => o.Key == SettingsMenuVariables.MIN_RANGE);
        //if (minValueData != null)
        //{
        //  //if (float.TryParse(minValueData.Value, NumberStyles.Float,
        //  //                   CultureInfo.InvariantCulture, out minValue))
        //  if (FloatExtensions.TryParseFloat(minValueData.Value, out minValue))
        //  {
        //    minValueFound = true;
        //  }
        //}

        //// MAX RANGE VALUE
        //var maxValueData = options.Find(o => o.Key == SettingsMenuVariables.MAX_RANGE);
        //if (maxValueData != null)
        //{
        //  //if (float.TryParse(maxValueData.Value, NumberStyles.Float,
        //  //                   CultureInfo.InvariantCulture, out maxValue))
        //  if (FloatExtensions.TryParseFloat(maxValueData.Value, out maxValue))
        //  {
        //    maxValueFound = true;
        //  }
        //}

        //// STEP SIZE VALUE
        //var stepSizeData = options.Find(o => o.Key == SettingsMenuVariables.STEP_SIZE);
        //if (stepSizeData != null)
        //{
        //  //if (!float.TryParse(stepSizeData.Value, NumberStyles.Float,
        //  //                   CultureInfo.InvariantCulture, out stepSize))
        //  if (!FloatExtensions.TryParseFloat(stepSizeData.Value, out stepSize))
        //  {
        //    stepSize = 1f;
        //  }
        //}

        //if (minValueFound && maxValueFound)
        //{
        //  optionsList = new List<string>();
        //  for (float i = minValue; i <= maxValue; i += stepSize)
        //  {
        //    optionsList.Add(i.ToString());
        //  }
        //}
        #endregion

        if (optionsList != null)
        {
          dropdown.choices = optionsList;
        }

        #region Localization
        if (Application.isPlaying)
        {
          if (!LocalizationManipulator_Dropdown.localizers.TryGetValue(dropdown, out var localizer))
          {
            localizer = new LocalizationManipulator_Dropdown(settings.LocalizationProvider);
            dropdown.AddManipulator(localizer);
            LocalizationManipulator_Dropdown.localizers.Add(dropdown, localizer);
          }

          localizer.AssignProvider(settings.LocalizationProvider, true);
          localizer.OptionKeys = optionsList;
          localizer.Localize();
        }
        #endregion
      }

      if (values?.Count > 0 && values[0] != null)
      {
        // TODO Remove later
        //var value = settingsHolder.GetOptionValueForKey(values[0]);
        //var dropdownOption = dropdown.choices.Find(c => c == value);

        var selectedDropdownIndex = SettingsUtility.GetDropdownIndexForValue(dropdown, settingsHolder, values[0]);

        // If the dropdown option is -1 it may be localized
        if (selectedDropdownIndex == -1)
        {
          string stringValue = null;
          stringValue = ProviderUtility.ParseValueToString(values);

          if (stringValue != null)
          {
            stringValue = settingsHolder.GetOptionValueForKey(stringValue);

            LocalizationManipulator_Dropdown.localizers.TryGetValue(dropdown, out var dropdownLocalizer);
            if (dropdownLocalizer != null && dropdownLocalizer.OptionKeys != null)
            {
              var selectedOptionIndex = dropdownLocalizer.OptionKeys.IndexOf(stringValue);
              if (selectedOptionIndex != -1)
              {
                selectedDropdownIndex = selectedOptionIndex;
              }
            }
          }
        }

        selectedDropdownIndex = selectedDropdownIndex.ClampLowerTo0();
        dropdown.SetValueWithoutNotify(dropdown.choices[selectedDropdownIndex]);
      }

      //if (initialize && settings != null)
      //{
      //  settings.ApplySettingChange(settingIdentifier, false, false, dropdown.value);
      //}

      if (initialize && Application.isPlaying)
      {
        // Old
        //string stringValue = string.Empty;

        //dropdown.RegisterValueChangedCallback((evt) =>
        //{
        //  if (settings != null)
        //  {
        //    var setting = settings.Settings?.Find(s => s.Identifier == settingIdentifier);

        //    if (setting != null)
        //    {
        //      var index = dropdown.choices.IndexOf(evt.newValue);
        //      if (index != -1)
        //      {
        //        stringValue = setting.Options[index]?.Value;
        //      }
        //    }

        //    settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
        //  }
        //});

        bool listenerExists = onValueChangedListeners.TryGetValue(dropdown, out var listener);

        if (listenerExists && listener != null)
        {
          dropdown.UnregisterValueChangedCallback(listener);
        }

        listener = (evt) =>
        {
          if (settings != null)
          {
            string stringValue = string.Empty;

            var setting = settings.Settings?.Find(s => s.Identifier == settingIdentifier);

            if (setting != null)
            {
              var index = dropdown.choices.IndexOf(evt.newValue);
              if (index != -1)
              {
                stringValue = setting.Options[index]?.Value;
              }
            }

            settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
          }
        };

        onValueChangedListeners.AddOrUpdateDictionaryItem(dropdown, listener);
        dropdown.RegisterValueChangedCallback(listener);
      }

      //dropdown.TryRegisterValueChangedCallbackForSetting(settingIdentifier, settings, initialize);
      return true;
    }
  }
}
