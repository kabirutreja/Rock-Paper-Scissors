// Cristian Pop - https://boxophobic.com/

using Boxophobic.StyledGUI;
using Boxophobic.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static Boxophobic.Utility.BoxoUtils;

public class BugReporter : EditorWindow
{
    //const string EXPORT_PATH = "Assets/ExportedForTesting.unitypackage";
    //const string EXCLUDE_FOLDER = "The Visual Engine";

    string reportProject = "";
    //string reportDetails = "";

    GUIStyle styleLabel;

    Color bannerColor;
    string bannerText;
    static BugReporter window;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/BOXOPHOBIC/Report Bug", false, 9999)]
    public static void ShowWindow()
    {
        window = GetWindow<BugReporter>(false, "Report Bug", true);
        window.minSize = new Vector2(400, 300);
    }

    void OnEnable()
    {
        reportProject = "";

        var pipeline = BoxoUtils.GetProjectPipeline();

        //reportProject += "TVE Version: " + stringVersion + "\n\n";
        reportProject += "Unity Version: " + Application.unityVersion + "\n";
        reportProject += "Unity Pipeline: " + pipeline + "\n";
        reportProject += "Unity Platform: " + Application.platform + "\n";
        reportProject += "Unity Color Space: " + QualitySettings.activeColorSpace + "\n";
        reportProject += "Unity Graphics API: " + SystemInfo.graphicsDeviceType + "\n";

        reportProject += "\n";

        reportProject += "OS: " + SystemInfo.operatingSystem + "\n";
        reportProject += "Graphics: " + SystemInfo.graphicsDeviceName + "\n";

        //reportProject += "\n";
        //reportProject += "Additional Details";

        bannerColor = new Color(0.9f, 0.7f, 0.4f);
        bannerText = "Report Details";
    }

