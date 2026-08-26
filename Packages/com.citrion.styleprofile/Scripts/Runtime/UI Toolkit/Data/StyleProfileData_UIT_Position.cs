using CitrioN.Common;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1019)]
  [MenuPath("UI Toolkit/Position")]
  [DisplayName("Position (Position)")]
  public class StyleProfileData_UIT_Position : GenericStyleProfileData<Position> { }

  [System.Serializable]
  [MenuOrder(1019)]
  [MenuPath("UI Toolkit/Position")]
  [DisplayName("Left (Position)")]
  public class StyleProfileData_UIT_Position_Left : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1019)]
  [MenuPath("UI Toolkit/Position")]
  [DisplayName("Top (Position)")]
  public class StyleProfileData_UIT_Position_Top : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1019)]
  [MenuPath("UI Toolkit/Position")]
  [DisplayName("Right (Position)")]
  public class StyleProfileData_UIT_Position_Right : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1019)]
  [MenuPath("UI Toolkit/Position")]
  [DisplayName("Bottom (Position)")]
  public class StyleProfileData_UIT_Position_Bottom : StyleProfileData_UIT_StyleLength_AutoInitial { }
}