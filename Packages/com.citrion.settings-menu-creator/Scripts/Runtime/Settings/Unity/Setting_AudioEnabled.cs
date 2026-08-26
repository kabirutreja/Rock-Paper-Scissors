using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(900)]
  [MenuPath("Audio")]
  [System.Serializable]
  public class Setting_AudioEnabled : Setting_Generic_Unity<bool>
  {
    public override bool StoreValueInternally => true;

    public override string EditorNamePrefix => "[Audio]";

    protected override object ApplySettingChangeWithValue(SettingsCollection settings, bool value)
    {
      AudioListener.pause = !value;
      return !AudioListener.pause;
    }

    public override List<object> GetCurrentValues(SettingsCollection settings)
    {
      return new List<object> { !AudioListener.pause };
    }
  } 
}