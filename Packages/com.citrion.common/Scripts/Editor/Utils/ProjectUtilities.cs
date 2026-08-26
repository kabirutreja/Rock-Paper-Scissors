using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  public static class ProjectUtilities
  {
    public static void ImportUnityPackageFromFilePath(string path, bool showDialog)
    {
      AssetDatabase.ImportPackage(path, showDialog);
    }

    public static void ImportUnityPackageFromFilePathWithDialog(string path)
    {
      AssetDatabase.ImportPackage(path, interactive: true);
    }

    public static void ImportUnityPackageFromFilePathWithoutDialog(string path)
    {
      AssetDatabase.ImportPackage(path, interactive: false);
    }

    public static bool IsFolderInProject(string folderPath)
    {
      if (!(folderPath.StartsWith("Assets/") || folderPath.StartsWith("Packages/"))) { return false; }

      var obj = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
      #if UNITY_6000_5_OR_NEWER
      var instanceID = obj.GetEntityId();
      #else
      var instanceID = obj.GetInstanceID();
      #endif

      if (!ProjectWindowUtil.IsFolder(instanceID)) { return false; }

      return true;
    }

    public static List<Object> GetAssetsInFolder(string folderPath)
    {
      if (!IsFolderInProject(folderPath)) { return null; }

      var guids = AssetDatabase.FindAssets("", new[] { folderPath });

      if (guids?.Length > 0)
      {
        List<Object> assets = new List<Object>();
        foreach (var guid in guids)
        {
          var assetPath = AssetDatabase.GUIDToAssetPath(guid);
          var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
          if (asset != null)
          {
            assets.Add(asset);
          }
        }
        return assets;
      }
      return null;
    }

    public static Object GetFirstAssetInFolder(string folderPath)
    {
      if (!IsFolderInProject(folderPath)) { return null; }

      var guids = AssetDatabase.FindAssets("", new[] { folderPath });

      if (guids?.Length > 0)
      {
        var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        return asset;
      }
      return null;
    }

    public static void ExpandFolder(string folderPath)
    {
      if (string.IsNullOrEmpty(folderPath)) { return; }

      if (!IsFolderInProject(folderPath)) { return; }

      var firstAssetInFolder = GetFirstAssetInFolder(folderPath);

      if (firstAssetInFolder != null)
      {
        var firstAssetPath = AssetDatabase.GetAssetPath(firstAssetInFolder);
        if (!string.IsNullOrEmpty(firstAssetPath))
        {
          Selection.activeObject = firstAssetInFolder;
        }
      }

      ScheduleUtility.InvokeDelayedByFrames(() =>
      {
        try { EditorUtilities.PingObjectAtPath(folderPath); }
        catch (System.Exception) { }
      });
    }
  }
}