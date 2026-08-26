using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class StyleProfileData_UIT_StyleLength_AutoInitial : StyleProfileData
  {
    [SerializeField]
    protected float value;
    [SerializeField]
    protected LengthUnit lengthUnit;
    [SerializeField]
    protected StyleKeyword_AutoInitial keyword = StyleKeyword_AutoInitial.None;

    public StyleProfileData_UIT_StyleLength_AutoInitial() : base() { }

    public override object GetValue()
    {
      if (keyword == StyleKeyword_AutoInitial.Auto)
      {
        return new StyleLength(StyleKeyword.Auto);
      }
      if (keyword == StyleKeyword_AutoInitial.Initial)
      {
        return new StyleLength(StyleKeyword.Initial);
      }

      return new StyleLength(new Length(value, lengthUnit));
    }
  }
}