using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  [CreateAssetMenu(fileName = "ScriptableObjectItemData_PackageRemoval_",
                   menuName = "CitrioN/Common/ScriptableObjects/ScriptableObjectItemDatas/ScriptableObjectItemData_PackageRemoval")]
  public class ScriptableObjectItemData_PackageRemoval : ScriptableObjectItemData
  {
    [SerializeField]
    [Tooltip("The name of the package.")]
    private string packageName;

    [SerializeField]
    [Tooltip("The dependencies to also remove")]
    private List<string> dependencies = new List<string>();

    public string PackageName { get => packageName; set => packageName = value; }

    public List<string> Dependencies { get => dependencies; set => dependencies = value; }
  }
}