using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public static class AssetUtilities
  {
    public static IEnumerable<T> GetAllAssetsOfType<T>(params string[] folders)
    {
      if (!typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
      {
        return Enumerable.Empty<T>();
      }

      var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", folders)
          .Select(AssetDatabase.GUIDToAssetPath)
          .Select(AssetDatabase.LoadMainAssetAtPath)
          .OfType<T>();

      EditorUtility.UnloadUnusedAssetsImmediate();
      return assets;
    }

    public static IEnumerable<UnityEngine.Object> GetAllAssetsOfName(string name)
    {
      var assets = AssetDatabase.FindAssets($"{name}")
          .Select(AssetDatabase.GUIDToAssetPath)
          .Select(AssetDatabase.LoadMainAssetAtPath);
      return assets;
    }

    public static string GetRelativePath(string absolutePath)
    {
      return absolutePath.Substring(absolutePath.LastIndexOf("Assets"));
    }

    public static T CreateScriptableObjectAssetWithSaveFilePanel<T>(string defaultName = null) where T : ScriptableObject
    {
      if (string.IsNullOrEmpty(defaultName)) { defaultName = $"{typeof(T).Name}_"; }
      var path = EditorUtility.SaveFilePanel("Select save path", "Assets", defaultName + ".asset", "asset");

      if (path.Length != 0)
      {
        path = GetRelativePath(path);
        var instance = ScriptableObject.CreateInstance<T>();
        PresetUtilities.ApplyPresets(instance);
        var newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}");
        AssetDatabase.CreateAsset(instance, newAssetPath);
        var newAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newAssetPath);
        EditorUtility.SetDirty(newAsset);
        EditorUtilities.PingObject(newAsset);
        return newAsset as T;
      }
      return null;
    }
  }
}