using CitrioN.Common;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1020)]
  [MenuPath("UI Toolkit/Display")]
  [DisplayName("Opacity (Display)")]
  public class StyleProfileData_UIT_Display_Opacity : GenericStyleProfileData<float> { }

  [System.Serializable]
  [MenuOrder(1020)]
  [MenuPath("UI Toolkit/Display")]
  [DisplayName("Style (Display)")]
  public class StyleProfileData_UIT_Display_Style : GenericStyleProfileData<DisplayStyle> { }

  [System.Serializable]
  [MenuOrder(1020)]
  [MenuPath("UI Toolkit/Display")]
  [DisplayName("Visibility (Display)")]
  public class StyleProfileData_UIT_Display_Visibility : GenericStyleProfileData<Visibility> { }

  [System.Serializable]
  [MenuOrder(1020)]
  [MenuPath("UI Toolkit/Display")]
  [DisplayName("Overflow (Display)")]
  public class StyleProfileData_UIT_Display_Overflow : GenericStyleProfileData<Overflow> { }
}