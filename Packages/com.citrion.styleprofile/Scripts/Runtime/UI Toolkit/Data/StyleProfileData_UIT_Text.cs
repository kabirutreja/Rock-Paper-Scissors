using CitrioN.Common;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Font (Text)")]
  public class StyleProfileData_UIT_Font : StyleProfileData_UIT_StyleFont { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Font Asset (Text)")]
  public class StyleProfileData_UIT_Font_Asset : StyleProfileData_UIT_StyleFontDefinition { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Font Style (Text)")]
  public class StyleProfileData_UIT_Font_Style : GenericStyleProfileData<FontStyle> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Size (Text)")]
  public class StyleProfileData_UIT_Text_Size : StyleProfileData_UIT_StyleLength_Initial { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Align (Text)")]
  public class StyleProfileData_UIT_Text_Align : GenericStyleProfileData<TextAnchor> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Wrap (Text)")]
  public class StyleProfileData_UIT_Text_Wrap : GenericStyleProfileData<WhiteSpace> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Overflow (Text)")]
  public class StyleProfileData_UIT_Text_Overflow : GenericStyleProfileData<TextOverflow> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Outline Width (Text)")]
  public class StyleProfileData_UIT_Text_Outline_Width : StyleProfileData_UIT_StyleFloat_Initial { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Shadow Horizontal Offset (Text)")]
  public class StyleProfileData_UIT_Text_Shadow_Offset_Horizontal : GenericStyleProfileData<float> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Shadow Vertical Offset (Text)")]
  public class StyleProfileData_UIT_Text_Shadow_Offset_Vertical : GenericStyleProfileData<float> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Shadow Blur Radius (Text)")]
  public class StyleProfileData_UIT_Text_Shadow_Blur_Radius : GenericStyleProfileData<float> { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Letter Spacing (Text)")]
  public class StyleProfileData_UIT_Text_Letter_Spacing : StyleProfileData_UIT_StyleLength_Initial { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Word Spacing (Text)")]
  public class StyleProfileData_UIT_Text_Word_Spacing : StyleProfileData_UIT_StyleLength_Initial { }

  [System.Serializable]
  [MenuOrder(1013)]
  [MenuPath("UI Toolkit/Text")]
  [DisplayName("Text Paragraph Spacing (Text)")]
  public class StyleProfileData_UIT_Text_Paragraph_Spacing : StyleProfileData_UIT_StyleLength_Initial { }
}