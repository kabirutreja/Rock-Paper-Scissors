namespace CitrioN.Common
{
  [System.Serializable]
  [ClassDescription(nameof(Description), DescriptionResolveMethod.Property)]
  public class ClassWithDescription
  {
    public ClassWithDescription() { }

    public virtual string Description
    {
      get { return GetType().Name; }
    }
  }
}