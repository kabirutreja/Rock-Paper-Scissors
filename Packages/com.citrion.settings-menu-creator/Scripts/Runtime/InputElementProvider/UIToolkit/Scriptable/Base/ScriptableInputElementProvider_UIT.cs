using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HideScriptField]
  [System.Serializable]
  public abstract class ScriptableInputElementProvider_UIT : ScriptableObject, IGenericInputElementProvider<VisualElement>
  {
    public virtual IGenericInputElementProvider<VisualElement> GetProvider(SettingsCollection settings) => this;

    public virtual string Name => this.GetType().Name;

    public abstract Type GetInputFieldParameterType(SettingsCollection settings);

    public abstract Type GetInputFieldType(SettingsCollection settings);

    public virtual VisualElement FindInputElement(VisualElement root, string settingIdentifier, 
                                                  SettingsCollection settings)
    {
      return ProviderUtility_UIT.FindInputElementBase(root, settingIdentifier, settings, GetInputFieldType(settings));
    }

    public abstract VisualElement GetInputElement(string settingIdentifier, SettingsCollection settings);

    public virtual bool UpdateInputElement(VisualElement elem, string settingIdentifier,
                                            string labelText, SettingsCollection settings,
                                            List<object> values, bool initialize)
    {
      return ProviderUtility_UIT.UpdateInputElementBase(elem, labelText, settings, initialize);
    }
  }
}
