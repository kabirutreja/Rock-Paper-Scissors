using UnityEngine;

namespace CitrioN.Common
{
  [System.Serializable]
  [SkipObfuscationRename]
  public struct Plane
  {
    [SkipObfuscationRename]
    public Vector3 center;// = Vector3.zero;
    [SkipObfuscationRename]
    public Vector3 normal;// = Vector3.up;
    [SkipObfuscationRename]
    public Vector3 xDirection;// = Vector3.right;
    [SkipObfuscationRename]
    public Vector3 yDirection;// = Vector3.up;
    [SkipObfuscationRename]
    public Vector2 dimensions;// = Vector2.one;

    public Plane(Vector3 center, Vector3 normal, Vector3 xDirection, Vector3 yDirection, Vector2 dimensions)
    {
      this.center = center;
      this.normal = normal;
      this.xDirection = xDirection;
      this.yDirection = yDirection;
      this.dimensions = dimensions;
    }
  }
}