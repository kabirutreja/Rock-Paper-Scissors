using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Contains extensions methods for vectors
  /// </summary>
  public static class VectorExtensions
  {
    /// <summary>
    /// Returns a random integer between the two values of the vector
    /// </summary>
    public static int GetRandomValue(this Vector2Int minMax)
    {
      return UnityEngine.Random.Range(minMax.x, minMax.y + 1);
    }

    public static float GetRandomValue(this Vector2 minMax)
    {
      return UnityEngine.Random.Range(minMax.x, minMax.y);
    }

    public static Vector3 Absolute(this Vector3 vector)
    {
      return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
  }
}