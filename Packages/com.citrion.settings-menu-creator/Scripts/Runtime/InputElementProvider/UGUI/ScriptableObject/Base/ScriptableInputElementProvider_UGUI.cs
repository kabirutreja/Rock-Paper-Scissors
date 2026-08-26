using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HideScriptField]
  public abstract class ScriptableInputElementProvider_UGUI : ScriptableObject, IGenericInputElementProvider<RectTransform>
  {
    public virtual IGenericInputElementProvider<RectTransform> GetProvider(SettingsCollection settings) => this;

    public virtual string Name => this.GetType().Name;

    public abstract Type GetInputFieldParameterType(SettingsCollection settings);

    public abstract Type GetInputFieldType(SettingsCollection settings);

    public virtual RectTransform FindInputElement(RectTransform root, string settingIdentifier, SettingsCollection settings)
    {
      var elementRoot = ProviderUtility_UGUI.GetSettingElementInHierarchy<RectTransform>(root, settingIdentifier);
      var inputFieldType = GetInputFieldType(settings);
      var component = elementRoot != null && inputFieldType != null ?
                      elementRoot.GetComponentInChildren(GetInputFieldType(settings), true) : null;

      if (elementRoot != null && component == null && inputFieldType != null)
      {
        //ConsoleLogger.LogWarning($"Found input element for setting '{settingIdentifier}' with the wrong input element type");
        //return null;
      }
      return elementRoot;
    }

    public abstract RectTransform GetInputElement(string settingIdentifier, SettingsCollection settings);

    public virtual bool UpdateInputElement(RectTransform elem, string settingIdentifier,
      string labelText, SettingsCollection settings, List<object> values, bool initialize)
    {
      if (elem == null) { return false; }

      var label = elem.GetComponentInChildren<ProviderLabel_UGUI>();
      if (label == null) { return true; }

      label.SetLabelText(labelText);

      #region Localization
      if (Application.isPlaying)
      {
        var textComponentLocalizer = label.GetComponent<TextMeshProUGUILocalizer>();
        if (textComponentLocalizer != null)
        {
          textComponentLocalizer.LocalizationKey = labelText;
          textComponentLocalizer.AssignProvider(settings.LocalizationProvider, false);
          textComponentLocalizer.Localize();
        }
      }
      #endregion

      GlobalEventHandler.InvokeEvent("OnSettingLabelTextChanged", label, labelText);
      return true;
    }
  }
}
