using CitrioN.SettingsMenuCreator;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  public abstract class LocalizationManipulator : Manipulator
  {
    [SerializeField]
    [Tooltip("The LocalizationProvider to use for localizing.")]
    protected LocalizationProvider localizationProvider;

    public LocalizationManipulator(LocalizationProvider provider)
    {
      localizationProvider = provider;
    }

    protected override void RegisterCallbacksOnTarget()
    {
      target.UnregisterCallback<AttachToPanelEvent>(RegisterForLanguageChange);
      target.UnregisterCallback<DetachFromPanelEvent>(UnregisterForLanguageChange);
      target.RegisterCallback<AttachToPanelEvent>(RegisterForLanguageChange);
      target.RegisterCallback<DetachFromPanelEvent>(UnregisterForLanguageChange);
      Localize();
    }

    protected override void UnregisterCallbacksFromTarget()
    {
      target.UnregisterCallback<AttachToPanelEvent>(RegisterForLanguageChange);
      target.UnregisterCallback<DetachFromPanelEvent>(UnregisterForLanguageChange);
    }

    protected virtual void RegisterForLanguageChange(AttachToPanelEvent evt)
    {
      if (localizationProvider == null) { return; }
      localizationProvider.Initialize();
      localizationProvider.OnLanguageChanged += Localize;
    }

    protected virtual void UnregisterForLanguageChange(DetachFromPanelEvent evt)
    {
      if (localizationProvider == null) { return; }
      localizationProvider.OnLanguageChanged -= Localize;
    }

    public void AssignProvider(LocalizationProvider localizationProvider, bool overrideExisting)
    {
      if (this.localizationProvider == null || overrideExisting)
      {
        UnregisterForLanguageChange(null);
        this.localizationProvider = localizationProvider;
        RegisterForLanguageChange(null);
      }
    }

    public abstract void Localize();
  } 
}
