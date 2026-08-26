using System;
using System.ComponentModel;
using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Extension methods for strings
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Returns the provided string colorized with the provided color
    /// </summary>
    /// <param name="color">The color for the string</param>
    /// <returns>The colorized string</returns>
    public static string Colorize(this string input, Color color)
    {
      string colorHex = ColorUtility.ToHtmlStringRGBA(color);
      return System.String.Format("<color=#{0}>{1}</color>", colorHex, input);
    }

    public static string Bold(this string input)
    {
      return System.String.Format("<b>{0}</b>", input);
    }

    /// <summary>
    /// Splits the string whenever there is an upper case character and adds a space
    /// </summary>
    /// <param name="fillString">A string to be appended to the resulting string after every split</param>
    /// <returns>The split string</returns>
    public static string SplitCamelCase(this string source, string fillString = " ")
    {
      source = System.Text.RegularExpressions.Regex.Replace(source, "A-Z", " $1");

      #region Old Verion
      var splitStringArray = System.Text.RegularExpressions.Regex.Split(source, @"(?<!^)(?=[A-Z])");
      System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
      foreach (string stringItem in splitStringArray)
      {
        stringBuilder.Append(stringItem);
        stringBuilder.Append(fillString);
      }
      string formattedString = stringBuilder.ToString();
      return formattedString.Substring(0, formattedString.Length - 1);
      #endregion
    }

    /// <summary>
    /// Splits the input by camel case and capitalizes the first character.
    /// Calling this function with the name of a <see cref="FieldInfo"/>
    /// will format it in a way Unity displays field names in the inspector.
    /// </summary>
    public static string FormatFieldOrClassName(this string input)
    {
      input = input.SplitCamelCase();
      var firstCharacter = input[0].ToString().ToUpperInvariant();
      return firstCharacter + input.Substring(1);
    }

    public static string ToUpperFirstCharacter(this string input)
    {
      if (input == null) { return null; }
      var length = input.Length;
      if (length == 0) { return input; }
      if (length == 1) { return input.ToUpper(); }
      return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1).ToString());
    }

    public static T TryParse<T>(this string input)
    {
      T output = default;
      var converter = TypeDescriptor.GetConverter(typeof(T));
      try
      {
        output = (T)(converter?.ConvertFromInvariantString(input));
      }
      catch (NotSupportedException e)
      {
        ConsoleLogger.LogError(e.Message, Common.LogType.Always);
        //throw;
      }
      return output;
    }
  }
}