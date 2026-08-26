using System;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [AddTooltips]
  [HeaderInfo("A profile that holds string to generic data relations. " +
              "Internally mapped to a dictionary so duplicate keys will be ignored but not prevented.")]
  public class StringToGenericDataRelationProfile<T> : ScriptableObject
  {
    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("The list of string to data relations.")]
    protected List<StringToGenericDataRelation<T>> relations = new List<StringToGenericDataRelation<T>>(1);

    protected Dictionary<string, T> map = new Dictionary<string, T>();

    protected Dictionary<string, T> Map
    {
      get
      {
        // TODO Should a refresh also happen in other scenarios?
        if (map.Count != relations.Count)
        {
          RefreshDictionary();
        }
        return map;
      }
      set
      {
        map = value;
      }
    }

    [SkipObfuscationRename]
    public void RefreshDictionary()
    {
      map.Clear();
      StringToGenericDataRelation<T> relation = null;
      for (int i = 0; i < relations.Count; i++)
      {
        relation = relations[i];
        if (relation != null)
        {
          if (map.ContainsKey(relation.Key))
          {
            map[relation.Key] = relation.Value;
          }
          else
          {
            map.Add(relation.Key, relation.Value);
          }
        }
      }
    }

    // TODO Change all usage of this to TryGetValue
    // to make it more secure cause default might not always be the desired value
    public T GetValue(string key)
    {
      if (string.IsNullOrEmpty(key)) { return default; }
      if (Map.TryGetValue(key, out T value))
      {
        return value;
      }
      return default;
    }

    public bool TryGetValue(string key, out T value)
    {
      if (!string.IsNullOrEmpty(key) &&
           Map.TryGetValue(key, out value))
      {
        return true;
      }
      value = default;
      return false;
    }

    public void AddRelation(StringToGenericDataRelation<T> relation)
    {
      if (relations.AddIfNotContains(relation))
      {
        RefreshDictionary();
      }
    }

    public void SetRelations(List<StringToGenericDataRelation<T>> relations)
    {
      this.relations = relations;
      RefreshDictionary();
    }

    public List<Tuple<string, T>> GetRelations()
    {
      List<Tuple<string, T>> relations = new List<Tuple<string, T>>();
      foreach (var r in this.relations)
      {
        relations.Add(new Tuple<string, T>(r.Key, r.Value));
      }
      return relations;
    }
  }
}