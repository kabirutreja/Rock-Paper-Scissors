using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  public abstract class LocalizationBehaviour : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The LocalizationProvider to use for localizing.")]
    protected LocalizationProvider localizationProvider;

    protected virtual void OnEnable()
    {
      UnregisterForLanguageChange();
      RegisterForLanguageChange();
      Localize();
    }

    protected virtual void OnDisable()
    {
      UnregisterForLanguageChange();
    }

    protected virtual void OnDestroy()
    {
      UnregisterForLanguageChange();
    }

    protected virtual void RegisterForLanguageChange()
    {
      if (localizationProvider == null) { return; }
      localizationProvider.Initialize();
      localizationProvider.OnLanguageChanged += Localize;
    }

    //protected virtual void RegisterForLanguageChange(string newLanguage) { }

    //protected virtual void RegisterForLanguageChange(string oldLanguage, string newLanguage) { }

    protected virtual void UnregisterForLanguageChange()
    {
      if (localizationProvider == null) { return; }
      localizationProvider.OnLanguageChanged -= Localize;
    }

    //protected virtual void UnregisterForLanguageChange(string newLanguage) { }

    //protected virtual void UnregisterForLanguageChange(string oldLanguage, string newLanguage) { }

    public void AssignProvider(LocalizationProvider localizationProvider, bool overrideExisting)
    {
      if (this.localizationProvider == null || overrideExisting)
      {
        UnregisterForLanguageChange();
        this.localizationProvider = localizationProvider;
        RegisterForLanguageChange();
      }
    }

    //protected virtual void OnLanguageChanged() => Localize();

    //protected virtual void OnLanguageChanged(string newLanguage) => Localize();

    //protected virtual void OnLanguageChanged(string oldLanguage, string newLanguage) => Localize();

    public abstract void Localize();
  }
}