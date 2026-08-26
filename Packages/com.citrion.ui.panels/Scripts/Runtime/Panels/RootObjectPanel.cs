using CitrioN.Common;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Base class for UI panels that use the active state 
  /// of a <see cref="GameObject"/> to open and close.
  /// </summary>
  [HeaderInfo("\n\nRoot Object Panel:\nPanel that is using a 'GameObject' as its root. " +
              "Useful if only a part of an object's hierarchy should serve as the panel. " +
              "The panel is opened and closed by enabling and disabling the root object of the panel.")]
  public class RootObjectPanel : AbstractUIPanel
  {
    [SerializeField]
    [Tooltip("The object to serve as the root of the panel.")]
    private GameObject rootObject;

    public override bool IsOpen => RootObject.activeSelf;

    public GameObject RootObject
    {
      get
      {
        if (rootObject == null)
        {
          rootObject = new GameObject("Panel Root");
          rootObject.transform.SetParent(transform, false);

          var rectTransform = rootObject.AddComponent<RectTransform>();
          rectTransform.anchorMin = Vector3.zero;
          rectTransform.anchorMax = Vector3.one;
          rectTransform.offsetMin = Vector3.zero;
          rectTransform.offsetMax = Vector3.zero;
        }
        return rootObject;
      }
      set => rootObject = value;
    }

    protected override void Show(bool show)
    {
      rootObject.SetActive(show);
    }

    protected override void OnPanelOpened()
    {
      base.OnPanelOpened();

      if (rootObject != null)
      {
        var t = rootObject.GetComponentInChildren<RectTransform>();
        if (t != null)
        {
          t.RebuildHierarchyLayoutImmediate();
        }
      }
    }
  }
}