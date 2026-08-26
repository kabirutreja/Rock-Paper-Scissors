using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [HideScriptField]
  [HeaderInfo("Helper script used to mark this object as the object to parent to when using the " +
              "ScriptableInputElementProvider_UGUI_FromPrefab. " +
              "This allows combining multiple prefabs to create more complex input elements. " +
              "An example would be to have a prefab for the input element that contains a label and " +
              "a container for the actual element to interact with such as a dropdown or slider. " +
              "Splitting this into two separate prefabs makes it much more modular and maintainable. " +
              "During the generation process those prefabs will be instantiated and parented appropriately " +
              "in the order they appear on the provider.")]
  public class ProviderAnchor : MonoBehaviour { }
}