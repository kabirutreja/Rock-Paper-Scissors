using System;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public class ButtonAttribute :
#if ODIN_INSPECTOR
    Sirenix.OdinInspector.ButtonAttribute
#else
    Attribute
#endif
  {

  }
}