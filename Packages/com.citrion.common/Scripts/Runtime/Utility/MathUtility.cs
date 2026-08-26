using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Holds useful methods for math related functionality
  /// </summary>
  public static class MathUtility
  {
    /// <summary>
    /// Snaps a value to the next multiple of the snapIncrement.
    /// Works for negative numbers.
    /// 
    /// Examples:
    /// A value of 26 with a snapIncrement of 50 will return 50.
    /// A value of 24 with a snapIncrement of 50 will return 0.
    /// </summary>
    /// <param name="value">The value to snap to the next increment of the snap value</param>
    /// <param name="snapIncrement">The increment of which to snap the value to</param>
    /// <returns>The value snapped to an increment of the provided snapIncrement</returns>
    public static int SnapTo(int value, int snapIncrement)
    {
      if (snapIncrement == 0) { return value; }
      int remainder = Mathf.Abs(value) % snapIncrement;
      //if (remainder >= (snapIncrement / 2f))
      //{
      //  // Round up
      //  if (value >= 0)
      //  {
      //    return value + (snapIncrement - remainder);
      //  }
      //  else
      //  {
      //    return value - (snapIncrement - remainder);
      //  }
      //}
      //else
      //{
      //  // Round down
      //  if (value >= 0)
      //  {
      //    return value - remainder;
      //  }
      //  else
      //  {
      //    return value + remainder;
      //  }
      //}
      return remainder >= (snapIncrement / 2f) ?
             value + (value >= 0 ? (snapIncrement - remainder) : -(snapIncrement - remainder)) :
             value - (value >= 0 ? remainder : -remainder);
    }

    public static float SnapTo(float value, float snapIncrement, float origin = 0)
    {
      if (snapIncrement == 0) { return value; }
      value -= origin;
      float remainder = Mathf.Abs(value) % snapIncrement;
      float snappedValue = remainder >= (snapIncrement / 2f) ?
            value + (value >= 0 ? (snapIncrement - remainder) : -(snapIncrement - remainder)) :
            value - (value >= 0 ? remainder : -remainder);
      //Debug.Log($"Origin: {origin} - Value {snappedValue}");
      return snappedValue + origin;
    }

    /// <summary>
    /// Rounds to the next larger/smaller multiple of the snapIncrement.
    /// Example:
    /// 
    /// value: 0.26f
    /// snapIncrement: 0.25f
    /// 
    /// This will snap the value to 0.5 when using larger or 0.25 when using smaller
    /// because it is the next multiple of 0.25 that is larger/smaller than the value
    /// </summary>
    /// <param name="value">The value to snap</param>
    /// <param name="snapIncrement">The snap increment to snap to</param>
    /// <param name="larger">Should the value snap to the next larger or smaller increment of snapIncrement?</param>
    /// <returns>The value snapped to the next multiple of the snap increment larger/smaller than value</returns>
    public static float SnapToNext(float value, float snapIncrement, bool larger, float origin = 0)
    {
      if (snapIncrement == 0) { return value; }
      float remainder = Mathf.Abs(value) % snapIncrement;
      return origin + value - remainder + snapIncrement;
    }

    public static int ClampLowerTo0(this int value)
    {
      return value < 0 ? value = 0 : value;
    }

    public static float ClampLowerTo0(this float value)
    {
      return value < 0 ? value = 0f : value;
    }

    public static bool PointInsideRectangle(this Vector3 point, Vector3 rectangleCenter, Vector3 right, Vector3 up, Vector2 dimensions)
    {
      //var bottomLeft = rectangleCenter - (right * dimensions.x / 2) - (up * dimensions.y / 2);

      var direction = point - rectangleCenter;
      float projectedX = Mathf.Abs(Vector3.Dot(direction, right));
      float projectedY = Mathf.Abs(Vector3.Dot(direction, up));
      //float projectedZ = Mathf.Abs(Vector3.Dot(direction, forward));

      if (2 * projectedX <= dimensions.x &&
          2 * projectedY <= dimensions.y/* &&*/
          /*2 * projectedZ <= bounds.size.z*/)
      {
        return true;
      }
      return false;
    }

    public static bool LinePlaneIntersectionPoint(this Line line, Vector3 planePos,
                                                  Vector3 planeNormal, out Vector3 intersectionPoint)
    {
      //Vector3 direction = line.end - line.start;
      Vector3 direction = line.Direction;

      // Project the direction onto the plane normal
      float projection = Vector3.Dot(direction, planeNormal);

      // Check if the projection is not parallel and not facing away from the plane.
      // A parallel or away facing vector would never intersect with the plane
      // TODO Test if this works with > 0 too
      if (projection > 0.000001f)
      {
        // Project the direction from the line start to the plane center onto the plane normal
        float projection2 = Vector3.Dot(planePos - line.start, planeNormal);

        // Divide the second project by the first projection to the projected distance between
        // the line start and the plane position
        float projectedLength = projection2 / projection;

        // Calculate the intersection point
        intersectionPoint = line.start + direction * projectedLength;

        // Check if the intersection point is further away than the lines' length
        if (Vector3.Distance(line.start, intersectionPoint) > line.Length)
        {
          return false;
        }
        return true;
      }
      intersectionPoint = Vector3.zero;
      return false;
    }

    public static float DistanceToLine(Line line, Vector3 point)
    {
      // TODO Finish Implementation

      //var projectedPoint = new Vector3(point.x - line.start.x, point.y - line.start.y, point.z - line.start.z);
      //var projectedEnd = new Vector3(line.end.x - line.start.x, line.end.y - line.start.y, line.end.z - line.start.z);

      //var squaredMagnitude = projectedEnd.sqrMagnitude;

      //var dotProduct = Vector3.Dot(projectedPoint, projectedEnd);

      //var t = dotProduct / squaredMagnitude;

      //var closestPoint = new Vector3(line.start.x + projectedEnd.x * t,
      //                               line.start.y + projectedEnd.y * t,
      //                               line.start.z + projectedEnd.z * t);

      //return Vector3.Distance(closestPoint, point);
      return 1;
    }

    /// <summary>
    /// Calculates the normalized progress values of a provided value between a minimum and maximum value.
    /// </summary>
    /// <returns>The normalized progress value of the provided value between min and max. Between 0 and 1.</returns>
    public static float GetNormalizedProgress(float minValue, float maxValue, float value)
    {
      var valueRange = maxValue - minValue;
      var relativeValueProgress = value - minValue;
      return relativeValueProgress / valueRange;
    }

    public static int GetDecimals(float value)
    {
      var decimals = 0;
      value = Mathf.Abs(value);
      decimal d = (decimal)(value % 1);

      while (d > 0)
      {
        d *= 10;
        d -= (int)d;
        decimals++;
      }

      return decimals;
    }
  }
}