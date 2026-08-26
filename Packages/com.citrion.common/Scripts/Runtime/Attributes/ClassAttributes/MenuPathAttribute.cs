namespace CitrioN.Common
{
  [System.AttributeUsage(System.AttributeTargets.Class |
                         System.AttributeTargets.Method)]
  [SkipObfuscationRename]
  public class MenuPathAttribute : System.Attribute
  {
    public string Path { get; private set; } = string.Empty;

    public MenuPathAttribute(string path)
    {
      Path = path;
    }
  }
}