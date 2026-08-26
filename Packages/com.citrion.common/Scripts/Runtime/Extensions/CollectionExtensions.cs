using System;
using System.Collections.Generic;
using System.Linq;

namespace CitrioN.Common
{
#if BOLT
  [Ludiq.IncludeInSettings(true)]
#endif
  [SkipObfuscationRename]
  /// <summary>
  /// Extension methods for collections
  /// </summary>
  public static class CollectionExtensions
  {
    public static bool AddIfNotContains<T>(this T[] array, T value)
    {
      if (array.Contains(value))
      {
        return false;
      }
      Array.Resize(ref array, array.Length + 1);
      array[array.Length - 1] = value;
      return true;
    }

    public static void Add<T>(this T[] array, T value)
    {
      Array.Resize(ref array, array.Length + 1);
      array[array.Length - 1] = value;
    }

    [SkipObfuscationRename]
    /// <returns>True if the item was successfully added to the list</returns>
    public static bool AddIfNotContains<T>(this List<T> list, T value)
    {
      if (list.Contains(value))
      {
        return false;
      }
      list.Add(value);
      return true;
    }

    public static void AddOrUpdateDictionaryItem<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
    {
      if (dictionary == null) { return; }
      if (dictionary.ContainsKey(key))
      {
        dictionary[key] = value;
      }
      else
      {
        dictionary.Add(key, value);
      }
    }

    public static bool ContainsAny<T>(this T[] array, params T[] values)
    {
      if (array.Length < 1) { return false; }

      if (values?.Length > 0)
      {
        foreach (T value in values)
        {
          for (int i = 0; i < array.Length; i++)
          {
            T v = array[i];
            if (v != null && array[i].Equals(value))
            {
              return true;
            }
          }
          // TODO Check if this works as intended
          //if (Array.Exists(array, v => v.Equals(value)))
          //{
          //  return true;
          //}
        }
      }
      return false;
    }

    public static bool ContainsAny<T>(this List<T> list, params T[] values)
    {
      if (list.Count < 1) { return false; }

      if (values?.Length > 0)
      {
        foreach (var value in values)
        {
          if (list.Contains(value))
          {
            return true;
          }
        }
      }
      return false;
    }

    public static bool ContainsAny<T>(this List<T> list, List<T> values)
    {
      if (list.Count < 1) { return false; }

      if (values?.Count > 0)
      {
        foreach (var value in values)
        {
          if (list.Contains(value))
          {
            return true;
          }
        }
      }
      return false;
    }

    public static T GetRandom<T>(this T[] array)
    {
      if (array == null || array.Length < 1)
      {
        return default;
      }
      return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T GetRandom<T>(this T[] array, ref UnityEngine.Random.State state)
    {
      UnityEngine.Random.state = state;
      if (array == null || array.Length < 1)
      {
        return default;
      }
      var value = array[UnityEngine.Random.Range(0, array.Length)];
      state = UnityEngine.Random.state;
      return value;
    }

    public static T GetRandom<T>(this T[] array, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return array.GetRandom();
    }

    public static T GetRandom<T>(this List<T> list)
    {
      if (list == null || list.Count < 1)
      {
        return default;
      }
      return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T GetRandom<T>(this List<T> list, ref UnityEngine.Random.State state)
    {
      UnityEngine.Random.state = state;
      if (list == null || list.Count < 1)
      {
        return default;
      }
      var value = list[UnityEngine.Random.Range(0, list.Count)];
      state = UnityEngine.Random.state;
      return value;
    }

    public static T GetRandom<T>(this List<T> list, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return list.GetRandom();
    }

    public static T GetNext<T>(this List<T> list, ref int currentIndex)
    {
      if (list?.Count < 1)
      {
        return default;
      }
      currentIndex = (currentIndex + 1) % list.Count;
      return list[currentIndex];
    }

    public static bool IsEmpty<T>(this List<T> list)
    {
      return list.Count < 1;
    }

    public static T Find<T>(this T[] array, Func<T, bool> predicate)
    {
      for (int i = 0; i < array.Length; i++)
      {
        if (predicate.Invoke(array[i]))
        {
          return array[i];
        }
      }
      return default;
    }

    public static void MergeDictionaryInto<T, V>(this IDictionary<T, V> dic, IDictionary<T, V> toMerge)
    {
      dic.AddEntriesToDictionary(toMerge);
    }

    public static void AddEntriesToDictionary<T, V>(this IDictionary<T, V> dic1, IDictionary<T, V> dic2,
                                            bool overrideExisting = false)
    {
      foreach (var item in dic2)
      {
        if (dic1.ContainsKey(item.Key) && overrideExisting)
        {
          dic1[item.Key] = item.Value;
        }
        else
        {
          dic1.Add(item.Key, item.Value);
        }
      }
    }
  }
}