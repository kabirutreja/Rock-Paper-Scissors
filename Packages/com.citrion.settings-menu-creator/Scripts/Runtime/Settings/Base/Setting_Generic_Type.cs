using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [MenuOrder(190)]
  [MenuPath("Types")]
  [ExcludeFromMenuSelection]
  [System.Serializable]
  public abstract class Setting_Generic_Type<T> : Setting_Generic<T>
  {
    public override string EditorNamePrefix => "[Type]";
  }
}

