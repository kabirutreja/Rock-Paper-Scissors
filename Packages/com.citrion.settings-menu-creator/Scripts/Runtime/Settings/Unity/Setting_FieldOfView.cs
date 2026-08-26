using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public class Setting_FieldOfView : Setting_Camera<float>
  {
    public Setting_FieldOfView()
    {
      options.AddMinMaxRangeValues("50", "100");
      options.AddStepSize("1");

      defaultValue = 60;
    }

    public override string PropertyName => nameof(Camera.fieldOfView);
  } 
}