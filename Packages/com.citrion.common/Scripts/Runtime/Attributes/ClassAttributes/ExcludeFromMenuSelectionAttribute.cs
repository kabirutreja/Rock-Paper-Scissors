namespace CitrioN.Common
{
  [System.AttributeUsage(System.AttributeTargets.Class)]
  [SkipObfuscation]
  public class ExcludeFromMenuSelectionAttribute : System.Attribute
  {
    public ExcludeFromMenuSelectionAttribute() { }
  }
}