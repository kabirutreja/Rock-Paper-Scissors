using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Generic_Reflection_Property_Unity_Static<T1, T2> : Setting_Generic_Reflection_Property_Unity<T1, T2>
  {
    public override object GetObject(SettingsCollection settings) => null;

    protected override bool IsStatic => true;
  }
}
