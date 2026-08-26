using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UIT
  {
    public const string INPUT_ELEMENT_LABEL_CLASS = "input-element__label";
    public const string INPUT_ELEMENT_SELECTABLE_CLASS = "input-element__selectable";
    public const string INPUT_ELEMENT_PROVIDER_ANCHOR_CLASS = "provider-anchor";

    public const string INPUT_ELEMENT_ROOT_CLASS = "input-element__root";
    public const string INPUT_ELEMENT_FOCUSED_CLASS = "input-element__focused";
    public const string INPUT_ELEMENT_UNFOCUSED_CLASS = "input-element__unfocused";

    public static ScriptableInputElementProvider_UIT GetSettingInputElementProvider_UIToolkit(
      SettingsCollection settings, List<string> parameterTypesNames)
    {
      if (settings == null || settings.InputElementProviders_UIT == null) { return null; }

      ScriptableInputElementProvider_UIT provider = null;
      string providerKey = null;

      if (parameterTypesNames == null || parameterTypesNames.Count == 0)
      {
        providerKey = "Void";

        if (settings.InputElementProviders_UIT.TryGetValue(providerKey, out provider))
        {
          return provider;
        }
        return null;
      }

      var firstParameterString = parameterTypesNames[0];
      var type = Type.GetType(firstParameterString);

      if (type == null) { return null; }

      providerKey = type.Name;

      if (settings.InputElementProviders_UIT.TryGetValue(providerKey, out provider))
      {
        return provider;
      }
      if (provider == null && typeof(Enum).IsAssignableFrom(type))
      {
        if (!settings.InputElementProviders_UIT.TryGetValue("Enum", out provider))
        {
          return null;
        }
      }

      return provider;
    }

    public static VisualElement FindInputFieldForParameters_UIToolkit(
      VisualElement root, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames)
    {
      var provider = GetSettingInputElementProvider_UIToolkit(settings, parameterTypesNames);
      if (provider != null)
      {
        return provider.FindInputElement(root, settingIdentifier, settings);
      }
      return null;
    }

    public static VisualElement CreateInputFieldForParameters_UIToolkit(
      string settingIdentifier, SettingsCollection settings,
      string labelText, List<string> parameterTypesNames)
    {
      var provider = GetSettingInputElementProvider_UIToolkit(settings, parameterTypesNames);
      return provider != null ? provider.GetInputElement(settingIdentifier, settings) : null;
    }

    public static bool UpdateInputFieldForParameters_UIToolkit(
      VisualElement elem, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames, List<object> values, bool initialize)
    {
      var provider = GetSettingInputElementProvider_UIToolkit(settings, parameterTypesNames);
      if (provider == null) { return false; }
      return provider.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);
    }

    public static bool UpdateInputFieldFallback_UIToolkit(
      VisualElement elem, SettingsCollection settings, string settingIdentifier,
      string labelText, List<string> parameterTypesNames, List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      if (ProviderUtility_UIT_Slider_Float.UpdateInputElement
         (elem, settingIdentifier, settings, labelText, values, initialize, 0, 1, true, false, default)) { return true; }

      if (ProviderUtility_UIT_Dropdown.UpdateInputElement
         (elem, settingIdentifier, settings, labelText, values, initialize)) { return true; }

      if (ProviderUtility_UIT_PreviousNextSelector.UpdateInputElement
         (elem, settingIdentifier, settings, labelText, values, initialize, true, true)) { return true; }

      if (ProviderUtility_UIT_Toggle.UpdateInputElement
         (elem, settingIdentifier, settings, labelText, values, initialize)) { return true; }

      ConsoleLogger.LogWarning($"Fallback initialization for {settingIdentifier} failed. " +
                               $"Couldn't find a valid input element.");

      return false;
    }

    public static void TryRegisterValueChangedCallbackForSetting<T>(
      this BaseField<T> field, string settingIdentifier,
      SettingsCollection settings, bool registerCallback,
      EventCallback<ChangeEvent<T>> eventCallback)
    {
      if (!registerCallback || field == null) { return; }
      if (!Application.isPlaying) { return; }

      if (eventCallback != null)
      {
        field.UnregisterValueChangedCallback(eventCallback);
      }

      eventCallback = (evt) =>
      {
        if (settings != null)
        {
          settings.ApplySettingChange(settingIdentifier, false, false, evt.newValue);
        }
      };

      field.RegisterValueChangedCallback(eventCallback);
    }

    public static bool IsCorrectInputElementType<T>(VisualElement elem) where T : VisualElement
    {
      //if (elem.GetType() != validInputElementType)
      if (elem.Q<T>() == null)
      {
        ConsoleLogger.LogWarning($"Input field element is of type {elem.GetType()}.\n" +
                                 $"It should be of type {typeof(T).Name}");
        return false;
      }
      return true;
    }

    public static VisualElement FindInputElementBase(VisualElement root, string settingIdentifier,
      SettingsCollection settings, Type inputFieldType)
    {
      if (root == null || string.IsNullOrEmpty(settingIdentifier)) { return null; }

      var elementRoot = root.Q(className: settingIdentifier);
      var elementType = elementRoot?.GetType();
      bool isMatchingType = elementRoot != null && inputFieldType != null &&
                            elementType == inputFieldType;
      if (elementRoot != null && !isMatchingType && inputFieldType != null)
      {
        // TODO Check if this should return null
        // or if it works fine with the fallback initialization
        //ConsoleLogger.LogWarning($"Found input element for setting '{settingIdentifier}' with the wrong input element type");
        return elementRoot;
      }
      return elementRoot;
    }

    public static VisualElement GetInputElementBase(string settingIdentifier,
      SettingsCollection settings, List<VisualTreeAsset> templates)
    {
      if (templates == null || templates.Count < 1) { return null; }

      VisualElement root = null;
      VisualElement previousInstanceRoot = null;

      for (int i = 0; i < templates.Count; i++)
      {
        // Create a new instance of the current template
        var templateContainer = templates[i].Instantiate();
        VisualElement instance = templateContainer;

        if (templateContainer.childCount > 0)
        {
          instance = templateContainer.ElementAt(0);

          // Add the style sheets from the template container
          // in case the template container will be removed.
          // This will ensure that any style sheets attached to the
          // original UXML file will not be lost.
          var styleSheetsCount = templateContainer.styleSheets.count;
          for (int j = 0; j < styleSheetsCount; j++)
          {
            instance.AddStyleSheet(templateContainer.styleSheets[j]);
          }
        }

        if (i == 0)
        {
          root = instance;
          root.AddToClassList(settingIdentifier);
        }
        else
        {
          VisualElement parent = previousInstanceRoot;
          if (previousInstanceRoot != null)
          {
            var anchor = previousInstanceRoot.Q(className: INPUT_ELEMENT_PROVIDER_ANCHOR_CLASS);
            if (anchor != null)
            {
              parent = anchor;
            }
          }

          parent?.Add(instance);
        }

        previousInstanceRoot = instance;
      }

      return root;
    }

    public static VisualElement GetInputElementBase<T>(string settingIdentifier,
  SettingsCollection settings, List<VisualTreeAsset> templates) where T : VisualElement
    {
      VisualElement root = null;
      VisualElement previousInstanceRoot = null;

      if (templates != null && templates.Count > 0)
      {
        for (int i = 0; i < templates.Count; i++)
        {
          // Create a new instance of the current template
          var templateContainer = templates[i].Instantiate();
          VisualElement instance = templateContainer;

          if (templateContainer.childCount > 0)
          {
            instance = templateContainer.ElementAt(0);

            // Add the style sheets from the template container
            // in case the template container will be removed.
            // This will ensure that any style sheets attached to the
            // original UXML file will not be lost.
            var styleSheetsCount = templateContainer.styleSheets.count;
            for (int j = 0; j < styleSheetsCount; j++)
            {
              instance.AddStyleSheet(templateContainer.styleSheets[j]);
            }
          }

          if (i == 0)
          {
            root = instance;
          }
          else
          {
            VisualElement parent = previousInstanceRoot;
            if (previousInstanceRoot != null)
            {
              var anchor = previousInstanceRoot.Q(className: INPUT_ELEMENT_PROVIDER_ANCHOR_CLASS);
              if (anchor != null)
              {
                parent = anchor;
              }
            }

            parent?.Add(instance);
          }

          previousInstanceRoot = instance;
        }
      }

      T elementOfType = root?.Q<T>();

      if (elementOfType == null)
      {
        // Create and attach an element of the specified type
        elementOfType = Activator.CreateInstance<T>();

        VisualElement parent = previousInstanceRoot;
        if (previousInstanceRoot != null)
        {
          var anchor = previousInstanceRoot.Q(className: INPUT_ELEMENT_PROVIDER_ANCHOR_CLASS);
          if (anchor != null)
          {
            parent = anchor;
          }
        }

        if (parent != null)
        {
          parent.Add(elementOfType);
        }
        else
        {
          root = elementOfType;
        }
      }

      root?.AddToClassList(settingIdentifier);
      return root;
    }

    public static bool UpdateInputElementBase(VisualElement elem, string labelText, SettingsCollection settings, bool initialize)
    {
      if (elem == null) { return false; }

      if (initialize)
      {
        elem.AddToClassList(INPUT_ELEMENT_ROOT_CLASS);

        elem.RemoveFromClassList(INPUT_ELEMENT_FOCUSED_CLASS);
        elem.AddToClassList(INPUT_ELEMENT_UNFOCUSED_CLASS);

        elem.UnregisterCallback<FocusInEvent>(OnInputElementRootFocusIn);
        elem.RegisterCallback<FocusInEvent>(OnInputElementRootFocusIn);

        elem.UnregisterCallback<FocusOutEvent>(OnInputElementRootFocusOut);
        elem.RegisterCallback<FocusOutEvent>(OnInputElementRootFocusOut);
      }

      var label = elem.Q<Label>(className: ProviderUtility_UIT.INPUT_ELEMENT_LABEL_CLASS);
      if (label == null) { return true; }

      // Set the raw label text.
      // If it is a localization key it will be updated via the localization.
      label.SetText(labelText);

      #region Localization
      if (Application.isPlaying)
      {
        if (!LocalizationManipulator_Label.localizers.TryGetValue(label, out var labelLocalizer))
        {
          labelLocalizer = new LocalizationManipulator_Label(settings.LocalizationProvider);
          label.AddManipulator(labelLocalizer);
          LocalizationManipulator_Label.localizers.Add(label, labelLocalizer);
        }

        labelLocalizer.AssignProvider(settings.LocalizationProvider, true);
        labelLocalizer.LocalizationKey = labelText;
        labelLocalizer.Localize();
      }
      #endregion

      GlobalEventHandler.InvokeEvent("OnSettingLabelTextChanged", label, labelText);
      return true;
    }

    public static void OnInputElementRootFocusIn(FocusInEvent evt)
    {
      var currentTarget = evt.currentTarget;
      if (currentTarget == null) { return; }
      var elem = currentTarget as VisualElement;
      if (elem == null) { return; }
      elem.RemoveFromClassList(INPUT_ELEMENT_UNFOCUSED_CLASS);
      elem.AddToClassList(INPUT_ELEMENT_FOCUSED_CLASS);
    }

    public static void OnInputElementRootFocusOut(FocusOutEvent evt)
    {
      var currentTarget = evt.currentTarget;
      if (currentTarget == null) { return; }
      var elem = currentTarget as VisualElement;
      if (elem == null) { return; }
      elem.RemoveFromClassList(INPUT_ELEMENT_FOCUSED_CLASS);
      elem.AddToClassList(INPUT_ELEMENT_UNFOCUSED_CLASS);
    }
  }
}
