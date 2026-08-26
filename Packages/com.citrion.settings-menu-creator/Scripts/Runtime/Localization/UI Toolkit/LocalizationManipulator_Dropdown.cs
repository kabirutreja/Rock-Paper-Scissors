using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public class LocalizationManipulator_Dropdown : LocalizationManipulator
  {
    public static Dictionary<DropdownField, LocalizationManipulator_Dropdown> localizers = new();

    [SerializeField]
    [Tooltip("The dropdown to localize.")]
    protected DropdownField dropdown;

    [SerializeField]
    [Tooltip("The localization keys for the dropdown's options.")]
    protected List<string> optionKeys = new List<string>();

    public List<string> OptionKeys { get => optionKeys; set => optionKeys = value; }

    public LocalizationManipulator_Dropdown(LocalizationProvider provider) : 
      base(provider) { }

    public DropdownField Dropdown
    {
      get
      {
        TryCacheDropdown();
        return dropdown;
      }
      set => dropdown = value;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      localizers.Clear();
    }

    private void TryCacheDropdown()
    {
      if (dropdown != null) { return; }
      dropdown = target.Q<DropdownField>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || Dropdown == null ||
          optionKeys == null || optionKeys.Count < 1) { return; }

      var localizedOptions = localizationProvider.Localize(optionKeys);

      var selectedChoice = dropdown.choices.IndexOf(dropdown.value);
      dropdown.choices = localizedOptions;
      
      if (selectedChoice >= 0 && selectedChoice < dropdown.choices.Count)
      {
        dropdown.SetValueWithoutNotify(dropdown.choices[selectedChoice]);
      }
    }
  }
}
