using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T instance;

    [SkipObfuscationRename]
    public static T Instance
    {
      get
      {
        if (ApplicationIsQuitting || !Application.isPlaying)
        {
          return null;
        }

        if (instance == null)
        {
#if UNITY_2023_1_OR_NEWER
          instance = (T)FindAnyObjectByType(typeof(T));
#else
          instance = (T)FindObjectOfType(typeof(T));
#endif

          if (instance == null)
          {
            GameObject singleton = new GameObject();
            instance = singleton.AddComponent<T>();
            singleton.name = "[Singleton (Instantiated)] " + typeof(T).Name.ToString();

            if (Application.isPlaying)
            {
              DontDestroyOnLoad(singleton);
            }

            ConsoleLogger.Log($"Created new singleton of type {typeof(T)}".Colorize(Color.magenta), LogType.Always);
          }
          else
          {
            if (instance.transform.parent != null)
            {
              instance.transform.SetParent(null);
            }
            if (Application.isPlaying)
            {
              DontDestroyOnLoad(instance.gameObject);
            }
          }
        }

        return instance;
      }
    }

    protected static bool ApplicationIsQuitting => ApplicationQuitListener.isQuitting;

    [SkipObfuscationRename]
    protected virtual void OnDestroy()
    {
      // If the application is not quitting don't update any variables
      // This could be the case if a scene change happens and for some odd reason
      // the OnDestroy method is called even tho the gameObject is not really destroyed?!
      if (ApplicationIsQuitting)
      {
        if (instance == this)
        {
          instance = null;
        }
      }
    }

    [SkipObfuscationRename]
    protected virtual void Awake()
    {
      name = "[Singleton] " + name;
      transform.SetParent(null);

      // Check if the instance is not set
      if (instance == null)
      {
        // Get and assign singleton instance from this gameobjects components
        instance = GetComponent<T>();

        // Check if the gameobject is a child of another gameobject.
        // If it is a child unparent it so the DontDestoryOnLoad method can function properly.
        if (instance?.gameObject.transform.parent != null) { instance?.gameObject.transform.SetParent(null); }

        DontDestroyOnLoad(gameObject);
      }
      // Check if instance is this singleton instance
      else if (instance == this)
      {
        // Check if the gameobject is a child of another gameobject.
        // If it is a child unparent it so the DontDestoryOnLoad method can function properly.
        if (gameObject.transform.parent != null) gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        ConsoleLogger.Log($"Created new singleton of type {typeof(T)}".Colorize(Color.magenta), LogType.Always);
      }
      else
      {
        // Destroy this gameobject if the instance was already assigned and not this instance
        DestroyImmediate(gameObject);
      }
    }
  }
}