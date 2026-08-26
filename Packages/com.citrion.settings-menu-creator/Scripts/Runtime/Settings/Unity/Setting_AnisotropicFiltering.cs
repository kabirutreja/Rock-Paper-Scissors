using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [MenuPath("Quality/General/")]
  public class Setting_AnisotropicFiltering : Setting_Quality<AnisotropicFiltering>
  {
    public override string PropertyName => nameof(QualitySettings.anisotropicFiltering);

    public Setting_AnisotropicFiltering()
    {
      options.Clear();
      options.Add(new StringToStringRelation("Disable", "Disabled"));
      options.Add(new StringToStringRelation("Enable", "Per Texture"));
      options.Add(new StringToStringRelation("ForceEnable", "Forced On"));

      defaultValue = AnisotropicFiltering.ForceEnable;
    }
  } 
}