using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Extension methods for the <see cref="Texture2D"/> class.
  /// </summary>
  public static class Texture2DExtensions
  {
    public static Sprite ConvertToSprite(this Texture2D texture2D)
    {
      return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                           new Vector2(0.5f, 0.5f), 100);
    }
  }
}