using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;

namespace CitrioN.Common.Editor
{
  public static class PresetUtilities
  {
    public static void InsertPresetAsFirstDefault(Preset preset, string filter = "")
    {
      var type = preset.GetPresetType();
      if (type.IsValidDefault())
      {
        var list = Preset.GetDefaultPresetsForType(type).ToList();
        list.Insert(0, new DefaultPreset(filter, preset));
        Preset.SetDefaultPresetsForType(type, list.ToArray());
      }
    }

    public static void AddToDefaultPresets(Preset preset, string filter = "")
    {
      if (preset == null) { return; }
      var type = preset.GetPresetType();
      if (type.IsValidDefault())
      {
        Preset.RemoveFromDefault(preset);
        var list = Preset.GetDefaultPresetsForType(type).ToList();
        list.Add(new DefaultPreset(filter, preset));
        Preset.SetDefaultPresetsForType(type, list.ToArray());
      }
    }

    public static void ApplyPresets(UnityEngine.Object obj)
    {
      if (obj == null) { return; }
      var presets = Preset.GetDefaultPresetsForObject(obj);
      foreach (var p in presets)
      {
        p.ApplyTo(obj);
      }
    }

    public static void UpdatePresets(params string[] presetsFolderPaths)
    {
      // TODO Should this handle the case where the entire project is searched for presets?
      if (presetsFolderPaths == null || presetsFolderPaths.Length == 0) { return; }

      var pathList = new List<string>();
      foreach (var p in presetsFolderPaths)
      {
        string path = p.ToString();
        if (AssetDatabase.IsValidFolder(path))
        {
          pathList.AddIfNotContains(path);
        }
      }

      //if (!AssetDatabase.IsValidFolder(presetsFolderPath))
      if (pathList.IsEmpty())
      {
        return;
      }

      //var presetDataGuids = AssetDatabase.FindAssets("t:PresetData", new string[] { presetsFolderPath });
      var presetDataGuids = AssetDatabase.FindAssets("t:PresetData", pathList.ToArray());
      var presetDataList = new List<PresetData>();

      foreach (var guid in presetDataGuids)
      {
        var presetDataPath = AssetDatabase.GUIDToAssetPath(guid);
        var presetData = AssetDatabase.LoadAssetAtPath<PresetData>(presetDataPath);
        if (presetData != null)
        {
          presetDataList.AddIfNotContains(presetData);
        }
      }

      var priorityOrderedPresetDataList = presetDataList.OrderBy(i => i.Priority).ToList();

      foreach (var presetData in priorityOrderedPresetDataList)
      {
        AddToDefaultPresets(presetData.Preset, presetData.Filter);
      }
    }

    [MenuItem("Tools/CitrioN/Common/Remove Empty Presets")]
    public static void RemoveEmptyPresets()
    {
      int presetsRemoved = 0;
      var allPresetTypes = Preset.GetAllDefaultTypes();
      List<DefaultPreset> presetsToRemove = new List<DefaultPreset>();
      foreach (var presetType in allPresetTypes)
      {
        bool assignNew = false;
        presetsToRemove.Clear();
        var presetsOfType = Preset.GetDefaultPresetsForType(presetType);

        for (int i = 0; i < presetsOfType.Length; i++)
        {
          var preset = presetsOfType[i];
          if (preset.preset == null)
          {
            ConsoleLogger.LogWarning($"Removed empty preset for {presetType.GetManagedTypeName()}.");
            presetsToRemove.Add(preset);
            assignNew = true;
            presetsRemoved++;
          }
        }
        if (assignNew)
        {
          var newPresets = new List<DefaultPreset>(presetsOfType);
          presetsToRemove.ForEach(p => newPresets.Remove(p));
          Preset.SetDefaultPresetsForType(presetType, newPresets.ToArray());
        }
      }

      if (presetsRemoved > 0)
      {
        ConsoleLogger.Log($"Removed {presetsRemoved} empty presets.");
      }
      else
      {
        ConsoleLogger.Log("No empty presets found.");
      }
    }
  }
}