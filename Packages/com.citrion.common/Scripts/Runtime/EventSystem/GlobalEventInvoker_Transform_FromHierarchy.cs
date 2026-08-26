using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscation]
  [HeaderInfo("\n\nAllows the specification of a dynamic transform relative to this objects hierarchy to be used as the parameter.")]
  public class GlobalEventInvoker_Transform_FromHierarchy : GlobalEventInvoker_Transform
  {
    [SerializeField]
    [Tooltip("The type of the object to use as the transform parameter.\n\n" +
             "This: Uses this object's transform\n" +
             "Parent: Uses this object's parent transform\n" +
             "Child: Uses this object's first child transform\n" +
             "Root: Uses this objects root transform\n" +
             "Custom: Uses the referenced transform")]
    protected HierarchyObjectType objectType = HierarchyObjectType.This;

    public override Transform Argument
    {
      get
      {
        switch (objectType)
        {
          case HierarchyObjectType.This:
            return transform;
          case HierarchyObjectType.Parent:
            return transform.parent;
          case HierarchyObjectType.Child:
            var child = transform.GetChild(0);
            if (child != null) { return child.transform; }
            return null;
          case HierarchyObjectType.Root:
            return transform.root;
          case HierarchyObjectType.Custom:
            return argument;
        }
        return null;
      }
    }
  } 
}