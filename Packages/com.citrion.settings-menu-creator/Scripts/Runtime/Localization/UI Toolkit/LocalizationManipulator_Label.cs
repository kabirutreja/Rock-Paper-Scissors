using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public class LocalizationManipulator_Label : LocalizationManipulator
  {
    public static Dictionary<Label, LocalizationManipulator_Label> localizers = new();

    [SerializeField]
    [Tooltip("The label component to localize.")]
    protected Label label;

    [SerializeField]
    [Tooltip("The localization key used for the localization.")]
    protected string localizationKey = string.Empty;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      localizers.Clear();
    }

    public LocalizationManipulator_Label(LocalizationProvider provider) :
      base(provider) { }

    public string LocalizationKey { get => localizationKey; set => localizationKey = value; }

    public Label Label
    {
      get
      {
        TryCacheTextComponent();
        return label;
      }
      set => label = value;
    }

    private void TryCacheTextComponent()
    {
      if (label != null) { return; }
      label = target.Q<Label>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || Label == null ||
          string.IsNullOrEmpty(LocalizationKey)) { return; }

      var localizedText = localizationProvider.Localize(LocalizationKey);

      label.SetText(localizedText);
    }
  }
}
