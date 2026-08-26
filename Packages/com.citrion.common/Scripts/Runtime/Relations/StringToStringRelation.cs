using UnityEngine;

namespace CitrioN.Common
{
  [System.Serializable]
  [Common.Serializable]
  [SkipObfuscationRename]
  public class StringToStringRelation : ClassWithDescription
  {
    [SerializeField]
    [SkipObfuscationRename]
    private string key;
    [SerializeField]
    [SkipObfuscationRename]
    private string value;

    public StringToStringRelation() { }

    public StringToStringRelation(string key, string value)
    {
      this.key = key;
      this.value = value;
    }

    public string Key { get => key; set => key = value; }
    public string Value { get => value; set => this.value = value; }

    public override string Description
      => !string.IsNullOrEmpty(key) ?
         $"{key} {(string.IsNullOrEmpty(value) ? "(n/a)" : "(" + value + ")")}" :
         string.Empty;
  }
}