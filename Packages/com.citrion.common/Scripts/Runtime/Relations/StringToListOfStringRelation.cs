using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  [System.Serializable]
  public class StringToListOfStringRelation : ClassWithDescription
  {
    [SerializeField]
    private string key;
    [SerializeField]
    private List<string> value = new List<string>();

    public StringToListOfStringRelation() { }

    public StringToListOfStringRelation(string key, List<string> value)
    {
      this.key = key;
      this.value = value;
    }

    public string Key => key;
    public List<string> Value => value;

    public override string Description => !string.IsNullOrEmpty(key) ? key : string.Empty;
  }
}