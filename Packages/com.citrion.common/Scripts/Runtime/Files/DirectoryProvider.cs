using UnityEngine;

namespace CitrioN.Common
{
  [System.Serializable]
  public class DirectoryProvider
  {
    [SerializeField]
    [Tooltip("The type of directory to provide:\n\n" +
             "1. Persistent Data Path\n" +
             "    The path to a persistent data directory\n\n" +
             "2. Data Path\n" +
             "    The path to the game data folder\n" +
             "    on the target device\n\n" +
             "3. Custom:\n" +
             "    A custom path that can be specified\n" +
             "    in the customDirectoryPath field")]
    protected DirectoryType directoryType;

    [SerializeField]
    [Tooltip("Custom directory path used when the\n" +
             "'Custom' directory type is used")]
    protected string customDirectoryPath = string.Empty;

    public string Path
    {
      get
      {
        return directoryType switch
        {
          DirectoryType.PersistentDataPath => Application.persistentDataPath,
          DirectoryType.DataPath => Application.dataPath,
          DirectoryType.Custom => customDirectoryPath,
          _ => string.Empty,
        };
      }
    }
  }
}