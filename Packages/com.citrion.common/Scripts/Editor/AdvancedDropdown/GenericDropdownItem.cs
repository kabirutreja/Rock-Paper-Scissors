using UnityEditor.IMGUI.Controls;

namespace CitrioN.Common.Editor
{
  public class GenericDropdownItem<T> : AdvancedDropdownItem
  {
    public T value { get; private set; }

    public GenericDropdownItem(string name, T value) : base(name)
    {
      this.value = value;
    }
  }
}
