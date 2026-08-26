using CitrioN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  public abstract class LocalizationProvider : ScriptableObject
  {
    public Action OnLanguageChanged;

    private static HashSet<LocalizationProvider> initializedProviders = new();

    public static bool IsInitialized(LocalizationProvider localizationProvider)
      => initializedProviders.Contains(localizationProvider);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      initializedProviders.Clear();
      GlobalEventHandler.AddEventListener("OnApplicationQuit", OnQuit);
    }

    private static void OnQuit()
    {
      var list = initializedProviders.ToList();
      for (int i = 0; i < list.Count; i++)
      {
        list[i].UnregisterFromOnLanguageChangedEvent();
      }
      initializedProviders.Clear();
      GlobalEventHandler.RemoveEventListener("OnApplicationQuit", OnQuit);
    }

    public void Initialize()
    {
      RegisterForOnLanguageChangedEvent();
    }

    public abstract string Localize(string localizationKey);

    public abstract List<string> Localize(List<string> localizationKeys);

    public virtual void RegisterForOnLanguageChangedEvent()
    {
      if (IsInitialized(this)) { return; }
      UnregisterFromOnLanguageChangedEvent();
      initializedProviders.Add(this);
      // Implement registering for the language change listening here
    }

    public virtual void UnregisterFromOnLanguageChangedEvent()
    {
      initializedProviders.Remove(this);
      // Implement unregistering for the language change listening here
    }

    public void OnLanguageChange()
    {
      OnLanguageChanged?.Invoke();
    }
  }
}