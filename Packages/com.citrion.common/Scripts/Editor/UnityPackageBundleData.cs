using UnityEngine;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "UnityPackageBundleData_",
                   menuName = "CitrioN/Common/UnityPackageBundleData")]
  public class UnityPackageBundleData : ScriptableObjectItemData
  {
    [SerializeField]
    [UnityPackageObject]
    [Tooltip("The Unity package file to manage")]
    private Object packageAsset;

    public Object Package { get => packageAsset; protected set => packageAsset = value; }
  }
}