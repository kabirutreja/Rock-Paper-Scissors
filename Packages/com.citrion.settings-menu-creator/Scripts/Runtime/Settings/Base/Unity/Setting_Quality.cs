using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [MenuOrder(1200)]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Quality<T> : Setting_Generic_Reflection_Property_Unity_Static<T, QualitySettings>
  {
    public override string EditorNamePrefix => "[Quality]";
  }
}