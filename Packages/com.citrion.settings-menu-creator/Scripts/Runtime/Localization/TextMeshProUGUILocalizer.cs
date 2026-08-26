using TMPro;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddComponentMenu("CitrioN/Localization/Text Mesh Pro UGUI Localizer")]
  public class TextMeshProUGUILocalizer : LocalizationBehaviour
  {
    [SerializeField]
    [Tooltip("The Text Mesh Pro UGUI component to localize.")]
    protected TextMeshProUGUI textComponent;

    [SerializeField]
    [Tooltip("The localization key used for the localization.")]
    protected string localizationKey = string.Empty;

    public string LocalizationKey { get => localizationKey; set => localizationKey = value; }

    public TextMeshProUGUI TextComponent
    {
      get
      {
        TryCacheTextComponent();
        return textComponent;
      }
      set => textComponent = value;
    }

    private void Reset()
    {
      TryCacheTextComponent();
    }

    private void TryCacheTextComponent()
    {
      if (textComponent != null) { return; }
      textComponent = GetComponent<TextMeshProUGUI>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || TextComponent == null || 
          string.IsNullOrEmpty(LocalizationKey)) { return; }

      var localizedText = localizationProvider.Localize(LocalizationKey);

      textComponent.SetText(localizedText);
    }
  }
}
