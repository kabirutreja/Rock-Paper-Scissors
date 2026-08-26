using CitrioN.Common;
using CitrioN.Common.Editor;
using CitrioN.StyleProfileSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class Preferences_SettingsMenuCreator_Templates : AbstractPreferences
  {
    [Header("UGUI")]
    [SerializeField]
    [Tooltip("The menu template for UGUI to duplicate")]
    protected SettingsMenu_UGUI menuTemplate;

    [SerializeField]
    [Tooltip("The menu layout prefab for UGUI to duplicate")]
    private GameObject menuLayoutTemplate;

    [SerializeField]
    [Tooltip("The style profile to duplicate")]
    private StyleProfile styleProfile;

    [SerializeField]
    [Tooltip("The collection of input element providers for UGUI to duplicate")]
    private InputElementProviderCollection_UGUI inputElementProviders;

    [Space(10)]
    [Header("UI Toolkit")]
    [SerializeField]
    [Tooltip("The menu template for UI Toolkit to duplicate")]
    private SettingsMenu_UIT menuTemplateUIToolkit;

    [SerializeField]
    [Tooltip("A list of UI Documents to make up the UI Toolkit menu")]
    private List<VisualTreeAsset> menuDocuments = new List<VisualTreeAsset>();

    [SerializeField]
    [Tooltip("A list of style sheets to apply to the UI Toolkit menu")]
    private List<StyleSheet> menuStyleSheets = new List<StyleSheet>();

    [SerializeField]
    [Tooltip("The UI Toolkit style profile to duplicate")]
    private StyleProfile styleProfile_UIT;

    [SerializeField]
    [Tooltip("The collection of input element providers for UI Toolkit to duplicate")]
    private InputElementProviderCollection_UIT inputElementProvidersUIToolkit;

    [SerializeField]
    [HideInInspector]
    private string appliedPresetsVersion = "0";

    public string AppliedPresetsVersion
    {
      get => appliedPresetsVersion;
      set => appliedPresetsVersion = value;
    }

    public SettingsMenu_UGUI MenuTemplate_UGUI
    {
      get => menuTemplate;
      set => menuTemplate = value;
    }

    public GameObject MenuLayoutTemplate_UGUI
    {
      get => menuLayoutTemplate;
      set => menuLayoutTemplate = value;
    }

    public StyleProfile StyleProfile
    {
      get => styleProfile;
      set => styleProfile = value;
    }

    public InputElementProviderCollection_UGUI InputElementProviders_UGUI
    {
      get => inputElementProviders;
      set => inputElementProviders = value;
    }

    public SettingsMenu_UIT MenuTemplate_UIT
    {
      get => menuTemplateUIToolkit;
      set => menuTemplateUIToolkit = value;
    }

    public List<VisualTreeAsset> MenuDocuments_UIT
    {
      get => menuDocuments;
      set => menuDocuments = value;
    }

    public List<StyleSheet> MenuStyleSheets_UIT
    {
      get => menuStyleSheets;
      set => menuStyleSheets = value;
    }

    public StyleProfile StyleProfile_UIT
    {
      get => styleProfile_UIT;
      set => styleProfile_UIT = value;
    }

    public InputElementProviderCollection_UIT InputElementProviders_UIT
    {
      get => inputElementProvidersUIToolkit;
      set => inputElementProvidersUIToolkit = value;
    }

    public GameObject InstantiateSettingsMenu_UGUI(SettingsCollection collection = null)
    {
      if (menuTemplate == null) { return null; }

      var obj = PrefabUtility.InstantiatePrefab(menuTemplate.gameObject) as GameObject;
      var menuComponent = obj.GetComponent<SettingsMenu_UGUI>();
      if (menuLayoutTemplate != null)
      {
        menuComponent.MenuTemplate = menuLayoutTemplate;
      }
      if (collection != null)
      {
        menuComponent.SettingsTemplate = collection;
      }
      return obj;
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
      AppliedPresetsVersion = "0";
      EditorUtility.RequestScriptReload();
    }

    [ContextMenu("Print Version")]
    public void PrintPresetVersion()
    {
      ConsoleLogger.Log($"Preset Version: {AppliedPresetsVersion}");
    }

    public override void Initialize()
    {

    }
  }
}