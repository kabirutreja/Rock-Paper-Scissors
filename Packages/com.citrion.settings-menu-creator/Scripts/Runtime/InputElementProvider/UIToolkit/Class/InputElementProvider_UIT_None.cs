using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(9999)]
  [DisplayName("Auto")]
  [System.Serializable]
  public class InputElementProvider_UIT_None : InputElementProvider_UIT
  {
    // We return null so it will not be used for processing input elements
    // Essentially makes sure it will be skipped and the default functionalities
    // will be used instead
    public override IGenericInputElementProvider<VisualElement> GetProvider(SettingsCollection settings) => null;

    public override string Name => "Auto";

    public override Type GetInputFieldType(SettingsCollection settings) => null;

    public override Type GetInputFieldParameterType(SettingsCollection settings) => null;

    public override VisualElement FindInputElement(VisualElement root, string settingIdentifier, SettingsCollection settings) => null;

    public override VisualElement GetInputElement(string settingIdentifier, SettingsCollection settings) => null;

    public override bool UpdateInputElement(VisualElement elem, string settingIdentifier, string labelText,
                                            SettingsCollection settings, List<object> values, bool initialize)
    { return true; }
  }
}