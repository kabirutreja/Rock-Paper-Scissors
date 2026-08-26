using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [HeaderInfo("\n\nSaves the setting values to a single player prefs entry.")]
  [CreateAssetMenu(fileName = "SettingsSaver_PlayerPrefs_",
               menuName = "CitrioN/Settings Menu Creator/Settings Saver/PlayerPrefs",
               order = 21)]

  public class SettingsSaver_PlayerPrefs : SettingsSaver
  {
    [SerializeField]
    [Tooltip("The player prefs key for the settings.")]
    protected string playerPrefsKey = "Settings";

    public override void SaveSettings(SettingsCollection collection)
    {
      SaveSettingsInternal(collection);
    }

    protected virtual void SaveSettingsInternal(SettingsCollection collection)
    {
      if (collection == null)
      {
        ConsoleLogger.LogError($"A collection is required to save its settings!");
        return;
      }

      var currentData = AppendData ? LoadSettingsInternal() : new Dictionary<string, object>();
      var saveString = XmlUtility_Settings.GetSaveString(collection, currentData);

      PlayerPrefs.SetString(playerPrefsKey, saveString);
      PlayerPrefs.Save();
    }

    public override Dictionary<string, object> LoadSettings()
    {
      return LoadSettingsInternal();
    }

    protected virtual Dictionary<string, object> LoadSettingsInternal()
    {
      if (PlayerPrefs.HasKey(playerPrefsKey))
      {
        var text = PlayerPrefs.GetString(playerPrefsKey);
        var settingsData = XmlUtility_Settings.LoadFromText(text);

        return settingsData;
      }
      return null;
    }

    public override void DeleteSave()
    {
      RemovePlayerPrefsSettingsKey();
    }

    [ContextMenu("Remove key")]
    public void RemovePlayerPrefsSettingsKey()
    {
      RemovePlayerPrefsKey(playerPrefsKey);
    }

    protected void RemovePlayerPrefsKey(string key)
    {
      if (PlayerPrefs.HasKey(key))
      {
        PlayerPrefs.DeleteKey(key);
        ConsoleLogger.Log($"Successfully deleted PlayerPrefs key: {key}");
      }
      else
      {
        ConsoleLogger.Log($"PlayerPrefs do not have the key to delete: {key}");
      }
    }
  }
}