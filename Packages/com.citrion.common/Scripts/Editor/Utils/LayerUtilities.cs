using UnityEditor;

namespace CitrioN.Common.Editor
{
  public static class LayerUtilities
  {
    private static SerializedObject GetTabManagerObject()
    {
      var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      if (tagManager == null)
      {
        ConsoleLogger.LogError("Failed to load the TagManager file.");
      }
      return tagManager;
    }

    private static SerializedProperty GetLayersProperty()
    {
      var tagManager = GetTabManagerObject();
      SerializedProperty layers = tagManager?.FindProperty("layers");

      if (layers == null || !layers.isArray)
      {
        ConsoleLogger.LogError("Failed to access the layers property in the TagManager file.");
        return null;
      }
      return layers;
    }

    public static bool IsLayerInProject(string layerName)
    {
      var layers = GetLayersProperty();
      if (layers == null) { return false; }

      for (int i = 0; i < layers.arraySize; i++)
      {
        SerializedProperty layer = layers.GetArrayElementAtIndex(i);

        if (layer.stringValue == layerName)
        {
          return true;
        }
      }

      return false;
    }

    public static bool AddLayerToProject(string layerName)
    {
      SerializedObject tagManager =
        new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      SerializedProperty layers = tagManager.FindProperty("layers");

      if (layers == null || !layers.isArray)
      {
        ConsoleLogger.LogWarning("Failed to access the layers property in the TagManager file.");
        return false;
      }

      if (IsLayerInProject(layerName))
      {
        ConsoleLogger.LogWarning($"Layer with name '{layerName}' already exists.");
        return false;
      }

      for (int i = 0; i < layers.arraySize; i++)
      {
        SerializedProperty layer = layers.GetArrayElementAtIndex(i);

        if (string.IsNullOrEmpty(layer.stringValue))
        {
          layer.stringValue = layerName;
          ConsoleLogger.Log($"Successfully added layer with name '{layerName}'.");
          tagManager.ApplyModifiedProperties();
          return true;
        }
      }

      ConsoleLogger.LogWarning("Failed to add new layer as there are no available layer slots in the TagManager file.");
      return false;
    }
  }
}