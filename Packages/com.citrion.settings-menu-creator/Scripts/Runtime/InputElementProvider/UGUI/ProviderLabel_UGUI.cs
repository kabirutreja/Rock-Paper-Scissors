using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  /// <summary>
  /// Marks an object as a provider label.
  /// Requires either a <see cref="Text"/> or a <br/>
  /// <see cref="TextMeshProUGUI"/> component attached
  /// to the same object to have its text modified.
  /// </summary>
  [HideScriptField]
  [HeaderInfo("Marks this object to be used as the label for an input element. " +
              "Used by 'Input Element Providers' to set the label text for the input element. " +
              "Requires a Text or TextMeshProUGUI script to be attached to this object.")]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Provider Label (UGUI)")]
  public class ProviderLabel_UGUI : MonoBehaviour
  {
    public void SetLabelText(string text)
    {
#if TEXT_MESH_PRO
      if (TryGetComponent<TMPro.TextMeshProUGUI>(out var textComponent_TMP)) textComponent_TMP.SetText(text);
#endif
      if (TryGetComponent<Text>(out var textComponent)) textComponent.text = text;
    }
  }
}
