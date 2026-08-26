#if NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CitrioN.Common.Editor
{
  public static class PackageUtilities
  {
    public static void PrintPackageSampleNames(string packageId, string packageVersion = "1.0.0")
    {
      var samples = Sample.FindByPackage(packageId, packageVersion);
      foreach (var sample in samples)
      {
        ConsoleLogger.Log(sample.displayName);
      }
    }

    /// <summary>
    /// Wrapper for installing/adding a package to the project
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    public static void InstallPackage(string packageId)
    {
      Client.Add(packageId);
    }

    public static bool IsPackageInstalled(string packageId, out string packageVersion)
    {
      #region Old - Not working for embedded/custom packages
      //  if (!File.Exists("Packages/manifest.json")) { return false; }

      //  string json = File.ReadAllText("Packages/manifest.json");
      //  return json.Contains(packageId); 
      #endregion

      List<PackageInfo> packageJsons = AssetDatabase.FindAssets("package")
        ?.Select(AssetDatabase.GUIDToAssetPath)?.Where(i => AssetDatabase.LoadAssetAtPath<TextAsset>(i) != null)
        ?.Select(PackageInfo.FindForAssetPath)?.ToList();

      var packageJson = packageJsons?.Find(i => i != null && i.name == packageId);

      bool isInstalled = packageJson != null;
      packageVersion = isInstalled ? packageJson.version : string.Empty;
      return isInstalled;
    }

    public static RemoveRequest RemovePackage(string packageId)
    {
      if (!IsPackageInstalled(packageId, out var packageVersion)) { return null; }

      return Client.Remove(packageId);
    }

    public static void RemovePackages(List<string> packagesToRemove)
    {
#if !NEWTONSOFT_JSON
      ConsoleLogger.LogError("Removing packages requires the 'Newtonsoft Json' package to be installed!");
      return;
#else
      if (packagesToRemove == null || packagesToRemove.Count == 0)
      {
        ConsoleLogger.LogWarning("No packages provided to remove.");
        return;
      }

      DirectoryInfo directoryInfo = Directory.GetParent(Application.dataPath);
      if (directoryInfo == null)
      {
        ConsoleLogger.LogWarning("Unable to find valid path to remove packages from.");
        return;
      }

      string lockFilePath = directoryInfo.FullName + "/Packages/packages-lock.json";
      string manifestPath = directoryInfo.FullName + "/Packages/manifest.json";

      if (!File.Exists(lockFilePath))
      {
        Debug.LogError("packages-lock.json file not found.");
        return;
      }

      if (!File.Exists(manifestPath))
      {
        Debug.LogError("manifest.json file not found.");
        return;
      }

      var manifestLines = File.ReadAllLines(manifestPath).ToList();
      var lockFileLines = File.ReadAllLines(lockFilePath).ToList();

      //var packageInfos = new List<PackageInfo>();
      //EditorJsonUtility.FromJsonOverwrite(json, packageInfos);

      FileUtility.CreateFileBackup(lockFilePath);
      FileUtility.CreateFileBackup(manifestPath);

      var json = File.ReadAllText(lockFilePath);
      var obj = JsonConvert.DeserializeObject<PackagesLockFile>(json);

      for (int i = 0; i < packagesToRemove.Count; i++)
      {
        var packageId = packagesToRemove[i];
        if (string.IsNullOrEmpty(packageId)) { continue; }

        manifestLines.RemoveAll(l => l.TrimStart().StartsWith($"\"{packageId}\":"));

        obj.dependencies.Remove(packageId);

        string embeddedPath = $"{directoryInfo.FullName}/Packages/{packageId}";
        bool exists = Directory.Exists(embeddedPath);

        if (exists)
        {
          Directory.Delete(embeddedPath, true);
        }
      }

      string newJson = JsonConvert.SerializeObject(obj, Formatting.Indented);

      var sb = new StringBuilder();
      using (StringReader sr = new StringReader(newJson))
      {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
          if (!line.Contains("\"url\": \"\""))
          {
            sb.AppendLine(line);
          }
        }
      }
      newJson = sb.ToString();

      File.WriteAllText(lockFilePath, newJson);
      //File.WriteAllLines(lockFilePath, lockFileLines);
      File.WriteAllLines(manifestPath, manifestLines);

      //EditorUtilities.RecompileScripts();
      Client.Resolve();
#endif
    }

    public static bool GetPackageVersion(string packageId, out string packageVersion)
    {
      var isInstalled = IsPackageInstalled(packageId, out packageVersion);
      return isInstalled;
    }

    public static void ShowPackageInPackageManager(string packageId)
    {
      Window.Open(packageId);
    }

    public static DependencyInfo[] GetPackageDependencies(string packageId)
    {
      var packageFiles = AssetDatabase.FindAssets("package", new string[] { $"Packages/{packageId}" })
        ?.Select(AssetDatabase.GUIDToAssetPath)?.Where(i => AssetDatabase.LoadAssetAtPath<TextAsset>(i) != null)
        ?.Select(PackageInfo.FindForAssetPath)?.ToList();

      if (packageFiles == null || packageFiles.Count < 1)
      {
        return null;
      }

      var packageFile = packageFiles[0];
      return packageFile?.dependencies;
    }

    public static PackageInfo[] GetAllPackages()
      => PackageInfo.GetAllRegisteredPackages();

    public class PackagesLockFile
    {
      public Dictionary<string, CustomPackageInfo> dependencies = new Dictionary<string, CustomPackageInfo>();
    }

    public class CustomPackageInfo
    {
      public string version;
      public int depth;
      public string source;
      //[JsonConverter(typeof(ConditionalPropertyConverter), "", nameof(PackageInfoCustom.url))]
      //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string url = null;
      public Dictionary<string, string> dependencies = new Dictionary<string, string>();
    }
  }
}