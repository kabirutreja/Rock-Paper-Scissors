using CitrioN.Common;
using CitrioN.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UGUI_Slider
  {
    private static Dictionary<Slider, UnityEngine.Events.UnityAction<float>> onValueChangedListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onValueChangedListeners.Clear();
    }

    public static Type InputFieldType => typeof(Slider);

    public static bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                              SettingsCollection settings, List<object> values,
                                              bool initialize, float minSliderValue, float maxSliderValue,
                                              bool wholeNumbers, Slider.Direction direction)
    {
      if (elem == null) { return false; }
      var field = elem.GetComponentInChildren<Slider>(true);
      if (field == null) { return false; }

      var options = settings != null ? settings.GetSettingHolder(settingIdentifier).Options : null;

      if (initialize)
      {
        bool minValueSet = false;
        bool maxValueSet = false;

        if (options != null)
        {
          // MIN RANGE VALUE
          if (ProviderUtility.TryGetFloatValueFromOptions(options,
              SettingsMenuVariables.MIN_RANGE, out var minValue))
          {
            field.minValue = minValue;
            minValueSet = true;
          }

          // MAX RANGE VALUE
          if (ProviderUtility.TryGetFloatValueFromOptions(options,
              SettingsMenuVariables.MAX_RANGE, out var maxValue))
          {
            field.maxValue = maxValue;
            maxValueSet = true;
          }

          if (field is StepSlider stepSlider)
          {
            // STEP SIZE VALUE
            if (ProviderUtility.TryGetFloatValueFromOptions(options,
                SettingsMenuVariables.STEP_SIZE, out var stepSize))
            {
              stepSlider.StepSize = stepSize;
            }

            // STEP COUNT VALUE
            if (ProviderUtility.TryGetIntValueFromOptions(options,
                SettingsMenuVariables.STEP_COUNT, out var stepCount))
            {
              stepSlider.StepCount = stepCount;
            }

            // VALUE VISUALS MULTIPLIER
            if (ProviderUtility.TryGetFloatValueFromOptions(options,
                SettingsMenuVariables.VALUE_VISUALS_MULTIPLIER, out var visualsMultiplier))
            {
              stepSlider.ValueVisualsMultiplier = visualsMultiplier;
            }

            // VALUE VISUALS OFFSET
            if (ProviderUtility.TryGetFloatValueFromOptions(options,
                SettingsMenuVariables.VALUE_VISUALS_OFFSET, out var visualsOffset))
            {
              stepSlider.ValueVisualsOffset = visualsOffset;
            }

            // VALUE SUFFIX
            if (ProviderUtility.GetVariableValue(options, SettingsMenuVariables.VALUE_SUFFIX, out var valueSuffix))
            {
              stepSlider.ValueVisualsSuffix = valueSuffix;
            }
          }
        }

        if (!minValueSet)
        {
          field.minValue = minSliderValue;
        }
        if (!maxValueSet)
        {
          field.maxValue = maxSliderValue;
        }
        field.wholeNumbers = wholeNumbers;
        field.direction = direction;
      }

      if (ProviderUtility.GetValueFromValues<float>(values, out var value))
      {
        field.SetValueWithoutNotify(value);
      }
      else if (ProviderUtility.GetValueFromValues<int>(values, out var intValue))
      {
        field.SetValueWithoutNotify(intValue);
      }
      else if (values?.Count > 0)
      {
        // Handle case in which the value is not of the correct type

        var firstValue = values?[0];
        if (firstValue != null)
        {
          var stringValue = firstValue.ToString();

          var matchingOption = options.Find(o => o.Key == stringValue);
          if (matchingOption == null)
          {
            matchingOption = options.Find(o => o.Value == stringValue);
          }

          if (matchingOption != null)
          {
            // Use the index so the actual option value can be used for dislay purposes
            stringValue = options.IndexOf(matchingOption).ToString();
          }

          if (initialize)
          {
            field.wholeNumbers = true;
            field.minValue = 0;
            field.maxValue = (options.Count - 1).ClampLowerTo0();
          }

          //if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
          if (FloatExtensions.TryParseFloat(stringValue, out value))
          {
            field.SetValueWithoutNotify(value);
          }
        }
      }

      if (initialize && Application.isPlaying)
      {
        //if (settings != null)
        //{
        //  settings.ApplySettingChange(settingIdentifier, false, false, field.value);
        //}

        // OLD
        //if (SettingHolder.InitializedInputElements.TryGetValue(elem, out var isInitialized))
        //{
        //  if (isInitialized)
        //  {
        //    return true;
        //  }
        //}

        //field.onValueChanged.AddListener((value) =>
        //{
        //  if (settings != null)
        //  {
        //    settings.ApplySettingChange(settingIdentifier, false, false, value);
        //  }
        //});

        //

        bool listenerExists = onValueChangedListeners.TryGetValue(field, out var listener);

        if (listenerExists && listener != null)
        {
          field.onValueChanged.RemoveListener(listener);
        }

        listener = (value) =>
        {
          if (settings != null)
          {
            settings.ApplySettingChange(settingIdentifier, false, false, value);
          }
        };

        onValueChangedListeners.AddOrUpdateDictionaryItem(field, listener);
        field.onValueChanged.AddListener(listener);
      }
      return true;
    }
  }
}
