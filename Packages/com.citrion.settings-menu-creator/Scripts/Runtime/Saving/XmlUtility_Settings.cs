using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CitrioN.SettingsMenuCreator
{
  public static class XmlUtility_Settings
  {
    public static string GetSaveString(SettingsCollection collection, Dictionary<string, object> settingValues)
    {
      if (settingValues == null) { settingValues = new Dictionary<string, object>(); }

      List<string> identifiers = new List<string>();

      if (collection != null)
      {
        foreach (var item in collection.activeSettingValues)
        {
          var identifier = item.Key;
          identifiers.AddIfNotContains(identifier);
          var value = item.Value;

          if (value == null) { continue; }

          settingValues.AddOrUpdateDictionaryItem(identifier, value);
        }

        foreach (var setting in collection.Settings)
        {
          if (setting == null) { continue; }
          var identifier = setting.Identifier;
          if (identifiers.Contains(identifier)) { continue; }
          identifiers.Add(identifier);
          var values = setting.Setting?.GetCurrentValues(collection);
          if (values == null || values.Count < 1) { continue; }
          var value = values[0];
          if (value == null) { continue; }

          settingValues.AddOrUpdateDictionaryItem(identifier, value);
        }
      }

      List<SerializedSetting> serializedSettings = new List<SerializedSetting>();

      foreach (var item in settingValues)
      {
        var identifier = item.Key;
        var value = item.Value;

        var type = value.GetType();
        string valueString = XmlUtility.ObjectToXml(value);

        var serializedSetting = new SerializedSetting(identifier, type, valueString);
        serializedSettings.Add(serializedSetting);
      }

      return XmlUtility.ObjectToXml(serializedSettings);
    }

    public static Dictionary<string, object> LoadFromText(string text)
    {
      var dict = new Dictionary<string, object>();

      List<SerializedSetting> settingsList = null;
      var listSerializer = new XmlSerializer(typeof(List<SerializedSetting>));

      using (var strReader = new StringReader(text))
      {
        using (var reader = XmlReader.Create(strReader))
        {
          settingsList = (List<SerializedSetting>)listSerializer.Deserialize(reader);
        }
      }

      if (settingsList == null) { return null; }

      foreach (var setting in settingsList)
      {
        var identifier = setting.Key;
        var typeString = setting.Type;
        var type = Type.GetType(typeString);
        var valueString = setting.Value;

        var value = XmlUtility.XmlStringToObject(valueString, type);
        dict.AddOrUpdateDictionaryItem(identifier, value);
      }
      return dict;
    }
  }
}