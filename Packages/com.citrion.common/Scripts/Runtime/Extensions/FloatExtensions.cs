using System;
using System.Globalization;
using UnityEngine;

namespace CitrioN.Common
{
#if BOLT
  [Ludiq.IncludeInSettings(true)]
#endif
  public static class FloatExtensions
  {
    public static float Truncate(this float value, int digits)
    {
      float multiplier = Mathf.Pow(10.0f, digits);
      return (float)Math.Truncate(Mathf.Round(value * multiplier)) / multiplier;
    }

    public static bool GetRandom(this float successChance)
    {
      if (successChance >= 1) return true;
      if (successChance <= 0) return false;
      return UnityEngine.Random.Range(0f, 1f) <= successChance;
    }

    public static bool GetRandom(this float successChance, ref UnityEngine.Random.State state)
    {
      if (successChance >= 1) return true;
      if (successChance <= 0) return false;
      return RandomUtilities.GetRandom(0f, 1f, ref state) <= successChance;
    }

    public static bool GetRandom(this float successChance, int seed)
    {
      UnityEngine.Random.InitState(seed);
      return successChance.GetRandom();
    }

    public static bool TryParseFloat(this string value, out float inputValue)
    {
      value = value.Replace(',', '.');
      return float.TryParse(value, NumberStyles.Float,
        CultureInfo.InvariantCulture, out inputValue);
    }

    // TODO This doesn't seem to work as intended
    //public static bool TryParseFloat(string input, out float result)
    //{
    //  NumberStyles styles = NumberStyles.Float/* | NumberStyles.AllowThousands*/;
    //  CultureInfo[] cultures = { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("en-US") };

    //  foreach (var culture in cultures)
    //  {
    //    if (float.TryParse(input, styles, culture, out result)) { return true; }
    //  }

    //  result = 0;
    //  return false;
    //}
  }
}