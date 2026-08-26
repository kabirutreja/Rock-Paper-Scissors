using System;
using System.Reflection;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [AttributeUsage(AttributeTargets.Field)]
  public class FieldAttribute : Attribute
  {
    public AttributeType AttributeType { get; protected set; } = AttributeType.Single;

    public static event Action<FieldAttribute, object, FieldInfo> onDraw = null;

    public void Draw(object obj, FieldInfo field)
    {
      onDraw?.Invoke(this, obj, field);
    }
  }
}