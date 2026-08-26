using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.Common
{
  /// <summary>
  /// Image extension class
  /// </summary>
  public static class ImageExtensions
  {
    /// <summary>
    /// Sets up the image to be used as a radial progress image
    /// </summary>
    public static void SetupRadialProgressImage(this Image image)
    {
      image.type = Image.Type.Filled;
      image.fillMethod = Image.FillMethod.Radial360;
      image.fillOrigin = (int)Image.Origin360.Top;
      image.fillClockwise = true;
      image.fillAmount = 0f;
    }

    public static void SetSprite(this Image image, Sprite sprite)
    {
      if (image != null)
      {
        image.sprite = sprite;
      }
    }

    /// <summary>
    /// Changes the alpha of the image color to the provided alpha value
    /// </summary>
    public static void SetAlpha(this Image image, float alpha)
    {
      if (image != null)
      {
        var color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
      }
    }

    /// <summary>
    /// Changes the alpha of the image color by the provided alpha value
    /// </summary>
    public static void ChangeAlpha(this Image image, float alphaChange)
    {
      if (image != null)
      {
        var color = image.color;
        color.a = Mathf.Clamp01(color.a + alphaChange);
        image.color = color;
      }
    }
  }
}