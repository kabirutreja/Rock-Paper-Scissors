using System;

namespace CitrioN.Common
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct |
                  AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Parameter |
                  AttributeTargets.Event | AttributeTargets.Enum | AttributeTargets.Property |
                  AttributeTargets.Delegate)]
  [SkipObfuscationRename]
  public class SkipObfuscationAttribute : System.Attribute
  {
    public SkipObfuscationAttribute() { }
  }
}