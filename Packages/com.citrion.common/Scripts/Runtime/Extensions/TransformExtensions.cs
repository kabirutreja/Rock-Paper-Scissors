using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  /// <summary>
  /// Transform extention methods
  /// </summary>
  public static class TransformExtensions
  {
    /// <summary>
    /// Copies the local position, rotation and scale values from the 
    /// specified Transform to the transform this method is called on.
    /// </summary>
    /// <param name="transform">The transform to copy the values to</param>
    /// <param name="copyFrom">The transform to copy the values from</param>
    public static void CopyTransformValues(this Transform transform, Transform copyFrom)
    {
      transform.position = copyFrom.position;
      transform.rotation = copyFrom.rotation;
      transform.localScale = copyFrom.localScale;
    }

    /// <summary>
    /// Finds and returns all child transforms of the transform this method is called on.
    /// </summary>
    /// <returns>An array of the children transforms</returns>
    public static Transform[] GetChildrenTransforms(this Transform transform, bool includeRoot = false)
    {
      int childCount = transform.childCount;
      Transform[] childTransforms = new Transform[includeRoot ? childCount + 1 : childCount];
      for (int i = 0; i < childCount; i++)
      {
        childTransforms[i] = transform.GetChild(i);
      }
      if (includeRoot)
      {
        childTransforms[childTransforms.Length - 1] = transform;
      }
      return childTransforms;
    }

    /// <summary>
    /// Finds and returns all children and subsequent 
    /// children transforms of the transform this method is called on.
    /// </summary>
    /// <returns>An array of the children transforms</returns>
    public static Transform[] GetNestedChildrenTransforms(this Transform transform, bool includeRoot = false)
    {
      List<Transform> transforms = new List<Transform>();
      List<Transform> nestedChilds = new List<Transform>(transform.GetChildrenTransforms());
      List<Transform> currentChilds = new List<Transform>();
      transforms.AddRange(nestedChilds);
      if (includeRoot)
      {
        transforms.Add(transform);
      }

      while (nestedChilds != null && nestedChilds.Count > 0)
      {
        currentChilds.Clear();
        foreach (var child in nestedChilds)
        {
          currentChilds.AddRange(child.GetChildrenTransforms());
        }
        transforms.AddRange(currentChilds);
        nestedChilds = new List<Transform>(currentChilds);
      }

      return transforms.ToArray();
    }

    /// <summary>
    /// Performs the provided action on all transforms in the array this method is used on.
    /// </summary>
    /// <param name="action">The action to perform on each transform</param>
    public static void PerformActionOnTransforms(this Transform[] transforms, Action<Transform> action)
    {
      if (action != null)
      {
        for (int i = 0; i < transforms.Length; i++)
        {
          action.Invoke(transforms[i]);
        }
      }
    }

    /// <summary>
    /// Destroys all direct children of the transform.
    /// </summary>
    public static void DestroyAllChildren(this Transform transform)
    {
      transform.GetChildrenTransforms()?.PerformActionOnTransforms((t) =>
      {
        GameObject.DestroyImmediate(t.gameObject);
      });
    }

    /// <summary>
    /// Finds and returns all components in children that match the specified parameters.
    /// </summary>
    /// <typeparam name="T">The <see cref="Behaviour"/> type to get</typeparam>
    /// <param name="includeDisabledComponents">Should disabled components be found?</param>
    /// <param name="includeDisabledGameObjects">Should disabled objects be checked?</param>
    /// <returns>An array of components that match the specifications</returns>
    public static T[] GetComponentsInChildren<T>(this Transform transform, bool includeDisabledComponents, bool includeDisabledGameObjects) where T : Behaviour
    {
      T[] components = transform.GetComponentsInChildren<T>(includeDisabledGameObjects);
      // Check if disabled components should be excluded
      if (includeDisabledComponents == false)
      {
        // Select only the enabled components
        components = components.Where(c => c.enabled).ToArray();
      }
      return components;
    }

    [SkipObfuscationRename]
    public static Component GetComponentInHierarchy(this Transform origin, Type type, string relativePath)
    {
      var child = origin.Find(relativePath);

      //Check if the child in the hierarchy was not found
      if (child == null)
      {
        // Make an array of the different child names in the hierarchy
        var hierarchyNames = relativePath.Split('/');
        int index = 0;

        for (int i = 0; i < hierarchyNames.Length; i++)
        {
          // Find the child in the hierarchy
          //child = origin.GetChild(int.Parse(hierarchyNames[i]));
          child = origin.Find(hierarchyNames[i]);

          // Check if the child exists
          if (child != null)
          {
            // Make the origin the child for which to find the next child
            origin = child;
          }
          else
          {
            // Store the last child name index used
            index = i;
            break;
          }
        }

        for (int i = index; i < hierarchyNames.Length; i++)
        {
          // Create a new object for the child
          child = new GameObject(hierarchyNames[i]).transform;
          // Parent the child to the origin
          child.SetParent(origin);
          // Use the child as the new origin
          origin = child;
        }
      }

      // Get the component on the child transform
      Component component = child.GetComponent(type);
      //Component component = child.gameObject.AddOrGetComponent(type);

      // Check if the required component was not found
      if (component == null)
      {
        // Add the missing component to the child transform
        component = child.gameObject.AddComponent(type);
      }
      return component;
    }

    [SkipObfuscationRename]
    public static string GetTransformPathInHierarchy(this Transform parent, Transform child)
    {
      List<string> names = new List<string>();
      //List<int> siblingIndices = new List<int>();

      do
      {
        if (child == parent)
        {
          break;
        }
        // Add the current transform to the list
        names.Add(child.name);
        //siblingIndices.Add(child.GetSiblingIndex());

        // Make the new current the parent of the current
        child = child.parent;
      } while (child != null);

      if (child != parent)
      {
        ConsoleLogger.LogWarning($"Invalid transform hierarchy path!");
        return string.Empty;
      }

      StringBuilder sb = new StringBuilder();

      // Iterate over all the transform names in reverse
      for (int i = names.Count - 1; i >= 0; i--)
      {
        // Append the transform name to the path
        sb.Append(names[i]);
        //sb.Append(siblingIndices[i]);
        if (i > 0)
        {
          sb.Append("/");
        }
      }

      return sb.ToString();
    }

    public static void AddMeshColliderToAllRenderersInHierarchy(this Transform transform)
    {
      if (transform.GetComponentInChildren<Collider>() == null)
      {
        // Add a convex MeshCollider to all MeshRenderers part of the item visuals
        Array.ForEach(transform.GetComponentsInChildren<MeshRenderer>(),
                      rend =>
                      {
                        var col = rend.gameObject.AddComponent<MeshCollider>();
                        col.convex = true;
                      });
      }
    }

    public static Transform GetRoot(this Transform transform, Transform searchUntil = null)
    {
      Transform root;

      do
      {
        root = transform;
        transform = transform.parent;

      } while (transform != null &&
              (searchUntil == null || transform != searchUntil));

      return root;
    }
  }
}