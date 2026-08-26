using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  [CreateAssetMenu(fileName = "ScriptableVisualTreeAssetCollection_",
               menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Collection")]
  public class ScriptableVisualTreeAssetCollection : ScriptableObject
  {
    public List<ScriptableVisualTreeAsset> assets = new List<ScriptableVisualTreeAsset>();
  }
}