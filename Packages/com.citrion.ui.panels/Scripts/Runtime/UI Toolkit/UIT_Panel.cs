using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.UI.UIToolkit
{
  [HeaderInfo("\n\nUI Toolkit Panel:\nPanel that is opened and closed by controlling a 'UI Document' component.")]
#if UNITY_2021_1_OR_NEWER || LEGACY_UI_TOOLKIT
  [RequireComponent(typeof(UIDocument))]
#endif
  public class UIT_Panel : AbstractUIPanel
  {
    [Header("UI Toolkit")]
    [Space(5)]

    [SerializeField]
    [Tooltip("The UXML files that will be instantiated to make up the menu hierarchy.")]
    private List<VisualTreeAsset> menuTemplates = new List<VisualTreeAsset>();

    [SerializeField]
    [Tooltip("The style sheets to apply to the menu.")]
    private List<StyleSheet> styleSheets = new List<StyleSheet>();

#if UNITY_2021_1_OR_NEWER || LEGACY_UI_TOOLKIT
    [Tooltip("The reference to a UI Document component. " +
             "If left blank the menu will attempt to find it on this GameObject.")]
    protected UIDocument document;
#endif
    protected VisualElement root;

    public override bool IsOpen => Root == null ? false :
                                   Root.style.display == DisplayStyle.Flex ? true : false;

    public List<VisualTreeAsset> MenuTemplates
    {
      get => menuTemplates;
      set => menuTemplates = value;
    }

    public List<StyleSheet> StyleSheets
    {
      get => styleSheets;
      set => styleSheets = value;
    }

    public VisualElement Root { get => root; set => root = value; }

    protected override void Show(bool show)
    {
      Root?.Show(show);
      //ShowVisualElement(root, show);
    }

    protected void ShowVisualElement(VisualElement element, bool show)
    {
      if (element == null) { return; }
      element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    protected override void Init()
    {
      base.Init();
#if UNITY_2021_1_OR_NEWER || LEGACY_UI_TOOLKIT
      document = GetComponent<UIDocument>();
      Root = GetRoot();
#endif
      AddElements();
      CacheReferences();
      SetupElements();
      AddBindings();
    }

    protected virtual void SetupElements() { }

    protected virtual void AddElements()
    {
      foreach (var t in menuTemplates)
      {
        if (t == null) { continue; }
        var instance = t.Instantiate();
        Root?.Add(instance);

        #region Fix Template Container Settings
        // Since the first element is always the template container
        // with default settings we need to stretch it to the parent
        // and disable picking mode so overlays are possible
        // Everything in the hierarchy below will still be functional
        // based on their own settings
        instance.StretchToParent();
        instance.pickingMode = PickingMode.Ignore;
        #endregion
      }

      Root?.AddStyleSheets(styleSheets);
    }

    protected virtual VisualElement GetRoot() => document.rootVisualElement;

    //protected void OnDestroy()
    //{
    //  RemoveBindings();
    //}

    protected virtual void CacheReferences() { }

    protected virtual void AddBindings() { }

    protected virtual void RemoveBindings() { }

    [Button]
    public void AddStyleSheets(params StyleSheet[] styleSheets)
    {
      if (!Application.isPlaying) { return; }
      Root?.AddStyleSheets(styleSheets);
    }
  }
}