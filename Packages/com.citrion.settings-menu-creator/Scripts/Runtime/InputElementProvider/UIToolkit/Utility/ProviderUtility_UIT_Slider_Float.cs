using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UIT_Slider_Float
  {
    public static Type InputFieldType => typeof(Slider);

    private static Dictionary<Slider, EventCallback<ChangeEvent<float>>> onValueChangedListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onValueChangedListeners.Clear();
    }

    public static VisualElement GetInputElement(string settingIdentifier)
    {
      var slider = new Slider();
      slider.AddToClassList(settingIdentifier);
      return slider;
    }

    public static bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                          SettingsCollection settings, string labelText, List<object> values,
                                          bool initialize, float minSliderValue, float maxSliderValue,
                                          bool showInputField, bool inverted, SliderDirection direction)
    {
      if (elem == null) { return false; }

      if (!ProviderUtility_UIT.IsCorrectInputElementType<Slider>(elem))
      {
        return false;
      }

      //var slider = elem as Slider;
      var slider = elem.Q<Slider>();
      if (slider == null) { return false; }

      // TODO Add localization
      slider.label = labelText;

      var options = settings != null ? settings.GetSettingHolder(settingIdentifier).Options : null;

      if (initialize)
      {
        slider.AddToClassList(ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);

        bool minValueSet = false;
        bool maxValueSet = false;

        if (options != null)
        {
          // MIN RANGE VALUE
          if (ProviderUtility.TryGetFloatValueFromOptions(options, 
              SettingsMenuVariables.MIN_RANGE, out var minValue))
          {
            slider.lowValue = minValue;
            minValueSet = true;
          }

          // MAX RANGE VALUE
          if (ProviderUtility.TryGetFloatValueFromOptions(options,
              SettingsMenuVariables.MAX_RANGE, out var maxValue))
          {
            slider.highValue = maxValue;
            maxValueSet = true;
          }
        }

        if (!minValueSet)
        {
          slider.lowValue = minSliderValue;
        }
        if (!maxValueSet)
        {
          slider.highValue = maxSliderValue;
        }

        slider.showInputField = showInputField;
        slider.inverted = inverted;
        slider.direction = direction;
      }

      if (ProviderUtility.GetValueFromValues<float>(values, out var value))
      {
        slider.SetValueWithoutNotify(value);
      }
      else if (ProviderUtility.GetValueFromValues<int>(values, out var intValue))
      {
        slider.SetValueWithoutNotify(intValue);
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
            //slider.wholeNumbers = true;
            slider.lowValue = 0;
            slider.highValue = (options.Count - 1).ClampLowerTo0();
          }

          if (FloatExtensions.TryParseFloat(stringValue, out value))
          {
            slider.SetValueWithoutNotify(value);
          }
        }
      }

      //if (initialize && settings != null)
      //{
      //  settings.ApplySettingChange(settingIdentifier, false, false, slider.value);
      //}
      onValueChangedListeners.TryGetValue(slider, out var eventCallback);
      slider.TryRegisterValueChangedCallbackForSetting(settingIdentifier, settings, initialize, eventCallback);
      if (eventCallback != null)
      {
        onValueChangedListeners.AddOrUpdateDictionaryItem(slider, eventCallback);
      }
      return true;
    }
  }
}
