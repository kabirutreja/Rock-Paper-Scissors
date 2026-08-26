namespace CitrioN.Common
{
  [System.AttributeUsage(System.AttributeTargets.Class)]
  [SkipObfuscationRename]
  public class MenuOrderAttribute : System.Attribute
  {
    public int Order { get; private set; }

    public MenuOrderAttribute(int order)
    {
      Order = order;
    }
  }
}