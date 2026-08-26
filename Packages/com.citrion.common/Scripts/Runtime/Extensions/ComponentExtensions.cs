using UnityEngine;

namespace CitrioN.Common
{
  public static class ComponentExtensions
  {
    public static void CopyValuesTo<T>(this T sourceComponent, T targetComponent) where T : Component
    {
      foreach (var fieldInfo in typeof(T).GetSerializableFields())
      {
        fieldInfo.SetValue(targetComponent, fieldInfo.GetValue(sourceComponent));
      }
    }
  }
}