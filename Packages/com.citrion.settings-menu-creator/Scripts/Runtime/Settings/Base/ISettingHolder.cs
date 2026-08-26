using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscationRename]
  public interface ISettingHolder
  {
#if UNITY_EDITOR
    public string MenuName { get; }
#endif

    public string Identifier { get; set; }

    public Setting Setting { get; set; }

    public bool ApplyImmediately { get; set; }

    public InputElementProviderSettings InputElementProviderSettings { get; set; }

    public List<string> ParameterTypes { get; }

    public List<StringToStringRelation> Options { get; }

    public List<string> DisplayOptions { get; }

#if UNITY_EDITOR
    public int CurrentTabMenuIndex { get; set; }
#endif

    public bool StoreValueInternally { get; }

    [SkipObfuscationRename]
    public object ApplySettingChange(SettingsCollection settings, params object[] args);

    [SkipObfuscationRename]
    public VisualElement CreateElement_UIToolkit(VisualElement root, SettingsCollection settings);

    [SkipObfuscationRename]
    public VisualElement FindElement_UIToolkit(VisualElement root, SettingsCollection settings);

    [SkipObfuscationRename]
    public void InitializeElement_UIToolkit(VisualElement elem, SettingsCollection settings, bool initialize);

    [SkipObfuscationRename]
    public RectTransform CreateElement_UGUI(RectTransform root, SettingsCollection settings);

    [SkipObfuscationRename]
    public RectTransform FindElement_UGUI(RectTransform root, SettingsCollection settings);

    [SkipObfuscationRename]
    public void InitializeElement_UGUI(RectTransform elem, SettingsCollection settings, bool initialize);
  }
}
