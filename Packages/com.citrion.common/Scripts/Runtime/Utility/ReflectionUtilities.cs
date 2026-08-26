using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  public static class ReflectionUtilities
  {
    [SkipObfuscationRename]
    public static bool IsConstantOrReadOnly(this FieldInfo field)
    {
      if (field == null) { return false; }
      return field.IsLiteral || field.IsInitOnly;
    }

    [SkipObfuscationRename]
    public static FieldInfo[] GetPublicAndNonPublicFields(Type type)
    {
      //return type.GetFields();
      return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance);
    }

    [SkipObfuscationRename]
    public static IEnumerable<FieldInfo> GetSerializableFields(Type type)
    {
      //return type.GetFields();
      return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance).Where(f => f.IsPublic ||
                                                         f.IsDefined(typeof(SerializeField)));
    }
  }
}