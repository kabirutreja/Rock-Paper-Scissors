using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  public abstract class ScriptableVisualTreeAssetController : ScriptableObject
  {
    public abstract void Setup(VisualElement root);
  }
}