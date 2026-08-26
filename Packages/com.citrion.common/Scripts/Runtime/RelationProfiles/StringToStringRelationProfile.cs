using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("A profile that holds string to string relations. " +
              "Internally mapped to a dictionary so duplicate keys will be ignored but not prevented.")]
  [CreateAssetMenu(fileName = "StringToStringRelationProfile_",
                   menuName = "CitrioN/Common/Profiles/StringToStringRelationProfile")]
  [SkipObfuscationRename]
  public class StringToStringRelationProfile : ScriptableObject
  {
    [SerializeField]
    [SkipObfuscationRename]
    [Tooltip("The list of string to string relations.")]
    protected List<StringToStringRelation> relations = new List<StringToStringRelation>(1);

    protected Dictionary<string, string> map = new Dictionary<string, string>();

    protected Dictionary<string, string> Map
    {
      get
      {
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

    public List<StringToStringRelation> Relations => relations;

    //private void OnEnable()
    //{
    //  if (Application.isPlaying)
    //  {
    //    CoroutineRunner.Instance.InvokeDelayedByFrames(RefreshDictionary);
    //  }
    //}

    [Button]
    private void RefreshDictionary()
    {
      map.Clear();
      StringToStringRelation relation = null;
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

    public string GetValue(string key)
    {
      if (string.IsNullOrEmpty(key)) { return null; }
      //return relations.Find(r => r.Key == key)?.Value;
      if (Map.TryGetValue(key, out string value))
      {
        return value;
      }
      return string.Empty;
    }

    public List<Tuple<string, string>> GetRelations()
    {
      List<Tuple<string, string>> relations = new List<Tuple<string, string>>();
      foreach (var entry in Map)
      {
        relations.Add(new Tuple<string, string>(entry.Key, entry.Value));
      }
      return relations;
    }

    [Button]
    public void CopyValuesFrom(StringToStringRelationProfile profile)
    {
      if (profile != null)
      {
        relations = new List<StringToStringRelation>(profile.Relations);
      }
    }

    [Button]
    public void AddMissingValuesFrom(StringToStringRelationProfile profile, bool copyValue = true)
    {
      if (profile != null)
      {
        foreach (var relation in profile.Relations)
        {
          // Check if there is no entry for that key
          if (Relations.Find(r => r.Key == relation.Key) == null)
          {
            Relations.Add(new StringToStringRelation(relation.Key, copyValue ? relation.Value : string.Empty));
          }
        }
      }
    }

    [Button]
    public void AssignValueToEmptyValues(string value)
    {
      foreach (var relation in Relations)
      {
        // Check if there is no value for that key
        if (string.IsNullOrEmpty(relation.Value))
        {
          relation.Value = value;
        }
      }
    }

    [Button]
    [ContextMenu("Sort By Name")]
    public void SortByName()
    {
      relations.RemoveAll(r => r == null);
      relations = relations.OrderBy(r => r.Key).ToList();
    }
  }
}