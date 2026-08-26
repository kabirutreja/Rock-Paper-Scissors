using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [SkipObfuscationRename]
  [HeaderInfo("Handles the saving and loading of setting values.")]
  public abstract class SettingsSaver : ScriptableObject
  {
    [SerializeField]
    [Tooltip("Whether the save data should be added to or override existing data.")]
    protected bool appendData = false;

    public bool AppendData { get => appendData; set => appendData = value; }

    [SkipObfuscationRename]
    public abstract void SaveSettings(SettingsCollection collection);

    [SkipObfuscationRename]
    public abstract Dictionary<string, object> LoadSettings();

    public abstract void DeleteSave();
  }
}