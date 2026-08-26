using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [System.Serializable]
  public class StringToGenericDataRelation<T> : ClassWithDescription
  {
    [SerializeField]
    [SkipObfuscationRename]
    protected string key;
    [SerializeField]
    [SerializeReference]
    [SkipObfuscationRename]
    protected T value;

    public StringToGenericDataRelation() { }

    public StringToGenericDataRelation(string key, T value)
    {
      this.key = key;
      this.value = value;
    }

    public string Key => key;
    public T Value => value;

    public override string Description => !string.IsNullOrEmpty(Key) ? Key : string.Empty;
  }
}