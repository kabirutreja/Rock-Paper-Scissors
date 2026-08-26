using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using PreviousNextSelector = CitrioN.UI.UIToolkit.PreviousNextSelector;

namespace CitrioN.SettingsMenuCreator
{
  // TODO Add all classes similarly to the native dropdown field
  public static class ProviderUtility_UIT_PreviousNextSelector
  {
    public static Type InputFieldType => typeof(PreviousNextSelector);

    private static Dictionary<PreviousNextSelector, UnityAction<string>> onValueChangedListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onValueChangedListeners.Clear();
    }

    public static VisualElement GetInputElement(string settingIdentifier)
    {
      var prevNextSelector = new PreviousNextSelector();
      prevNextSelector.AddToClassList(settingIdentifier);
      return prevNextSelector;
    }

    public static bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                          SettingsCollection settings, string labelText,
                                          List<object> values, bool initialize,
                                          bool allowCycle, bool representNoCycleOnButtons)
    {
      if (elem == null) { return false; }

      if (!ProviderUtility_UIT.IsCorrectInputElementType<PreviousNextSelector>(elem))
      {
        return false;
      }

      //var field = elem as PreviousNextSelector;
      var field = elem.Q<PreviousNextSelector>();
      if (field == null) { return false; }
      field.label?.SetText(labelText);

      var settingsHolder = settings.GetSettingHolder(settingIdentifier);

      if (initialize)
      {
        field.AddToClassList(ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);

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
          if (!LocalizationManipulator_PreviousNextSelector.localizers.TryGetValue(field, out var localizer))
          {
            localizer = new LocalizationManipulator_PreviousNextSelector(settings.LocalizationProvider);
            field.AddManipulator(localizer);
            LocalizationManipulator_PreviousNextSelector.localizers.Add(field, localizer);
          }

          localizer.AssignProvider(settings.LocalizationProvider, true);
          localizer.OptionKeys = optionsList;
          localizer.Localize();
        }
        #endregion
      }

      // TODO Test extensively
      if (values?.Count > 0 && values[0] != null)
      {
        // OLD Pre localization code
        //var option = SettingsUtility.GetOptionForValue(field.Values, settingsHolder, values[0], out string value);

        //if (option != null)
        //{
        //  field.SetValueWithoutNotify(value);
        //}

        var selectedPreviousNextSelectorIndex = SettingsUtility.GetPreviousNextSelectorIndexForValue(field, settingsHolder, values[0]);

        // If the option is -1 it may be localized
        if (selectedPreviousNextSelectorIndex == -1)
        {
          // TODO Refactor this to a common method?
          string stringValue = null;
          stringValue = ProviderUtility.ParseValueToString(values);

          if (stringValue != null)
          {
            stringValue = settingsHolder.GetOptionValueForKey(stringValue);

            LocalizationManipulator_PreviousNextSelector.localizers.TryGetValue(field, out var localizer);
            if (localizer != null && localizer.OptionKeys != null)
            {
              var selectedOptionIndex = localizer.OptionKeys.IndexOf(stringValue);
              if (selectedOptionIndex != -1)
              {
                selectedPreviousNextSelectorIndex = selectedOptionIndex;
              }
            }
          }
        }

        selectedPreviousNextSelectorIndex = selectedPreviousNextSelectorIndex.ClampLowerTo0();
        field.SetValueWithoutNotify(field.Values[selectedPreviousNextSelectorIndex]);
      }

      // OLD
      //if (values?.Count > 0)
      //{
      //  var options = settingsHolder?.Options;
      //  var value = values[0].ToString();
      //  var relation = options.Find(o => o.Key == value);
      //  if (relation != null)
      //  {
      //    value = relation.Value;
      //  }
      //  field.SetValueWithoutNotify(value);
      //}

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

        // OLD Pre localization
        //field.onValueChanged.AddListener((value)
        //  => settings?.ApplySettingChange(settingIdentifier, false, false, value));


        //field.TryRegisterValueChangedCallbackForSetting(settingIdentifier, settings, initialize);

        bool listenerExists = onValueChangedListeners.TryGetValue(field, out var listener);

        if (listenerExists && listener != null)
        {
          field.onValueChanged.RemoveListener(listener);
        }

        listener = (currentValue) =>
        {
          if (settings != null)
          {
            string stringValue = string.Empty;

            var setting = settings.Settings?.Find(s => s.Identifier == settingIdentifier);

            if (setting != null)
            {
              var index = field.Values.IndexOf(currentValue);
              if (index != -1)
              {
                stringValue = setting.Options[index]?.Value;
              }
            }

            settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
          }
        };

        onValueChangedListeners.AddOrUpdateDictionaryItem(field, listener);
        field.onValueChanged.AddListener(listener);
      }
      return true;
    }

  }
}
