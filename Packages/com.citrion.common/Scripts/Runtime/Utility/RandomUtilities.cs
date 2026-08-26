using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Class with methods related to random number generation
  /// </summary>
  [SkipObfuscation]
  public static class RandomUtilities
  {
    [SkipObfuscationRename]
    public static int GetRandom(int min, int max, ref Random.State state)
    {
      Random.state = state;
      var value = Random.Range(min, max);
      state = Random.state;
      return value;
    }

    [SkipObfuscationRename]
    public static float GetRandom(float min, float max, ref Random.State state)
    {
      Random.state = state;
      var value = Random.Range(min, max);
      state = Random.state;
      return value;
    }

    [SkipObfuscationRename]
    public static float GetRandom(float min, float max, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return Random.Range(min, max);
    }

    [SkipObfuscationRename]
    public static int GetRandom(int min, int max, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return Random.Range(min, max);
    }

    [SkipObfuscationRename]
    public static bool GetRandom(float chanceForTrue, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return Random.Range(0f, 1f) <= chanceForTrue;
    }
  }
}