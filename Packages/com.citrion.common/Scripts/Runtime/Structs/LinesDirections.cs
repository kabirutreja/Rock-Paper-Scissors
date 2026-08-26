using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  public struct LinesDirections
  {
    [SkipObfuscationRename]
    public Line line;
    [SkipObfuscationRename]
    public CubePlaneDirection direction;
    //public Plane plane;
    [SkipObfuscationRename]
    public Quaternion rotation;
    [SkipObfuscationRename]
    public Bounds bounds;

    public LinesDirections(Line line, CubePlaneDirection direction, Quaternion rotation, Bounds bounds/*, Plane plane*/)
    {
      this.line = line;
      this.direction = direction;
      this.rotation = rotation;
      this.bounds = bounds;
      //this.plane = plane;
    }
  }
}