using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class StyleProfileData_UIT_StyleLength_Initial : StyleProfileData
  {
    [SerializeField]
    protected float value;
    [SerializeField]
    protected LengthUnit lengthUnit;
    [SerializeField]
    protected StyleKeyword_Initial keyword = StyleKeyword_Initial.None;

    public StyleProfileData_UIT_StyleLength_Initial() : base() { }

    public override object GetValue()
    {
      if (keyword == StyleKeyword_Initial.Initial)
      {
        return new StyleLength(StyleKeyword.Initial);
      }

      return new StyleLength(new Length(value, lengthUnit));
    }
  }
}