using CitrioN.Common;
using System.ComponentModel;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Width (Size)")]
  public class StyleProfileData_UIT_Size_Width : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Height (Size)")]
  public class StyleProfileData_UIT_Size_Height : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Min Width (Size)")]
  public class StyleProfileData_UIT_Size_MinWidth : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Min Height (Size)")]
  public class StyleProfileData_UIT_Size_MinHeight : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Max Width (Size)")]
  public class StyleProfileData_UIT_Size_MaxWidth : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1016)]
  [MenuPath("UI Toolkit/Size")]
  [DisplayName("Max Height (Size)")]
  public class StyleProfileData_UIT_Size_MaxHeight : StyleProfileData_UIT_StyleLength_AutoInitial { }
}