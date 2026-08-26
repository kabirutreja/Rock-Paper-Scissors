using CitrioN.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [ExcludeFromMenuSelection]
  public class StyleProfileData_UIT_StyleFloat_Initial : StyleProfileData
  {
    [SerializeField]
    protected float value;
    [SerializeField]
    protected StyleKeyword_Initial keyword = StyleKeyword_Initial.None;

    public StyleProfileData_UIT_StyleFloat_Initial() : base() { }

    public override object GetValue()
    {
      if (keyword == StyleKeyword_Initial.Initial)
      {
        return new StyleFloat(StyleKeyword.Initial);
      }

      return new StyleFloat(value);
    }
  }
}