using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class StyleProfileData_UIT_StyleFont : StyleProfileData
  {
    [SerializeField]
    protected Font value;

    public StyleProfileData_UIT_StyleFont() : base() { }

    public override object GetValue()
    {
      //return new StyleFont(value);
      return new StyleFontDefinition(value);
    }
  }
}