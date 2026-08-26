using System.Collections.Generic;
using System.Linq;

namespace CitrioN.Common
{
#if BOLT
  [Ludiq.IncludeInSettings(true)]
#endif
  public static class CollectionUtilities
  {
    public static IDictionary<T, V> MergeIDictionariesIntoNew<T, V>(IDictionary<T, V> dic1, IDictionary<T, V> dic2)
    {
      IDictionary<T, V> result = new Dictionary<T, V>();
      result.AddEntriesToDictionary(dic1);
      result.AddEntriesToDictionary(dic2);
      return result;
    }

    public static T GetDictionaryKeyFromIndexGeneric<T, V>(IDictionary<T, V> dictionary, int index)
    {
      if (index >= dictionary.Count) { return default; }
      return dictionary.Keys.ElementAt(index);
    }

    public static object GetDictionaryKeyFromIndex<T, V>(IDictionary<T, V> dictionary, int index)
    {
      if (index >= dictionary.Count) { return null; }
      return dictionary.Keys.ElementAt(index);
    }

    public static V GetDictionaryValueFromIndexGeneric<T, V>(IDictionary<T, V> dictionary, int index)
    {
      if (index >= dictionary.Count) { return default; }
      return dictionary[dictionary.Keys.ElementAt(index)];
    }

    public static object GetDictionaryValueFromIndex<T, V>(IDictionary<T, V> dictionary, int index)
    {
      if (index >= dictionary.Count) { return null; }
      return dictionary[dictionary.Keys.ElementAt(index)];
    }
  }
}