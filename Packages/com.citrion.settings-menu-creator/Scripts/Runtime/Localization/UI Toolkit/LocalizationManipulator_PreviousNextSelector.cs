using CitrioN.UI.UIToolkit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public class LocalizationManipulator_PreviousNextSelector : LocalizationManipulator
  {
    public static Dictionary<PreviousNextSelector, LocalizationManipulator_PreviousNextSelector> localizers = new();

    [SerializeField]
    [Tooltip("The previous next selector to localize.")]
    protected PreviousNextSelector previousNextSelector;

    [SerializeField]
    [Tooltip("The localization keys for the previous next selector's options.")]
    protected List<string> optionKeys = new List<string>();

    public List<string> OptionKeys { get => optionKeys; set => optionKeys = value; }

    public LocalizationManipulator_PreviousNextSelector(LocalizationProvider provider) :
      base(provider)
    { }

    public PreviousNextSelector PreviousNextSelector
    {
      get
      {
        TryCachePreviousNextSelector();
        return previousNextSelector;
      }
      set => previousNextSelector = value;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      localizers.Clear();
    }

    private void TryCachePreviousNextSelector()
    {
      if (previousNextSelector != null) { return; }
      previousNextSelector = target.Q<PreviousNextSelector>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || PreviousNextSelector == null ||
          optionKeys == null || optionKeys.Count < 1) { return; }

      var localizedOptions = localizationProvider.Localize(optionKeys);

      var selectedChoice = previousNextSelector.CurrentIndex;
      previousNextSelector.values = localizedOptions;

      if (selectedChoice >= 0 && selectedChoice < previousNextSelector.Values.Count)
      {
        previousNextSelector.SetValueWithoutNotify(previousNextSelector.Values[selectedChoice]);
      }
    }
  }
}
