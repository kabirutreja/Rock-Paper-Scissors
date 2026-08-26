using CitrioN.Common;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image (Background)")]
  public class StyleProfileData_UIT_Background_Image : StyleProfileData
  {
    [SerializeField]
    protected Texture2D texture;

    [SerializeField]
    protected Sprite sprite;

    [SerializeField]
    protected VectorImage vectorImage;

    [SerializeField]
    protected RenderTexture renderTexture;

    public StyleProfileData_UIT_Background_Image() : base() { }

    public override object GetValue()
    {
      if (texture != null)
      {
        return new StyleBackground(texture);
      }
      if (sprite != null)
      {
        return new StyleBackground(sprite);
      }
      if (vectorImage != null)
      {
        return new StyleBackground(vectorImage);
      }
      if (renderTexture != null)
      {
        var background = new Background();
        background.renderTexture = renderTexture;
        return new StyleBackground(background);
      }

      return new StyleBackground();
    }
  }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Scale Mode (Background)")]
  public class StyleProfileData_UIT_Background_Image_Scale_Mode : GenericStyleProfileData<ScaleMode> { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice All (Background)")]
  public class StyleProfileData_UIT_Background_Slice_All : StyleProfileData_UIT_StyleInt_Initial { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice Left (Background)")]
  public class StyleProfileData_UIT_Background_Slice_Left : StyleProfileData_UIT_StyleInt_Initial { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice Top (Background)")]
  public class StyleProfileData_UIT_Background_Slice_Top : StyleProfileData_UIT_StyleInt_Initial { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice Right (Background)")]
  public class StyleProfileData_UIT_Background_Slice_Right : StyleProfileData_UIT_StyleInt_Initial { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice Bottom (Background)")]
  public class StyleProfileData_UIT_Background_Slice_Bottom : StyleProfileData_UIT_StyleInt_Initial { }

  [System.Serializable]
  [MenuOrder(1012)]
  [MenuPath("UI Toolkit/Background")]
  [DisplayName("Image Slice Scale (Background)")]
  public class StyleProfileData_UIT_Background_Slice_Scale : StyleProfileData_UIT_StyleFloat_Initial { } 
}