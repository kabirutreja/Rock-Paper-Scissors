using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  public abstract class LanguageChanger : MonoBehaviour
  {
    [SerializeField]
    protected string language;

    public abstract void ChangeLanguage(string language);

    [ContextMenu("Change Language")]
    public void ChangeLanguage()
    {
      if (string.IsNullOrEmpty(language)) { return; }
      ChangeLanguage(language);
    }
  }
}