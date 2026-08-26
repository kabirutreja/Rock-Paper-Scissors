using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [SkipObfuscationRename]
  public class InputElementProviderSettings
  {
    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("Custom text to be used for the input element label\n" +
             "instead of the default label text.")]
    protected string customLabel = string.Empty;

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("The identifier of the input element parent.\n\n" +
             "UI Toolkit:\n" +
             "Requires a VisualElement matching this class name\n" +
             "in the menu hierarchy.\n\n" +
             "UGUI:\n" +
             "Requires a GameObject with the SettingObject\n" +
             "component attached that matches this identifier.")]
    protected string parentIdentifier = "settings-parent";

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("Should a spacer VisualElement be added\n" +
             "after this settings input element?\n\n" +
             "Useful to add spacing between input elements.")]
    protected bool addSpacer = true;

    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("The class name to add to the spacer VisualElement.\n\n" +
             "Requires 'addSpacer' to be enabled.\n\n" +
             "This class name can be referred to in style sheets\n" +
             "to apply custom styles such as adding a margin\n" +
             "to add the desired spacing between input elements.")]
    protected string spacerElementClass = "setting-spacer";

    [SerializeField]
    [SerializeReference]
    [SkipObfuscationRename]
    protected InputElementProvider_UIT inputElementProvider_UIToolkit;

    [SerializeField]
    [SerializeReference]
    [SkipObfuscationRename]
    protected InputElementProvider_UGUI inputElementProvider_UGUI;

    public InputElementProvider_UIT InputElementProvider_UIToolkit
    {
      get => inputElementProvider_UIToolkit;
      set => inputElementProvider_UIToolkit = value;
    }

    public InputElementProvider_UGUI InputElementProvider_UGUI
    {
      get => inputElementProvider_UGUI;
      set => inputElementProvider_UGUI = value;
    }

    public string ParentIdentifier
    {
      get => parentIdentifier;
      set => parentIdentifier = value;
    }
    public bool AddSpacer { get => addSpacer; set => addSpacer = value; }
    public string SpacerElementClass { get => spacerElementClass; set => spacerElementClass = value; }
    public string CustomLabel { get => customLabel; set => customLabel = value; }

    public InputElementProviderSettings()
    {
      InputElementProvider_UIToolkit = new InputElementProvider_UIT_None();
      InputElementProvider_UGUI = new InputElementProvider_UGUI_None();
    }

    public InputElementProviderSettings(InputElementProvider_UIT inputElementProvider)
    {
      InputElementProvider_UIToolkit = inputElementProvider;
    }
  }
}