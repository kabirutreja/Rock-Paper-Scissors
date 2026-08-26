using CitrioN.Common;
using CitrioN.StyleProfileSystem;
using CitrioN.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [AddTooltips]
  [HeaderInfo("Allows the creation or updating of a prefab for the settings menu. It will " +
              "add all the necessary input elements to the prefab so the menu can easily be worked on " +
              "during edit mode. This includes the additional of edit mode StyleProfile support.")]
  [CreateAssetMenu(fileName = "MenuWithInputElementsPrefabCreator_",
                 menuName = "CitrioN/Settings Menu Creator/Menu With Input Elements Prefab Creator")]
  public class MenuWithInputElementsPrefabCreator : ScriptableObject
  {
    [Header("Prefab")]

    [SerializeField]
    [Tooltip("Whether or not a new prefab variant should be created. " +
             "If false any overrides will be applied to the referenced prefab.")]
    protected bool createNewPrefabVariant = true;

    [SerializeField]
    [Tooltip("The layout prefab to use for modification or duplication.")]
    protected GameObject menuLayoutPrefab;

    [Header("Input Elements For Settings")]

    [SerializeField]
    [Tooltip("Should the added input elements be initialized? " +
             "Initialized elements will for example have their labels updated to " +
             "match the labels of their corresponding setting.")]
    protected bool initializeInputElements = true;

    [SerializeField]
    [Tooltip("The SettingsCollection to add the input elements for its settings to the menu layout prefab.")]
    protected SettingsCollection settingsCollection;

    [Header("Style Profile")]

    [SerializeField]
    [Tooltip("Whether the 'Register StyleListeners In Hierarchy (Edit Mode Only)' script should be attached to the prefab." +
             "This will enable all StyleListeners on the prefab to react to StyleProfile changes and allows proper visualization " +
             "of the menu's visuals.")]
    protected bool addEditModeStyleProfileListening = true;

    [SerializeField]
    [Tooltip("Whether the 'Apply Style Profile' script should be attached to the prefab." +
             "This will automatically apply the referenced StyleProfile to prompt all " +
             "StyleListeners that are enabled during edit mode to apply their modifications.")]
    protected bool addApplyStyleProfileScript = true;

    [SerializeField]
    [Tooltip("The StyleProfile to add to the 'Apply Style Profile' script. This only has an " +
             "effect if the 'Add Apply Style Profile' script is attached to the root of the prefab.")]
    protected StyleProfile styleProfile;

    public GameObject MenuLayoutPrefab
    {
      get => menuLayoutPrefab;
      set => menuLayoutPrefab = value;
    }

    public SettingsCollection SettingsCollection
    {
      get => settingsCollection;
      set => settingsCollection = value;
    }

    public bool CreateNewPrefabVariant
    {
      get => createNewPrefabVariant;
      set => createNewPrefabVariant = value;
    }

    protected bool InitializeInputElements
    {
      get => initializeInputElements;
      set => initializeInputElements = value;
    }
    public bool AddEditModeStyleProfileListening
    {
      get => addEditModeStyleProfileListening;
      set => addEditModeStyleProfileListening = value;
    }

    public bool AddApplyStyleProfileScript
    {
      get => addApplyStyleProfileScript;
      set => addApplyStyleProfileScript = value;
    }

    public StyleProfile StyleProfile
    {
      get => styleProfile;
      set => styleProfile = value;
    }

    public virtual GameObject CreatePrefab()
    {
      if (menuLayoutPrefab == null) { return null; }
      if (GameObjectExtensions.IsSceneObject(menuLayoutPrefab))
      {
        ConsoleLogger.LogWarning("The referenced object is a scene object which " +
                                 "does not work with the prefab creator script!");
        return null;
      }
      if (settingsCollection == null) { return null; }

      var layoutVariant = CreateNewPrefabVariant ? CreatePrefabVariant(menuLayoutPrefab) : menuLayoutPrefab;

      if (layoutVariant == null) { return null; }

      var instance = PrefabUtility.InstantiatePrefab(layoutVariant) as GameObject;

      if (instance == null) { return null; }

      //var assetPath = AssetDatabase.GetAssetPath(layoutVariant);

      //var prefabStageInstance = PrefabUtility.LoadPrefabContents(assetPath);

      //if (prefabStageInstance == null) { return; }

      var actionInvokers = instance.transform.GetComponentsInChildren<OnPrefabCreationActionInvoker>(true, true).ToList();

      if (actionInvokers != null)
      {
        foreach (var invoker in actionInvokers)
        {
          if (invoker == null) { continue; }
          invoker.InvokeAction();
        }
      }

      var secondActionInvokers = instance.transform.GetComponentsInChildren<OnPrefabCreationActionInvoker>(true, true);

      if (secondActionInvokers != null)
      {
        foreach (var invoker in secondActionInvokers)
        {
          if (invoker != null && !actionInvokers.Contains(invoker))
          {
            invoker.InvokeAction();
          }
        }
      }

      var addedGameObjects = AddElements(instance.GetComponentInChildren<RectTransform>());

      if (AddEditModeStyleProfileListening)
      {
        instance.AddOrGetComponent<RegisterStyleListenersInHierarchyInEditMode>();
      }

      if (AddApplyStyleProfileScript)
      {
        var applyStyleProfileScript = instance.AddOrGetComponent<ApplyStyleProfile>();
        if (styleProfile != null && applyStyleProfileScript != null)
        {
          applyStyleProfileScript.StyleProfile = styleProfile;
        }
      }

      PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);

      DestroyImmediate(instance);
      return layoutVariant;
      //PrefabUtility.ApplyAddedGameObjects(addedGameObjects.ToArray(), assetPath, InteractionMode.AutomatedAction);
    }

    protected static GameObject CreatePrefabVariant(GameObject source)
    {
      string folderPath = AssetDatabase.GetAssetPath(source);
      string extension = Path.GetExtension(folderPath);
      var pathWithoutExtension = FileUtility.RemoveFileExtension(folderPath);
      var newFolderPath = AssetDatabase.GenerateUniqueAssetPath($"{pathWithoutExtension}_WithInputElements{extension}");

      GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
      //var prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, $"{folderPath}/{source.name}_Variant.prefab");
      var prefabVariant = PrefabUtility.SaveAsPrefabAsset(instance, newFolderPath);
      GameObject.DestroyImmediate(instance);
      EditorUtility.SetDirty(prefabVariant);
      return prefabVariant;
    }

    protected List<GameObject> AddElements(RectTransform root)
    {
      var list = new List<GameObject>();
      if (root != null && SettingsCollection != null)
      {
        if (SettingsCollection.InputElementProviders_UGUI == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UGUI)} reference assigned!");
          return list;
        }
        foreach (var s in SettingsCollection.Settings)
        {
          if (s == null) { continue; }
          var elem = s.FindElement_UGUI(root, SettingsCollection);
          if (elem == null)
          {
            elem = s.CreateElement_UGUI(root, SettingsCollection);
          }

          if (elem != null)
          {
            if (InitializeInputElements)
            {
              s.InitializeElement_UGUI(elem, SettingsCollection, initialize: false);
            }
            list.Add(elem.gameObject);
          }
          else
          {
            ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
                         $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
          }
        }
      }
      return list;
    }
  }
}