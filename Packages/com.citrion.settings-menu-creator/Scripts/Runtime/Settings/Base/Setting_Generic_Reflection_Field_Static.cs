using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [ExcludeFromMenuSelection]
  [System.Serializable]
  public abstract class Setting_Generic_Reflection_Field_Static<T1, T2> : Setting_Generic_Reflection_Field<T1, T2>
  {
    public override object GetObject(SettingsCollection settings) => null;

    protected override bool IsStatic => true;
  }
}

