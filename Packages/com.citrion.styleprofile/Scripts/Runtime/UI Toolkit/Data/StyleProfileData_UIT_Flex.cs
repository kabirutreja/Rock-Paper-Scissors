using CitrioN.Common;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1018)]
  [MenuPath("UI Toolkit/Flex")]
  [DisplayName("Basis (Flex)")]
  public class StyleProfileData_UIT_Flex_Basis : StyleProfileData_UIT_StyleLength_AutoInitial { }

  [System.Serializable]
  [MenuOrder(1018)]
  [MenuPath("UI Toolkit/Flex")]
  [DisplayName("Shrink (Flex)")]
  public class StyleProfileData_UIT_Flex_Shrink : StyleProfileData_UIT_StyleFloat_Initial { }

  [System.Serializable]
  [MenuOrder(1018)]
  [MenuPath("UI Toolkit/Flex")]
  [DisplayName("Grow (Flex)")]
  public class StyleProfileData_UIT_Flex_Grow : StyleProfileData_UIT_StyleFloat_Initial { }

  [System.Serializable]
  [MenuOrder(1018)]
  [MenuPath("UI Toolkit/Flex")]
  [DisplayName("Direction (Flex)")]
  public class StyleProfileData_UIT_Flex_Direction : GenericStyleProfileData<FlexDirection> { }

  [System.Serializable]
  [MenuOrder(1018)]
  [MenuPath("UI Toolkit/Flex")]
  [DisplayName("Wrap (Flex)")]
  public class StyleProfileData_UIT_Flex_Wrap : GenericStyleProfileData<Wrap> { }
}