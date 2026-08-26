using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  [CreateAssetMenu(fileName = "ScriptablePersistentData_",
                   menuName = "CitrioN/Common/ScriptableObjects/Data/ScriptablePersistentData")]
  public class ScriptablePersistentData : ScriptableObject
  {
    [SerializeField]
    public List<StringToGenericDataRelation<object>> data = new List<StringToGenericDataRelation<object>>();
  } 
}