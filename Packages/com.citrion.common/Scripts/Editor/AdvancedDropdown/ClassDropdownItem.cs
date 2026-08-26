using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public class ClassDropdownItem : AdvancedDropdownItem
  {
    public Type Type { get; private set; }

    public ClassDropdownItem(string name, Type type) : base(name)
    {
      Type = type;
    }

    public ClassDropdownItem(string name, Type type, Texture2D icon) : base(name)
    {
      Type = type;
      this.icon = icon;
    }
  }
}