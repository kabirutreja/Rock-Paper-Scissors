using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public static class PreferencesUtility
  {
    public static SerializedObject GetSerializedSettings<T>(string fileDirectory, string fileName)
      where T : AbstractPreferences
    {
      GetOrCreateSettings<T>(fileDirectory, fileName, out var settings);
      return new SerializedObject(settings);
    }

    public static bool GetOrCreateSettings<T>(string fileDirectory, string fileName, out T value) 
      where T : AbstractPreferences
    {
      var allSettingsOfType = GetScriptableObjectsOfTypeInProject<T>();
      var settings = allSettingsOfType != null && allSettingsOfType.Count > 0 ? allSettingsOfType[0] : null;
      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<T>();
        PresetUtilities.ApplyPresets(settings);

        settings.Initialize();

        if (!Directory.Exists(fileDirectory))
        {
          Directory.CreateDirectory(fileDirectory);
        }

        var filePath = fileDirectory + fileName;
        AssetDatabase.CreateAsset(settings, filePath);
        AssetDatabase.SaveAssets();
        value = settings;
        return false;
      }
      value = settings;
      return true;
    }

    public static List<T> GetScriptableObjectsOfTypeInProject<T>() where T : ScriptableObject
    {
      return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
                  .Select(AssetDatabase.GUIDToAssetPath)
                  .Select(AssetDatabase.LoadAssetAtPath<T>)
                  .ToList();
    }
  }
}