using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class StyleProfileData_UIT_StyleFontDefinition : StyleProfileData
  {
    [SerializeField]
    protected FontDefinition value;

    public StyleProfileData_UIT_StyleFontDefinition() : base() { }

    public override object GetValue()
    {
      return new StyleFontDefinition(value);
    }
  }
}