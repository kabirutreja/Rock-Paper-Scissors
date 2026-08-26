using CitrioN.UI;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddComponentMenu("CitrioN/Localization/Previous Next Selector Localizer (UGUI)")]
  public class PreviousNextSelectorLocalizer_UGUI : LocalizationBehaviour
  {
    [SerializeField]
    [Tooltip("The previous next selector component to localize.")]
    protected PreviousNextSelector selector;

    [SerializeField]
    [Tooltip("The localization keys for the selector's options.")]
    protected List<string> optionKeys = new List<string>();

    public List<string> OptionKeys { get => optionKeys; set => optionKeys = value; }

    public PreviousNextSelector Selector
    {
      get
      {
        TryCacheSelector();
        return selector;
      }
      set => selector = value;
    }

    private void Reset()
    {
      TryCacheSelector();
    }

    private void TryCacheSelector()
    {
      if (selector != null) { return; }
      selector = GetComponent<PreviousNextSelector>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || Selector == null || 
          optionKeys == null || optionKeys.Count < 1) { return; }

      var localizedOptions = localizationProvider.Localize(optionKeys);

      var currentIndex = selector.CurrentIndex;
      selector.ClearOptions();
      selector.AddOptions(localizedOptions);
      selector.CurrentIndex = currentIndex;
    }
  }
}