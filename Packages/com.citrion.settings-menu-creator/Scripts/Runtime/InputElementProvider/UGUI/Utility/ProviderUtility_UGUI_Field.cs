using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  // TODO finish implementation and test
  public static class ProviderUtility_UGUI_Field
  {
    public static Type InputFieldType => typeof(TMP_InputField);

    public static bool UpdateInputElement<T>(RectTransform elem, string settingIdentifier,
                                             SettingsCollection settings, List<object> values,
                                             bool initialize)
    {
      if (elem == null) { return false; }
      var field = elem.GetComponentInChildren<TMP_InputField>(true);
      if (field == null) { return false; }

      if (initialize)
      {
        if (typeof(T) == typeof(int))
        {
          field.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
        else if (typeof(T) == typeof(float))
        {
          field.contentType = TMP_InputField.ContentType.DecimalNumber;
        }
      }

      string stringValue = null;
      if (ProviderUtility.GetValueFromValues<T>(values, out var value))
      {
        stringValue = value.ToString();
        field.SetTextWithoutNotify(stringValue);
      }
      if (initialize && Application.isPlaying)
      {
        //if (settings != null && stringValue != null)
        //{
        //  settings.ApplySettingChange(settingIdentifier, false, false, stringValue);
        //}

        field.onValueChanged.RemoveAllListeners();
        field.onValueChanged.AddListener((value) =>
        {
          if (settings != null)
          {
            settings.ApplySettingChange(settingIdentifier, false, false, value);
          }
        });
      }
      return true;
    }
  }
}
