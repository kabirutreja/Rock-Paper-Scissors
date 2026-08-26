using UnityEngine;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  public enum StyleType
  {
    None,

    #region Display
    [InspectorName("Display/Opacity")]
    Display_Opacity,              // int
    [InspectorName("Display/Display")]
    Display_Display,              // string
    [InspectorName("Display/Visibility")]
    Display_Visibility,           // string
    [InspectorName("Display/Overflow")]
    Display_Overflow,             // string 
    #endregion

    #region Position
    [InspectorName("Position/Position")]
    Position_Position,            // string
    [InspectorName("Position/Left")]
    Position_Left,                // string
    [InspectorName("Position/Top")]
    Position_Top,                 // string
    [InspectorName("Position/Right")]
    Position_Right,               // string
    [InspectorName("Position/Bottom")]
    Position_Bottom,              // string 
    #endregion

    #region Flex
    [InspectorName("Flex/Basis")]
    Flex_Basis,                   // string
    [InspectorName("Flex/Shrink")]
    Flex_Shrink,                  // string
    [InspectorName("Flex/Grow")]
    Flex_Grow,                    // string
    [InspectorName("Flex/Direction")]
    Flex_Direction,               // string
    [InspectorName("Flex/Wrap")]
    Flex_Wrap,                    // string 
    #endregion

    #region Align
    [InspectorName("Align/Self")]
    Align_Self,                   // string 
    [InspectorName("Align/Items")]
    Align_Items,                  // string
    [InspectorName("Align/Justify Content")]
    Justify_Content,              // string
    #endregion

    #region Size
    [InspectorName("Size/Width")]
    Size_Width,                   // string
    [InspectorName("Size/Min Width")]
    Size_Width_Min,               // string
    [InspectorName("Size/Max Width")]
    Size_Width_Max,               // string
    [InspectorName("Size/Height")]
    Size_Height,                  // string
    [InspectorName("Size/Min Height")]
    Size_Height_Min,              // string
    [InspectorName("Size/Max Height")]
    Size_Height_Max,              // string 
    #endregion

    #region Margin & Padding
    [InspectorName("Margin & Padding/Margin/All")]
    Margin_All,                   // string
    [InspectorName("Margin & Padding/Margin/Left")]
    Margin_Left,                  // string
    [InspectorName("Margin & Padding/Margin/Right")]
    Margin_Right,                 // string
    [InspectorName("Margin & Padding/Margin/Top")]
    Margin_Top,                   // string
    [InspectorName("Margin & Padding/Margin/Bottom")]
    Margin_Bottom,                // string

    [InspectorName("Margin & Padding/Padding/All")]
    Padding_All,                   // string
    [InspectorName("Margin & Padding/Padding/Left")]
    Padding_Left,                  // string
    [InspectorName("Margin & Padding/Padding/Right")]
    Padding_Right,                 // string
    [InspectorName("Margin & Padding/Padding/Top")]
    Padding_Top,                   // string
    [InspectorName("Margin & Padding/Padding/Bottom")]
    Padding_Bottom,                // string 
    #endregion

    #region Text
    [InspectorName("Text/Font")]
    Text_Font,                       // Font
    [InspectorName("Text/Font Asset")]
    Text_FontAsset,                  // Font Asset
    [InspectorName("Text/Style")]
    Text_Style,                      //
    [InspectorName("Text/Size")]
    Text_Size,                       //
    [InspectorName("Text/Color")]
    Text_Color,                      // Color
    [InspectorName("Text/Align")]
    Text_Align,                      //
    [InspectorName("Text/Wrap")]
    Text_Wrap,                       //
    [InspectorName("Text/Overflow")]
    Text_Overflow,                   //
    [InspectorName("Text/Outline/Width")]
    Text_Outline_Width,              //
    [InspectorName("Text/Outline/Color")]
    Text_Outline_Color,              // Color
    [InspectorName("Text/Shadow/Horizontal Offset")]
    Text_Shadow_Offset_Horizontal,   //
    [InspectorName("Text/Shadow/Vertical Offset")]
    Text_Shadow_Offset_Vertical,     //
    [InspectorName("Text/Shadow/Blur Radius")]
    Text_Shadow_Blur_Radius,         //
    [InspectorName("Text/Shadow/Color")]
    Text_Shadow_Color,               // Color
    [InspectorName("Text/Spacing/Letter")]
    Text_Spacing_Letter,             //
    [InspectorName("Text/Spacing/Word")]
    Text_Spacing_Word,               //
    [InspectorName("Text/Spacing/Paragraph")]
    Text_Spacing_Paragraph,          // 
    #endregion

    #region Background
    [InspectorName("Background/Color")]
    Background_Color,                // Color
    [InspectorName("Background/Image")]
    Background_Image,                //
    [InspectorName("Background/Image Tint")]
    Background_Image_Tint,           // Color
    [InspectorName("Background/Scale Mode")]
    Background_Scale_Mode,           //
    [InspectorName("Background/Slice/All")]
    Background_Slice_All,            //
    [InspectorName("Background/Slice/Left")]
    Background_Slice_Left,           //
    [InspectorName("Background/Slice/Top")]
    Background_Slice_Top,            //
    [InspectorName("Background/Slice/Right")]
    Background_Slice_Right,          //
    [InspectorName("Background/Slice/Bottom")]
    Background_Slice_Bottom,         //
    [InspectorName("Background/Slice/Scale")]
    Background_Slice_Scale,          //   
    #endregion

    #region Border
    [InspectorName("Border/Color/All")]
    Border_Color_All,                // Color
    [InspectorName("Border/Color/Top")]
    Border_Color_Top,                // Color
    [InspectorName("Border/Color/Right")]
    Border_Color_Right,              // Color
    [InspectorName("Border/Color/Bottom")]
    Border_Color_Bottom,             // Color
    [InspectorName("Border/Color/Left")]
    Border_Color_Left,               // Color
    [InspectorName("Border/Width/All")]
    Border_Width_All,                //
    [InspectorName("Border/Width/Top")]
    Border_Width_Top,                //
    [InspectorName("Border/Width/Right")]
    Border_Width_Right,              //
    [InspectorName("Border/Width/Bottom")]
    Border_Width_Bottom,             //
    [InspectorName("Border/Width/Left")]
    Border_Width_Left,               //
    [InspectorName("Border/Radius/All")]
    Border_Radius_All,               //
    [InspectorName("Border/Radius/Top Left")]
    Border_Radius_Top_Left,          //
    [InspectorName("Border/Radius/Top Right")]
    Border_Radius_Top_Right,         //
    [InspectorName("Border/Radius/Bottom Right")]
    Border_Radius_Bottom_Right,      //
    [InspectorName("Border/Radius/Bottom Left")]
    Border_Radius_Bottom_Left,       // 
    #endregion

    // TODO Transform

    #region Cursor
    [InspectorName("Cursor/Image")]
    Cursor_Image,                    // Texture2D 
    #endregion

    // TODO Transition Animations
  }
}
