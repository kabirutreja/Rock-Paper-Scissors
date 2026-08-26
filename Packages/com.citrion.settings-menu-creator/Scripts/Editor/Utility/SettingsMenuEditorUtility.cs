using CitrioN.Common;
using CitrioN.Common.Editor;
using UnityEditor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public static class SettingsMenuEditorUtility
  {
    [MenuItem("Tools/CitrioN/Settings Menu Creator/Dependencies/Import Newtonsoft Json Package")]
    public static void AddNewtonsoftJsonPackage()
    {
      PackageUtilities.InstallPackage("com.unity.nuget.newtonsoft-json");
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Dependencies/Import 2D Sprite Package")]
    public static void Add2DSpritePackage()
    {
      PackageUtilities.InstallPackage("com.unity.2d.sprite");
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Dependencies/Import Post Processing Package")]
    public static void AddPostProcessingPackage()
    {
      PackageUtilities.InstallPackage("com.unity.postprocessing");
    }

#if TEXT_MESH_PRO
    [MenuItem("Tools/CitrioN/Settings Menu Creator/Dependencies/Import TMP Essential Resources")]
    public static void ImportTextMeshProEssentialResources()
    {
      TMPro.TMP_PackageUtilities.ImportProjectResourcesMenu();
    }
#endif
  }
}