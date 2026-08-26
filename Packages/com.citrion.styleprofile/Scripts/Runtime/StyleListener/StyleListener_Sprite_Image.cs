using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.StyleProfileSystem
{
  [HeaderInfo("\n\nChanges the 'Sprite' of the referenced image component.")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (Image - Sprite)")]
  public class StyleListener_Sprite_Image : StyleListener_Sprite
  {
    [Header("Image Sprite")]
    [Space(10)]

    [SerializeField]
    [Tooltip("Reference to the image script for which to change the Sprite.")]
    protected Image image;

    protected void Reset()
    {
      CacheImage();
    }

    protected override void Awake()
    {
      base.Awake();
      CacheImage();
    }

    private void CacheImage()
    {
      if (image == null)
      {
        image = GetComponent<Image>();
      }
    }

    protected override void ApplyChange(Sprite sprite)
    {
      base.ApplyChange(sprite);
      if (image != null)
      {
        image.sprite = sprite;
      }
    }
  } 
}