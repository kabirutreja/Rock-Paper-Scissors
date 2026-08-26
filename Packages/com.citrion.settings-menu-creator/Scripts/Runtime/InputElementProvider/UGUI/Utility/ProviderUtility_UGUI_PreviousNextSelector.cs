using CitrioN.Common;
using CitrioN.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UGUI_PreviousNextSelector
  {
    public static Type InputFieldType => typeof(PreviousNextSelector);

    private static Dictionary<PreviousNextSelector, UnityAction<int, int>> onIndexChangedListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onIndexChangedListeners.Clear();
    }

    public static bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                          SettingsCollection settings, List<object> values, bool initialize,
                                          bool allowCycle, bool representNoCycleOnButtons)
    {
      if (elem == null) { return false; }
      var field = elem.GetComponentInChildren<PreviousNextSelector>(true);
      if (field == null) { return false; }

      var settingsHolder = settings.GetSettingHolder(settingIdentifier);

      if (initialize)
      {
        var optionsList = settingsHolder?.DisplayOptions;

        #region Check For Variables
        var options = settingsHolder?.Options;
        optionsList = ProviderUtility.GetOptionsList(options, optionsList);
        //float minValue = 0;
        //float maxValue = 1;
        //bool minValueFound = false;
        //bool maxValueFound = false;
        //float stepSize = 1f;

        //if (options != null && options.Count > 0)
        //{
        //  // MIN RANGE VALUE
        //  var minValueData = options.Find(o => o.Key == SettingsMenuVariables.MIN_RANGE);
        //  if (minValueData != null)
        //  {
        //    //if (float.TryParse(minValueData.Value, NumberStyles.Float,
        //    //                   CultureInfo.InvariantCulture, out minValue))
        //    if (FloatExtensions.TryParseFloat(minValueData.Value, out minValue))
        //    {
        //      minValueFound = true;
        //    }
        //  }

        //  // MAX RANGE VALUE
        //  var maxValueData = options.Find(o => o.Key == SettingsMenuVariables.MAX_RANGE);
        //  if (maxValueData != null)
        //  {
        //    //if (float.TryParse(maxValueData.Value, NumberStyles.Float,
        //    //                   CultureInfo.InvariantCulture, out maxValue))
        //    if (FloatExtensions.TryParseFloat(maxValueData.Value, out maxValue))
        //    {
        //      maxValueFound = true;
        //    }
        //  }

        //  // STEP SIZE VALUE
        //  var stepSizeData = options.Find(o => o.Key == SettingsMenuVariables.STEP_SIZE);
        //  if (stepSizeData != null)
        //  {
        //    //if (!float.TryParse(stepSizeData.Value, NumberStyles.Float,
        //    //                   CultureInfo.InvariantCulture, out stepSize))
        //    if (!FloatExtensions.TryParseFloat(stepSizeData.Value, out stepSize))
        //    {
        //      stepSize = 1f;
        //    }
        //  } 
        //}

        //if (minValueFound && maxValueFound)
        //{
        //  // We create a new list of options because the
        //  // min and max values were defined
        //  optionsList = new List<string>();
        //  for (float i = minValue; i <= maxValue; i += stepSize)
        //  {
        //    optionsList.Add(i.ToString());
        //  }
        //}
        #endregion

        field.AllowCycle = allowCycle;
        field.RepresentNoCycleOnButtons = representNoCycleOnButtons;

        field.ClearOptions();
        if (optionsList != null)
        {
          field.AddOptions(optionsList);
        }

        #region Localization
        if (Application.isPlaying)
        {
          if (field.gameObject.TryGetComponent<PreviousNextSelectorLocalizer_UGUI>(out var localizer))
          {
            localizer.OptionKeys = optionsList;
            localizer.AssignProvider(settings.LocalizationProvider, false);
            localizer.Localize();
          }
        }
        #endregion
      }

      //string currentValue = null;

      //if (values?.Count > 0)
      //{
      //  var options = settingsHolder?.Options;
      //  currentValue = values[0]?.ToString();

      //  var relation = options.Find(o => o.Key == currentValue);
      //  if (relation != null)
      //  {
      //    currentValue = relation.Value;
      //  }
      //  field.SetValueWithoutNotify(currentValue);
      //}

      // TODO Test extensively
      if (values?.Count > 0 && values[0] != null)
      {
        // TODO Remove later (old)
        //var value = settingsHolder?.GetOptionValueForKey(values[0]);
        //var dropdownOption = field.Values.Find(c => c == value);

        var option = SettingsUtility.GetOptionForValue(field.Values, settingsHolder, values[0], out string value);

        if (option != null)
        {
          field.SetValueWithoutNotify(value);
        }
        else
        {
          int selectedIndex = 0;

          // If the dropdown option is null it may be localized
          if (option == null)
          {
            string stringValue = null;
            stringValue = ProviderUtility.ParseValueToString(values);

            //var type = values[0].GetType();
            //if (values[0] is string)
            //{
            //  stringValue = (string)values[0];
            //}
            //else if (values[0] is bool)
            //{
            //  stringValue = values[0].ToString();
            //}
            //else if (typeof(Enum).IsAssignableFrom(type))
            //{
            //  //if (values[0] is int intValue)//
            //  {
            //    var enumName = Enum.GetName(type, values[0]);
            //    stringValue = enumName;
            //  }
            //}

            if (stringValue != null)
            {
              stringValue = settingsHolder.GetOptionValueForKey(stringValue);

              var localizer = field.gameObject.GetComponent<PreviousNextSelectorLocalizer_UGUI>();
              if (localizer != null && localizer.OptionKeys != null)
              {
                var selectedOptionIndex = localizer.OptionKeys.IndexOf(stringValue);
                if (selectedOptionIndex != -1)
                {
                  selectedIndex = selectedOptionIndex;
                }
              }
            }

            selectedIndex = selectedIndex.ClampLowerTo0();
            field.CurrentIndex = selectedIndex;
            //field.SetValueWithoutNotify(selectedIndex);
          }
        }
      }

      if (initialize && Application.isPlaying)
      {
        //if (settings != null)
        //{
        //  var currentValue = field.GetValue();
        //  if (currentValue != null)
        //  {
        //    settings.ApplySettingChange(settingIdentifier, false, false, currentValue);
        //  }
        //}

        // OLD
        //field.onIndexChanged.RemoveAllListeners();
        //field.onIndexChanged.AddListener((previousIndex, selectedIndex) =>
        //{
        //  if (settings != null)
        //  {
        //    string stringValue = string.Empty;
        //    var setting = settings.Settings?.Find(s => s.Identifier == settingIdentifier);
        //    if (setting != null)
        //    {
        //      stringValue = setting.Options[selectedIndex]?.Value;
        //    }
        //    settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
        //  }
        //});


        //field.onValueChanged.AddListener((value)
        //  => settings?.ApplySettingChange(settingIdentifier, false, false, value));

        bool listenerExists = onIndexChangedListeners.TryGetValue(field, out var listener);

        if (listenerExists && listener != null)
        {
          field.onIndexChanged.RemoveListener(listener);
        }

        listener = (previousIndex, selectedIndex) =>
        {
          if (settings != null && selectedIndex >= 0 && field.Values?.Count > selectedIndex)
          {
            var stringValue = field.Values[selectedIndex];
            var setting = settings.Settings?.Find(s => s.Identifier == settingIdentifier);
            if (setting != null)
            {
              stringValue = setting.Options[selectedIndex]?.Value;
            }
            settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
          }
        };

        onIndexChangedListeners.AddOrUpdateDictionaryItem(field, listener);
        field.onIndexChanged.AddListener(listener);
      }
      return true;
    }
  }
}
