using UnityEngine;

namespace CitrioN.Common
{
  public static class QuaternionExtensions
  {
    public static Vector3 Right(this Quaternion quaternion)
    {
      return quaternion * Vector3.right;
    }

    public static Vector3 Up(this Quaternion quaternion)
    {
      return quaternion * Vector3.up;
    }

    public static Vector3 Forward(this Quaternion quaternion)
    {
      return quaternion * Vector3.forward;
    }
  }
}