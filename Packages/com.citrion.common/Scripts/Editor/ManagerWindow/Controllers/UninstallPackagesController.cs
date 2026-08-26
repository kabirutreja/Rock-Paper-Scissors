using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "UninstallPackagesController_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Controller/UninstallPackagesController")]
  public class UninstallPackagesController : ScriptableVisualTreeAssetController
  {
    [SerializeField]
    [Tooltip("The packages group name that should be available for removal.")]
    protected string groupName;

    [SerializeField]
    [Tooltip("The uxml file to instantiate as the list item for the packages list.")]
    protected VisualTreeAsset listItemTemplate;

    [SerializeField]
    [Tooltip("The list of action to invoke when the package removal dialog was canceled.")]
    protected List<ScriptableAction> onCancelActions;

    public override void Setup(VisualElement root)
    {
      if (!string.IsNullOrEmpty(groupName) && listItemTemplate != null)
      {
        new UnityPackagesList(groupName, listItemTemplate, root, OnCancel);
      }
    }

    protected virtual void OnCancel()
    {
      if (onCancelActions != null && onCancelActions.Count > 0)
      {
        foreach (var a in onCancelActions)
        {
          if (a == null) { continue; }
          a.InvokeAction();
        }
      }
    }
  }
}