using UnityEngine;

namespace CitrioN.Common
{
  [CreateAssetMenu(fileName = "ScriptableObjectItemData_String_",
                   menuName = "CitrioN/Common/ScriptableObjects/ScriptableObjectItemDatas/ScriptableObjectItemData_String")]
  public class ScriptableObjectItemData_String : ScriptableObjectItemData
  {
    [SerializeField]
    [Tooltip("The string value for this item.")]
    private string stringValue;

    public string StringValue { get => stringValue; set => stringValue = value; }
  }
}