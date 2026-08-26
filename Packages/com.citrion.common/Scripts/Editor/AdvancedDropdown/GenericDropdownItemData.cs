namespace CitrioN.Common.Editor
{
  [System.Serializable]
  public class GenericDropdownItemData<T>
  {
    public T value;
    public string displayName;
    public string path;
    public int order;

    public GenericDropdownItemData(T value, string displayName, string path = null, int order = -1)
    {
      this.value = value;
      this.displayName = displayName;
      this.path = path;
      this.order = order;
    }
  }
}