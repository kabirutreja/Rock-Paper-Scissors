using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UIT_Button
  {
    private static Dictionary<Button, Action> onClickListeners = new();

    public static Type InputFieldType => typeof(Button);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onClickListeners.Clear();
    }

    public static VisualElement GetInputElement(string settingIdentifier)
    {
      var button = new Button();
      button.AddToClassList(settingIdentifier);
      return button;
    }

    public static bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                          SettingsCollection settings, string labelText,
                                          List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      if (!ProviderUtility_UIT.IsCorrectInputElementType<Button>(elem))
      {
        return false;
      }

      //var button = elem as Button;
      var button = elem.Q<Button>();
      if (button == null) { return false; }

      if (initialize)
      {
        button.text = labelText;

        #region Localization
        if (Application.isPlaying)
        {
          if (!LocalizationManipulator_Button.localizers.TryGetValue(button, out var localizer))
          {
            localizer = new LocalizationManipulator_Button(settings.LocalizationProvider);
            button.AddManipulator(localizer);
            LocalizationManipulator_Button.localizers.Add(button, localizer);
          }

          localizer.AssignProvider(settings.LocalizationProvider, true);
          localizer.LocalizationKey = labelText;
          localizer.Localize();
        }
        #endregion
      }

      if (initialize && Application.isPlaying)
      {
        button.AddToClassList(ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);

        if (settings != null)
        {
          bool listenerExists = onClickListeners.TryGetValue(button, out var listener);

          if (listenerExists && listener != null)
          {
            button.clicked -= listener;
          }

          listener = () =>
          {
            if (settings != null)
            {
              settings.ApplySettingChange(settingIdentifier, forceApply: false, false, null);
            }
          };

          onClickListeners.AddOrUpdateDictionaryItem(button, listener);
          button.clicked += listener;

          //button.clickable = new Clickable(()
          //  => settings?.ApplySettingChange(settingIdentifier, forceApply: false, false, null));
          //button.clicked += () => settings.ApplySettingChange(settingIdentifier, forceApply: false, false, null);
        }
      }
      return true;
    }
  }
}
