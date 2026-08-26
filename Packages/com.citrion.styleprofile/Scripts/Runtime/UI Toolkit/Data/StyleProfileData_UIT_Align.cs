using CitrioN.Common;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1017)]
  [MenuPath("UI Toolkit/Align")]
  [DisplayName("Align Items (Align)")]
  public class StyleProfileData_UIT_Align_Items : GenericStyleProfileData<Align> { }

  [System.Serializable]
  [MenuOrder(1017)]
  [MenuPath("UI Toolkit/Align")]
  [DisplayName("Justify Content (Align)")]
  public class StyleProfileData_UIT_Align_Justify_Content : GenericStyleProfileData<Justify> { }

  [System.Serializable]
  [MenuOrder(1017)]
  [MenuPath("UI Toolkit/Align")]
  [DisplayName("Align Self (Align)")]
  public class StyleProfileData_UIT_Align_Self : GenericStyleProfileData<Align> { }
}