    void OnGUI()
    {
        SetGUIStyles();

        StyledGUI.DrawWindowBanner(bannerColor, bannerText);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginVertical();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 80));

        GUILayout.Label(reportProject, styleLabel);

        //reportDetails = GUILayout.TextArea(reportDetails);

        //if (GUILayout.Button("Take Editor Screensshot", GUILayout.Height(24)))
        //{
        //    var vec2Position = Vector2.zero;
        //    var sizeX = Screen.currentResolution.width;
        //    var sizeY = Screen.currentResolution.height;

        //    var colors = InternalEditorUtility.ReadScreenPixel(vec2Position, (int)sizeX, (int)sizeY);

        //    var result = new Texture2D((int)sizeX, (int)sizeY);
        //    result.SetPixels(colors);

        //    var bytes = result.EncodeToPNG();

        //    File.WriteAllBytes("Assets/Editor Screenshot.png", bytes);

        //    Object.DestroyImmediate(result);

        //    AssetDatabase.Refresh();

        //    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Editor Screenshot.png"));
        //}

        GUILayout.Space(10);

        if (GUILayout.Button("Copy Details To Clipboard", GUILayout.Height(24)))
        {
            var copyData = reportProject;

            GUIUtility.systemCopyBuffer = copyData;
        }

        if (GUILayout.Button("Export Selected Objects", GUILayout.Height(24)))
        {
            ExportSelectedObjects();
        }

        GUILayout.FlexibleSpace();

        GUILayout.Space(20);

        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.Space(13);
        GUILayout.EndHorizontal();
    }

    void SetGUIStyles()
    {
        styleLabel = new GUIStyle(EditorStyles.label)
        {
            richText = true,
        };
    }

    [MenuItem("GameObject/Export For Testing", false, 11)]
    [MenuItem("Assets/Export For Testing", false, 30)]
    static void ExportSelectedObjects()
    {
        Object[] selection = Selection.objects;

        if (selection == null || selection.Length == 0)
        {
            Debug.Log("<b>[The Visual Engine]</b> " + "Nothing selected to export!");

            return;
        }

        var projectData = BoxoUtils.GetProjectData();

        var saveName = "Exported_" + projectData.pipeline + "_" + Application.unityVersion;
        var savePath = EditorUtility.SaveFilePanelInProject("Save Package", saveName, "unityPackage", "");

        if (string.IsNullOrEmpty(savePath))
        {
            return;
        }

        HashSet<string> assetPaths = new HashSet<string>();

        foreach (Object obj in selection)
        {
            if (obj is GameObject go)
            {
                GatherPrefab(go, assetPaths);
                GatherGameObject(go, assetPaths);
                GatherScene(go, assetPaths);

                Terrain terrain = go.GetComponent<Terrain>();
                if (terrain != null)
                    GatherTerrain(terrain, assetPaths);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(obj);
                TryAddPath(path, assetPaths);
                GatherModelAsset(path, assetPaths);
            }
        }

        if (assetPaths.Count == 0)
        {
            Debug.Log("<b>[The Visual Engine]</b> " + "No valid assets gathered!");

            return;
        }

        AssetDatabase.ExportPackage(
            new List<string>(assetPaths).ToArray(),
            savePath,
            ExportPackageOptions.Interactive
        );

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(savePath));
    }

    static void GatherPrefab(GameObject go, HashSet<string> paths)
    {
        GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);

        if (prefabAsset != null)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabAsset);
            TryAddPath(prefabPath, paths);
        }

        if (PrefabUtility.IsPartOfPrefabAsset(go))
        {
            string prefabPath = AssetDatabase.GetAssetPath(go);
            TryAddPath(prefabPath, paths);
        }
    }

    static void GatherGameObject(GameObject go, HashSet<string> paths)
    {
        foreach (var mf in go.GetComponentsInChildren<MeshFilter>(true))
            TryAddAsset(mf.sharedMesh, paths);

        foreach (var col in go.GetComponentsInChildren<Collider>(true))
            if (col is MeshCollider meshCol)
                TryAddAsset(meshCol.sharedMesh, paths);

        foreach (var renderer in go.GetComponentsInChildren<Renderer>(true))
            foreach (var mat in renderer.sharedMaterials)
                GatherMaterial(mat, paths);
    }

    static void GatherTerrain(Terrain terrain, HashSet<string> paths)
    {
        if (terrain == null)
            return;

        TerrainData data = terrain.terrainData;

        if (data != null)
        {
            if (AssetDatabase.Contains(data))
                TryAddAsset(data, paths);

            TerrainLayer[] layers = data.terrainLayers;
            if (layers != null)
            {
                foreach (var layer in layers)
                {
                    if (layer == null) continue;

                    if (AssetDatabase.Contains(layer))
                        TryAddAsset(layer, paths);

                    TryAddAsset(layer.diffuseTexture, paths);
                    TryAddAsset(layer.normalMapTexture, paths);
                    TryAddAsset(layer.maskMapTexture, paths);
                }
            }

            DetailPrototype[] details = data.detailPrototypes;
            if (details != null)
            {
                foreach (var detail in details)
                {
                    if (detail.prototype != null)
                    {
                        GatherPrefab(detail.prototype, paths);
                        GatherGameObject(detail.prototype, paths);
                    }

                    if (detail.prototypeTexture != null)
                        TryAddAsset(detail.prototypeTexture, paths);
                }
            }

            TreePrototype[] trees = data.treePrototypes;
            if (trees != null)
            {
                foreach (var tree in trees)
                {
                    if (tree.prefab != null)
                    {
                        GatherPrefab(tree.prefab, paths);
                        GatherGameObject(tree.prefab, paths);

                        string treePath = AssetDatabase.GetAssetPath(tree.prefab);
                        TryAddPath(treePath, paths);
                        GatherModelAsset(treePath, paths);
                    }
                }
            }
        }

        Material terrainMat = terrain.materialTemplate;
        if (terrainMat != null && AssetDatabase.Contains(terrainMat))
            GatherMaterial(terrainMat, paths);
    }

    static void GatherModelAsset(string assetPath, HashSet<string> paths)
    {
        if (string.IsNullOrEmpty(assetPath))
            return;

        var importer = AssetImporter.GetAtPath(assetPath);
        if (!(importer is ModelImporter))
            return;

        bool isSpeedTree = IsSpeedTreeAsset(importer);

        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var sub in subAssets)
        {
            if (sub is Material mat)
                GatherMaterial(mat, paths, isSpeedTree);
            else if (sub is Mesh mesh)
                TryAddAsset(mesh, paths);
        }

        var dependencies = AssetDatabase.GetDependencies(assetPath, true);
        foreach (var dep in dependencies)
        {
            if (dep == assetPath) continue;

            Object depObj = AssetDatabase.LoadAssetAtPath<Object>(dep);

            if (depObj is Material mat)
                GatherMaterial(mat, paths, isSpeedTree);
            else
                TryAddPath(dep, paths);
        }
    }

    static bool IsSpeedTreeAsset(AssetImporter importer)
    {
        if (importer == null) return false;
        return importer.assetPath.ToLower().Contains("speedtree");
    }

    static void GatherMaterial(Material mat, HashSet<string> paths, bool skipSpeedTreeShaders = false)
    {
        if (mat == null) return;

        TryAddAsset(mat, paths);

        if (mat.shader != null)
        {
            if (!(skipSpeedTreeShaders && mat.shader.name.Contains("SpeedTree")))
                TryAddAsset(mat.shader, paths);
        }

        if (mat.shader == null) return;

        int count = BoxoUtils.GetShaderPropertyCount(mat.shader);
        for (int i = 0; i < count; i++)
        {
            if (BoxoUtils.GetShaderPropertyType(mat.shader, i) == ShaderPropertyType.Texture)
            {
                string prop = BoxoUtils.GetShaderPropertyName(mat.shader, i);
                Texture tex = mat.GetTexture(prop);
                TryAddAsset(tex, paths);
            }
        }
    }

    static void GatherScene(GameObject go, HashSet<string> paths)
    {
        Scene scene = go.scene;
        if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
            return;

        TryAddPath(scene.path, paths);
    }

    static void TryAddAsset(Object obj, HashSet<string> paths)
    {
        if (obj == null)
            return;

        if (!AssetDatabase.Contains(obj))
            return;

        string path = AssetDatabase.GetAssetPath(obj);

        if (string.IsNullOrEmpty(path))
            return;

        if (path.Contains("The Visual Engine"))
            return;

        paths.Add(path);
    }

    static void TryAddPath(string path, HashSet<string> paths)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!path.StartsWith("Assets"))
            return;

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (!AssetDatabase.Contains(asset))
            return;

        if (path.Contains("The Visual Engine"))
            return;

        paths.Add(path);
    }
}


