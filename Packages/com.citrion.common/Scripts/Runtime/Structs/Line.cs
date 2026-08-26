using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  public struct Line
  {
    [SkipObfuscationRename]
    public Vector3 start;
    [SkipObfuscationRename]
    public Vector3 end;

    public Line(Vector3 start, Vector3 end)
    {
      this.start = start;
      this.end = end;
    }

    public void SetStartEnd(Vector3 start, Vector3 end)
    {
      this.start = start;
      this.end = end;
    }

    public float Length => Vector3.Distance(start, end);
    public Vector3 Direction => end - start;
  }
}