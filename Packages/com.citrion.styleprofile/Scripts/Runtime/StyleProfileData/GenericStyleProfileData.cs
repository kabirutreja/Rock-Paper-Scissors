using CitrioN.Common;

namespace CitrioN.StyleProfileSystem
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class GenericStyleProfileData<T> : StyleProfileData
  {
    public T value;

    public GenericStyleProfileData() : base() { }

    public GenericStyleProfileData(T value) : this()
    {
      this.value = value;
    }

    public override object GetValue()
    {
      return value;
    }
  }
}