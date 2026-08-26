using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public class LocalizationManipulator_Button : LocalizationManipulator
  {
    public static Dictionary<Button, LocalizationManipulator_Button> localizers = new();

    [SerializeField]
    [Tooltip("The button component to localize.")]
    protected Button button;

    [SerializeField]
    [Tooltip("The localization key used for the localization.")]
    protected string localizationKey = string.Empty;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      localizers.Clear();
    }

    public LocalizationManipulator_Button(LocalizationProvider provider) :
      base(provider)
    { }

    public string LocalizationKey { get => localizationKey; set => localizationKey = value; }

    public Button Button
    {
      get
      {
        TryCacheButton();
        return button;
      }
      set => button = value;
    }

    private void TryCacheButton()
    {
      if (button != null) { return; }
      button = target.Q<Button>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || Button == null ||
          string.IsNullOrEmpty(LocalizationKey)) { return; }

      var localizedText = localizationProvider.Localize(LocalizationKey);

      button.SetText(localizedText);
    }
  }
}
