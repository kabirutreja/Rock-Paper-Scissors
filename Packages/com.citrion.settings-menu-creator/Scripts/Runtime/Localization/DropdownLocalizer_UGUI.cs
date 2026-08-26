using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddComponentMenu("CitrioN/Localization/Dropdown Localizer (UGUI)")]
  public class DropdownLocalizer_UGUI : LocalizationBehaviour
  {
    [SerializeField]
    [Tooltip("The Text Mesh Pro dropdown component to localize.")]
    protected TMP_Dropdown dropdown;

    [SerializeField]
    [Tooltip("The localization keys for the dropdown's options.")]
    protected List<string> optionKeys = new List<string>();

    public List<string> OptionKeys { get => optionKeys; set => optionKeys = value; }

    public TMP_Dropdown Dropdown
    {
      get
      {
        TryCacheDropdown();
        return dropdown;
      }
      set => dropdown = value;
    }

    private void Reset()
    {
      TryCacheDropdown();
    }

    private void TryCacheDropdown()
    {
      if (dropdown != null) { return; }
      dropdown = GetComponent<TMP_Dropdown>();
    }

    public override void Localize()
    {
      if (localizationProvider == null || Dropdown == null || 
          optionKeys == null || optionKeys.Count < 1) { return; }

      var localizedOptions = localizationProvider.Localize(optionKeys);


      for (var i = 0; i < dropdown.options.Count; i++)
      {
        if (localizedOptions.Count > i)
        {
          dropdown.options[i].text = localizedOptions[i];
        }
      }
      dropdown.RefreshShownValue();
      //dropdown.ClearOptions();
      //dropdown.AddOptions(localizedOptions);

      //dropdown.SetValueWithoutNotify(value);
      //dropdown.RefreshShownValue();
    }
  }
}
