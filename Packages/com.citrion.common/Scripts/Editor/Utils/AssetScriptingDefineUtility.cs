using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;

namespace CitrioN.Common.Editor
{
  [InitializeOnLoad]
  public static class AssetScriptingDefineUtility
  {
    public static void UpdateScriptingDefineForAsset(string pathToAsmdef, string define)
    {
      ConsoleLogger.Log($"Checking for define: {define}", Common.LogType.Debug);
      //bool isImported = AssetDatabase.IsValidFolder($"Assets/{rootFolderName}");
      bool isImported = AssetDatabase.GetMainAssetTypeAtPath($"Assets/{pathToAsmdef}") ==
                        typeof(UnityEditorInternal.AssemblyDefinitionAsset);
      bool isDefineSet = IsScriptingDefineSet(define);

      if (!isDefineSet && isImported)
      {
        ConsoleLogger.Log($"Found {pathToAsmdef} folder, adding scripting define: {define}");
        UpdateScriptingDefineSymbols(define, true);
      }
      else if (isDefineSet && !isImported)
      {
        ConsoleLogger.Log($"No {pathToAsmdef} folder found, removing scripting define: {define}");
        UpdateScriptingDefineSymbols(define, false);
      }
    }

    public static void UpdateScriptingDefineForAnyAsset(string relativePath, string define)
    {
      ConsoleLogger.Log($"Checking for define: {define}", Common.LogType.Debug);

      var path = $"Assets/{relativePath}";
      //var guid = AssetDatabase.GUIDFromAssetPath(path);
      var type = AssetDatabase.GetMainAssetTypeAtPath(path);
      //var isImported = guid != null && !guid.ToString().Equals("00000000000000000000000000000000");
      var isImported = type != null;
      bool isDefineSet = IsScriptingDefineSet(define);

      if (!isDefineSet && isImported)
      {
        ConsoleLogger.Log($"Found {relativePath}, adding scripting define: {define}");
        UpdateScriptingDefineSymbols(define, true);
      }
      else if (isDefineSet && !isImported)
      {
        ConsoleLogger.Log($"No {relativePath} found, removing scripting define: {define}");
        UpdateScriptingDefineSymbols(define, false);
      }
    }

    public static bool IsScriptingDefineSet(string define)
    {
      BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_2023_1_OR_NEWER
      var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
      PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);
      return defines != null && defines.Contains(define);
#else
      string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
      return defines.Contains(define);
#endif
    }

    public static void UpdateScriptingDefineSymbols(string define, bool add)
    {
      BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

#if UNITY_2023_1_OR_NEWER
      var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
      PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);
#else
      string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif

      bool contains = defines.Contains(define);

      if (add && contains) { return; }
      else if (!add && !contains) { return; }

      if (add)
      {
#if UNITY_2023_1_OR_NEWER
        var definesAsString = new StringBuilder();
        foreach (var d in defines) { definesAsString.Append($"{d};"); }
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, $"{definesAsString}{define}");
#else
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, $"{defines};{define}");
#endif
      }
      else
      {
#if UNITY_2023_1_OR_NEWER
        var definesAsString = new StringBuilder();
        foreach (var d in defines) { definesAsString.Append($"{d};"); }
        var newDefines = definesAsString.ToString().Replace(define, string.Empty);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);
#else
        defines = defines.Replace(define, string.Empty);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
#endif
      }
    }
  }
}