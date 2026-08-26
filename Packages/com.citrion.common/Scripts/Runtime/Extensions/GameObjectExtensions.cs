using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CitrioN.Common
{
  public static class GameObjectExtensions
  {
    // TODO Is this method neccessary or does the regular GetComponentsInChildren offer the same functionality?
    public static T[] GetComponentsInChildrenOrdered<T>(this GameObject obj, bool includeInactive = false)
    {
      var components = new List<T>();
      Array.ForEach(obj.transform.GetChildrenTransforms(true), t =>
      {
        if (includeInactive ||
           (includeInactive == false && t.gameObject.activeInHierarchy))
        {
          var component = t.GetComponent<T>();
          if (component != null)
          {
            components.AddIfNotContains(component);
          }
        }
      });
      return components.ToArray();
    }

    public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
    {
      // TODO Copy code from old method
      return layerMask == (layerMask | (1 << obj.layer));
      //return true;
    }

    /// <summary>
    /// Inverts the active state of the GameObject.
    /// </summary>
    public static void ToggleActiveState(this GameObject obj)
    {
      obj.SetActive(!obj.activeSelf);
    }

    public static bool IsSceneObject(this GameObject obj)
    {
      if (obj == null) return false;
      return !string.IsNullOrEmpty(obj.scene.name);
    }

#if UNITY_EDITOR
    public static bool TryGetPrefab(this GameObject obj, out GameObject prefab)
    {
      // TODO Make this work for prefab variants in the hierarchy/scene

      // Set the prefab to null
      prefab = null;

      // Check if the object is of any type of prefab
      if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Regular ||
          PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Variant)
      {
        // Check if the object is currently in the scene
        if (obj.IsSceneObject())
        {
          // Get the prefab from the scene object
          prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
        }
        else { prefab = obj; }
      }
      return prefab != null;
    }

    public static bool IsPrefabAsset(GameObject obj)
    {
      return PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Regular ||
             PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Variant;
    }

    public static GameObject CreatePrefabAsset(this GameObject prefabSource, out GameObject sceneObject, string prefabName = "", string absolutePath = "", bool saveAsVariant = true)
    {
      if (saveAsVariant)
      {
        sceneObject = PrefabUtility.InstantiatePrefab(prefabSource) as GameObject;
      }
      else
      {
        // TODO Replace with new code
#pragma warning disable CS0618
        sceneObject = PrefabUtility.InstantiateAttachedAsset(prefabSource) as GameObject;
#pragma warning restore CS0618
      };

      if (string.IsNullOrEmpty(prefabName))
      {
        prefabName = prefabSource.name;
      }
      if (string.IsNullOrEmpty(absolutePath))
      {
        absolutePath = Application.dataPath;
      }
      // Set the path as within the Assets folder,
      // and name it as the GameObject's name with the .Prefab format
      string relativePath = absolutePath.Substring(absolutePath.LastIndexOf("Assets"));
      string localPath = relativePath + "/" + prefabName + ".prefab";
      // Make sure the file name is unique, in case an existing Prefab has the same name.
      localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
      return PrefabUtility.SaveAsPrefabAsset(sceneObject, localPath);
    }
#endif

    /// <summary>
    /// Finds and returns the first object of the provided type in the scene.
    /// </summary>
    /// <typeparam name="T">The type to find</typeparam>
    /// <returns>The first object of the provided type or null if none was found</returns>
    public static T GetFirstObjectOfType<T>(this GameObject obj) where T : MonoBehaviour
    {
#if UNITY_2023_1_OR_NEWER
      return GameObject.FindFirstObjectByType<T>();
#else
      var objectsOfType = GameObject.FindObjectsOfType<T>();
      if (objectsOfType != null)
      {
        if (objectsOfType.Length > 1)
        {
          string errorMsg = ("There are more than one of the requested object in the scene. There are: "
                            + objectsOfType.Length).Colorize(Color.red);
          Debug.LogWarning(errorMsg);
        }
        if (objectsOfType.Length > 0)
        {
          return objectsOfType[0];
        }
      }
      return null;
#endif
    }

    public static bool IsObjectInFront(this GameObject obj, GameObject objectToFindRelativePositionOf)
    {
      Vector3 localStationaryPosition = obj.transform.InverseTransformVector(obj.transform.position);
      Vector3 localObjectPosition = obj.transform.InverseTransformVector(objectToFindRelativePositionOf.transform.position);
      return (localStationaryPosition.z < localObjectPosition.z);
    }

    public static List<CardinalDirection> RelativePositionTo(this GameObject obj, GameObject objectToFindRelativePositionOf)
    {
      List<CardinalDirection> relativePositions = new List<CardinalDirection>();
      Vector3 localStationaryPosition = obj.transform.InverseTransformVector(obj.transform.position);
      Vector3 localObjectPosition = obj.transform.InverseTransformVector(objectToFindRelativePositionOf.transform.position);

      if (localStationaryPosition.z < localObjectPosition.z) relativePositions.Add(CardinalDirection.North);
      if (localStationaryPosition.z > localObjectPosition.z) relativePositions.Add(CardinalDirection.South);
      if (localStationaryPosition.x < localObjectPosition.x) relativePositions.Add(CardinalDirection.East);
      if (localStationaryPosition.x > localObjectPosition.x) relativePositions.Add(CardinalDirection.West);

      return relativePositions;
    }

    /// <summary>
    /// Finds all objects in range.
    /// Use this method with caution as it may affect performance significantly due to
    /// the necessity to check all GameObjects in the scenes.
    /// </summary>
    /// <param name="obj">The object to find all objects in range for</param>
    /// <param name="validLayers">LayerMask to only include objects on layers present in the mask. 
    /// Use -1 if you want all layer to be included.</param>
    /// <param name="range">The range to find valid object within</param>
    /// <returns>List of valid GameObjects in range.</returns>
    public static List<GameObject> GetObjectsInRange(this GameObject obj, LayerMask validLayers, float range)
    {
#if UNITY_2023_1_OR_NEWER
      GameObject[] allSceneObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
      GameObject[] allSceneObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
#endif
      List<GameObject> objectsInRange = new List<GameObject>();
      for (int i = 0; i < allSceneObjects.Length; i++)
      {
        var sceneObject = allSceneObjects[i];
        if (sceneObject.IsInLayerMask(validLayers))
        {
          if (Vector3.Distance(sceneObject.transform.position, obj.transform.position) <= range)
          {
            objectsInRange.Add(sceneObject);
          }
        }
      }
      return objectsInRange;
    }

    public static Bounds GetHierarchyMeshBounds(this GameObject obj, bool includeChildren = true)
    {
      var renderers = new List<MeshFilter>();

      if (includeChildren)
      {
        var childs = obj.transform.GetNestedChildrenTransforms(includeRoot: true);
        Array.ForEach(childs, c =>
        {
          foreach (var rend in c.GetComponents<MeshFilter>())
          {
            renderers.AddIfNotContains(rend);
          }
        });
      }
      else
      {
        foreach (var rend in obj.GetComponents<MeshFilter>())
        {
          renderers.AddIfNotContains(rend);
        }
      }

      Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

      {
        if (renderers.Count > 0)
        {
          for (int i = 0; i < renderers.Count; i++)
          {
            bounds.Encapsulate(renderers[i].sharedMesh.bounds);
          }
        }
      }
      return bounds;
    }

    public static Bounds GetHierarchyRendererBounds(this GameObject obj, bool includeChildren = true, bool moveToWorldOrigin = false,
                                                    bool includeDisabledRenderers = false)
    {
      var previousParent = obj.transform.parent;
      obj.transform.SetParent(null);
      var renderers = new List<Renderer>();

      if (includeChildren)
      {
        var childs = obj.transform.GetNestedChildrenTransforms(includeRoot: true);
        Array.ForEach(childs, c =>
        {
          foreach (var rend in c.GetComponents<Renderer>())
          {
            if (includeDisabledRenderers || (rend.enabled && rend.gameObject.activeSelf))
            {
              renderers.AddIfNotContains(rend);
            }
          }
        });
      }
      else
      {
        foreach (var rend in obj.GetComponents<Renderer>())
        {
          if (includeDisabledRenderers || rend.enabled)
          {
            renderers.AddIfNotContains(rend);
          }
        }
      }

      List<Renderer> disabledRenderers = null;
      if (includeDisabledRenderers)
      {
        // Cache all the disabled renderers so they can be disabled again after the bounds calculcation
        disabledRenderers = renderers.Where(r => r.enabled == false).ToList();
        disabledRenderers.ForEach(r => r.enabled = true);
      }

      Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);

      //if (renderers.Count > 0)
      {
        // Cache the position & rotation
        var originalPosition = obj.transform.position;
        var originalRotation = obj.transform.rotation;

        if (moveToWorldOrigin)
        {
          // Move the object to the worlds origin
          obj.transform.position = Vector3.zero;
          // Clear the rotation
          obj.transform.rotation = Quaternion.identity;
          // Recreate the bounds after moving the object to the worlds origin
          bounds = new Bounds(obj.transform.position, Vector3.zero);
        }

        //if (renderers.Count > 0)
        //{
        //  // TODO Should this use the bounds center instead of the transform position?
        //  //bounds = new Bounds(obj.transform.position, renderers[0].bounds.size);
        //  bounds = renderers[0].bounds;
        //  //return renderers[0].bounds;
        //}
        if (renderers.Count > 0)
        {
          //Bounds totalBounds = renderers[0].bounds;
          //Bounds totalBounds = new Bounds(obj.transform.TransformPoint(renderers[0].bounds.center), renderers[0].bounds.size);
          //Bounds totalBounds = new Bounds(renderers[0].gameObject.transform.InverseTransformPoint(renderers[0].bounds.center), renderers[0].bounds.size);
          //Bounds totalBounds = new Bounds(obj.transform.position, Vector3.zero);
          //Bounds totalBounds = renderers[0].bounds;
          //Bounds totalBounds = new Bounds(obj.transform.position, renderers[0].bounds.size);
          //Debug.Log("Bounds (" + renderers[0].name + "): " + renderers[0].bounds);
          for (int i = 0; i < renderers.Count; i++)
          {
            //Debug.Log("Bounds (" + renderers[i].name + "): " + renderers[i].bounds);
            bounds.Encapsulate(renderers[i].bounds);
          }
          //Debug.Log("===");
          //bounds = totalBounds;
          //return totalBounds;
        }

        if (moveToWorldOrigin)
        {
          // Restore the original position & rotation
          obj.transform.position = originalPosition;
          obj.transform.rotation = originalRotation;
        }
      }
      obj.transform.SetParent(previousParent);
      disabledRenderers?.ForEach(r => r.enabled = false);
      return bounds;
    }

    public static Bounds GetHierarchyColliderBounds(this GameObject obj, bool moveToWorldOrigin = true)
    {
      return obj.GetHierarchyColliderBounds(-1, moveToWorldOrigin);
    }

    public static Bounds GetHierarchyColliderBounds(this GameObject obj, LayerMask validLayers, bool moveToWorldOrigin = true)
    {
      Collider[] colliders = obj.GetComponentsInChildren<Collider>();
      colliders = Array.FindAll(colliders, c => c.gameObject.IsInLayerMask(validLayers));
      if (colliders == null || colliders.Length == 0)
      {
        return new Bounds(obj.transform.position, Vector3.zero);
      }

      var previousParent = obj.transform.parent;
      obj.transform.SetParent(null);

      Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);

      // Cache the position & rotation
      var originalPosition = obj.transform.position;
      var originalRotation = obj.transform.rotation;

      if (moveToWorldOrigin)
      {
        // Disable this if bounds calculation have actually changed within Unity
        // Move the object to the worlds origin
        obj.transform.position = Vector3.zero;
        // Clear the rotation
        obj.transform.rotation = Quaternion.identity;

        bounds = new Bounds(obj.transform.position, Vector3.zero);

        UnityEngine.Physics.SyncTransforms();
      }

      if (colliders.Length > 0)
      {
        for (int i = 0; i < colliders.Length; i++)
        {
          bounds.Encapsulate(colliders[i].bounds);
        }
      }

      if (moveToWorldOrigin)
      {
        // Restore the original position & rotation
        obj.transform.position = originalPosition;
        obj.transform.rotation = originalRotation;

        UnityEngine.Physics.SyncTransforms();
      }

      obj.transform.SetParent(previousParent);
      return bounds;
    }

    public static float GetVolumeInLiter(this GameObject obj, ObjectShape shape, BoundsCalculationMethod method)
    {
      return obj.GetVolumeOfShape(shape, method) * 1000;
    }

    public static float GetVolumeOfShape(this GameObject obj, ObjectShape shape, BoundsCalculationMethod method)
    {
      var bounds = new Bounds();
      float volume = 0;

      switch (method)
      {
        case BoundsCalculationMethod.Renderer:
          bounds = obj.GetHierarchyRendererBounds();
          break;
        case BoundsCalculationMethod.Colliders:
          bounds = obj.GetHierarchyColliderBounds();
          break;
      }

      switch (shape)
      {
        case ObjectShape.Rectangular:
          volume = volume = bounds.size.x * bounds.size.z * bounds.size.y;
          break;
        case ObjectShape.Sphere:
          volume = 4 / 3 * Mathf.PI * Mathf.Pow(bounds.extents.x, 3f);
          break;
        case ObjectShape.Capsule:
          volume = 4 / 3 * Mathf.PI * Mathf.Pow(bounds.extents.x, 3f) +
                   Mathf.PI * Mathf.Pow(bounds.extents.x, 2f) * bounds.size.y;
          break;
        case ObjectShape.Cone:
          volume = 1 / 3 * Mathf.PI * Mathf.Pow(bounds.extents.x, 2f) * bounds.size.y;
          break;
      }

      return volume;
    }

    /// <summary>
    /// Applies a random offset to the GameObject's world space position.
    /// </summary>
    /// <param name="obj">The object to apply the position offset to</param>
    /// <param name="offsetMin">The values for the minimum offset along each axis</param>
    /// <param name="offsetMax">The values for the maximum offset along each axis</param>
    public static void ApplyPositionalOffset(this GameObject obj, Vector3 offsetMin, Vector3 offsetMax)
    {
      var currentPosition = obj.transform.position;
      Vector3 offset = new Vector3(UnityEngine.Random.Range(offsetMin.x, offsetMax.x),
                                   UnityEngine.Random.Range(offsetMin.y, offsetMax.y),
                                   UnityEngine.Random.Range(offsetMin.z, offsetMax.z));
      currentPosition += offset;
      obj.transform.position = currentPosition;
    }

    public static void ApplyPositionalOffset(this GameObject obj, Vector3 offsetMin, Vector3 offsetMax, ref UnityEngine.Random.State state)
    {
      UnityEngine.Random.state = state;
      obj.ApplyPositionalOffset(offsetMin, offsetMax);
      state = UnityEngine.Random.state;
    }

    public static void ApplyPositionalOffset(this GameObject obj, Vector3 offsetMin, Vector3 offsetMax, int seed)
    {
      UnityEngine.Random.InitState(seed);
      obj.ApplyPositionalOffset(offsetMin, offsetMax);
    }

    public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
    {
      if (obj == null) { return null; }
      var component = obj.GetComponent<T>();
      if (component == null)
      {
        component = obj.AddComponent<T>();
      }
      return component;
    }

    public static Component AddOrGetComponent(this GameObject obj, Type type)
    {
      if (type != null && typeof(Component).IsAssignableFrom(type))
      {
        var component = obj.GetComponent(type);
        if (component == null)
        {
          component = obj.AddComponent(type);
        }
        return component;
      }
      ConsoleLogger.LogWarning($"Type {type} needs to inherit from {typeof(Component)}");
      return null;
    }
  }
}