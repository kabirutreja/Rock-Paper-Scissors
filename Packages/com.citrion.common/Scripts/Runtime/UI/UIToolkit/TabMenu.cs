using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  public enum TabNavigationMode_UIT
  {
    HorizontalTop,
    HorizontalBottom,
    VerticalLeft,
    VerticalRight
  }

  [SkipObfuscation]
#if UNITY_2023_2_OR_NEWER
  [UxmlElement]
#endif
  public partial class TabMenu : VisualElement
  {
    #region Constants
    private const string rootClassName = "tab-menu";

    private const string tabsContainerClassName = "tab-menu__tabs-container";
    private const string tabTemplateClassName = "tab-menu__tab-template";
    private const string tabsTemplateContainerClassName = "tab-menu__tabs-template-container";
    private const string tabContentElementContainerClassName = "tab-menu__tabs-content-container__element-container";

    private const string tabClassName = "tab-menu__tab";
    private const string tabFirstClassName = "tab-menu__tab--first";
    private const string tabLastClassName = "tab-menu__tab--last";
    private const string tabSelectedClassName = "tab-menu__tab__selected";
    private const string tabDeselectedClassName = "tab-menu__tab__deselected";
    private const string tabLabelClassName = "tab-menu__tab__label";

    private const string tabsContentContainerClassName = "tab-menu__tabs-content-container";
    #endregion

#if !UNITY_2023_2_OR_NEWER
    public new class UxmlFactory : UxmlFactory<TabMenu, UxmlTraits> { }
#endif

#if !UNITY_2023_2_OR_NEWER
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
      UxmlIntAttributeDescription currentTabIndex = new UxmlIntAttributeDescription { name = "active-tab", defaultValue = 0 };
      UxmlEnumAttributeDescription<TabNavigationMode_UIT> tabNavigation = new UxmlEnumAttributeDescription<TabNavigationMode_UIT>
      {
        name = "tab-navigation",
        defaultValue = TabNavigationMode_UIT.HorizontalTop
      };
      UxmlBoolAttributeDescription autoSelectOnTabNavigation = new UxmlBoolAttributeDescription
      {
        name = "auto-select-on-tab-navigation",
        defaultValue = true
      };
      UxmlStringAttributeDescription tabNames =
        new UxmlStringAttributeDescription { name = "tab-names", defaultValue = "Tab 1, Tab 2, Tab 3, Tab 4, Tab 5" };

      public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
      {
        get { yield break; }
      }

      public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
      {
        base.Init(ve, bag, cc);
        var menu = ve as TabMenu;
        menu.SelectedTabIndex = currentTabIndex.GetValueFromBag(bag, cc);
        menu.TabNames = tabNames.GetValueFromBag(bag, cc)?.Split(',');
        menu.TabNavigation = tabNavigation.GetValueFromBag(bag, cc);
        menu.AutoSelectOnTabNavigation = autoSelectOnTabNavigation.GetValueFromBag(bag, cc);
        //menu.CheckForTabs();
      }
    }
#endif

    #region Fields
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    public TabNavigationMode_UIT tabNavigation = TabNavigationMode_UIT.HorizontalTop;
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    private bool autoSelectOnTabNavigation = true;

    private VisualElement tabsContainer;
    private VisualElement tabsTemplateContainer;
    private VisualElement tabsContentContainer;

    protected Action<int> onTabChanged;

    [SerializeField]
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    public int selectedTabIndex = -1;

#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    public string[] tabNames = null;
    #endregion

    #region Properties
    public int TabsCount => tabsContainer != null ? tabsContainer.childCount : 0;

    public int TabsContentCount => tabsContentContainer?.Children() != null ? tabsContentContainer.Children().Count() : 0;

    public List<VisualElement> Tabs => tabsContainer?.Children()?.ToList();

    public List<VisualElement> TabsContent => tabsContentContainer?.Children()?.ToList();

    public override VisualElement contentContainer => tabsContentContainer;

    protected int SelectedTabIndex { get => selectedTabIndex; set => selectedTabIndex = value; }

    protected string[] TabNames { get => tabNames; set => tabNames = value; }

    protected TabNavigationMode_UIT TabNavigation { get => tabNavigation; set => tabNavigation = value; }

    public bool AutoSelectOnTabNavigation
    {
      get => autoSelectOnTabNavigation;
      set => autoSelectOnTabNavigation = value;
    }

    public VisualElement TabsContainer { get => tabsContainer; }
    #endregion

    public TabMenu()
    {
      AddToClassList(rootClassName);
      //this.SetFlexGrow(1);

      tabsTemplateContainer = new VisualElement();
      tabsTemplateContainer.name = tabsTemplateContainerClassName;
      hierarchy.Add(tabsTemplateContainer);
      tabsTemplateContainer.AddToClassList(tabsTemplateContainerClassName);
      tabsTemplateContainer.style.display = DisplayStyle.None;

      tabsContainer = new VisualElement();
      tabsContainer.name = tabsContainerClassName;
      hierarchy.Add(tabsContainer);
      tabsContainer.AddToClassList(tabsContainerClassName);

      tabsContentContainer = new VisualElement();
      tabsContentContainer.name = tabsContentContainerClassName;
      hierarchy.Add(tabsContentContainer);
      tabsContentContainer.AddToClassList(tabsContentContainerClassName);

      //tabsContentContainer.RegisterCallback<GeometryChangedEvent>(HandleContentChanged);
      //contentContainer.RegisterCallback<AttachToPanelEvent>(HandleAttachedToPanel);
      //contentContainer.RegisterCallback<DetachFromPanelEvent>(HandleDetachedFromPanel);

      #region Delayed CheckForTabs
      // We delay the CheckForTabs method call by a frame
      // so the entire tab menu layout was able to be setup
      // and processed. This allows proper (re-)parenting to
      // happen afterwards.

      //#if UNITY_EDITOR
      //      if (!Application.isPlaying)
      //      {
      //        UnityEditor.EditorApplication.delayCall += ReparentTabTemplates;
      //        UnityEditor.EditorApplication.delayCall += CheckForTabs;
      //      }
      //      else
      //      {
      //        CoroutineRunner.Instance.InvokeDelayedByFrames(ReparentTabTemplates);
      //        CoroutineRunner.Instance.InvokeDelayedByFrames(CheckForTabs);
      //      }
      //#else
      //      CoroutineRunner.Instance.InvokeDelayedByFrames(ReparentTabTemplates);
      //      CoroutineRunner.Instance.InvokeDelayedByFrames(CheckForTabs);
      //#endif

      //ReparentTabTemplates();
      //CheckForTabs();
      this.Show(false);
      ScheduleUtility.InvokeDelayedByFrames(ReparentTabTemplates);
      ScheduleUtility.InvokeDelayedByFrames(CheckForTabs);
      ScheduleUtility.InvokeDelayedByFrames(() => this.Show(true));
      #endregion
    }

    private void ReparentTabTemplates()
    {
      // Find all elements that represent a tab template
      var tabTemplates = TabsContent.FindAll(c => c.Q(className: tabTemplateClassName) != null);
      // Reparent all tab templates to the tab templates container
      for (int i = 0; i < tabTemplates.Count; i++)
      {
        var t = tabTemplates[i];
        if (t.parent != tabsTemplateContainer)
        {
          tabsTemplateContainer.Add(t);
        }
        //tabTemplates.Remove(t);
      }
    }

    //private void HandleDetachedFromPanel(DetachFromPanelEvent evt)
    //{
    //  var panel = evt.destinationPanel;
    //  ConsoleLogger.Log($"Detached from panel {panel?.visualTree.name}");
    //}

    //private void HandleAttachedToPanel(AttachToPanelEvent evt)
    //{
    //  var panel = evt.destinationPanel;
    //  ConsoleLogger.Log($"Attached to panel {panel?.visualTree.name}");
    //}

    private void CheckForTabs()
    {
      //return;

      //if (tabsContainer.childCount > 0) { return; }
      // Old - TODO Might need reenabling
      //tabsContainer.Clear();
      var tabsContent = TabsContent;

      for (int i = 0; i < tabsContent.Count; i++)
      {
        var content = tabsContent[i];
        content.Show(false);
        if (!content.ClassListContains(tabContentElementContainerClassName))
        {
          // Add class to the content element root
          content.AddToClassList(tabContentElementContainerClassName);
        }

        var tabName = TabNames != null && TabNames.Length > i ? TabNames[i] : $"Tab {i + 1}";
        if (TabsCount <= i)
        {
          AddTab(CreateTab(tabName), i);
        }
      }

      UpdateTabClasses();
      RefreshTabData();
      UpdateTabsNavigation();

      if (TabsCount > 0/* && (SelectedTabIndex < 0 || SelectedTabIndex >= TabsCount)*/)
      {
        SelectTab(0);
      }
    }

    public void RegisterTabChangedCallback(Action<int> callback)
    {
      onTabChanged += callback;
    }

    public void UnregisterTabChangedCallback(Action<int> callback)
    {
      onTabChanged -= callback;
    }

    public void AddTabElement(VisualElement tabElementRoot, string tabDisplayName)
    {
      // Create and add a new tab for the tab selection
      AddTab(CreateTab(tabDisplayName), TabsCount);

      UpdateTabClasses();

      // Add class to the content element root
      tabElementRoot.AddToClassList(tabContentElementContainerClassName);

      // Add the tab content to the content container
      tabsContentContainer.Add(tabElementRoot);

      // TODO Show hide tabs

      if (SelectedTabIndex >= 0)
      {
        ScheduleUtility.InvokeDelayedByFrames(() => SelectTab(SelectedTabIndex));
      }
    }

    public void RemoveTabAtIndex(int index)
    {
      tabsContentContainer?.RemoveAt(index);
      tabsContainer?.RemoveAt(index);
      UpdateTabClasses();
      RefreshTabData();
    }

    private void RefreshTabData()
    {
      var tabs = Tabs;

      if (tabs?.Count > 0)
      {
        for (int i = 0; i < tabs.Count; i++)
        {
          tabs[i].userData = i;
        }
      }
    }

    private void UpdateTabClasses()
    {
      var tabs = Tabs;

      if (tabs?.Count > 0)
      {
        tabs.ForEach(tab =>
        {
          tab.RemoveFromClassList(tabFirstClassName);
          tab.RemoveFromClassList(tabLastClassName);
        });

        tabs.First().AddToClassList(tabFirstClassName);
        tabs.Last().AddToClassList(tabLastClassName);
      }
    }

    private void AddTab(VisualElement tab, int index)
    {
      // Assign the tab index
      tab.userData = index;

      // Add the tab to the container
      tabsContainer.Add(tab);
    }

    private VisualElement CreateTab(string tabName = "")
    {
      VisualElement tab = null;
      Label label = null;

      if (tabsTemplateContainer.childCount > 0)
      {
        VisualTreeAsset template = null;
        var templates = tabsTemplateContainer.Children();
        foreach (var t in templates)
        {
          if (t.childCount > 0)
          {
            var child = t.Children().First();
            if (child != null && child.visualTreeAssetSource != null)
            {
              template = child.visualTreeAssetSource;
              break;
            }
          }
        }
        if (template != null)
        {
          var instance = template.Instantiate();
          if (instance != null)
          {
            tab = instance;
            label = tab.Q<Label>(className: tabLabelClassName);
          }
        }
      }

      // Check if no tab was created from a template
      if (tab == null)
      {
        tab = new VisualElement();

        label = new Label();
        label.AddToClassList(tabLabelClassName);
        tab.Add(label);
      }

      tab.AddToClassList(tabClassName);
      tab.name = $"Tab - {tabName}";
      tab.RegisterCallback<MouseDownEvent>(OnTabClicked);
      tab.RegisterCallback<NavigationSubmitEvent>(OnTabSubmit);
      tab.focusable = true;

      if (label != null)
      {
        label.name = "Tab Label";
        label.text = tabName;
        GlobalEventHandler.InvokeEvent("OnTabLabelTextChanged", label, tabName);
      }

      return tab;
    }

    private void OnTabSubmit(NavigationSubmitEvent evt)
    {
      var clickedTabIndex = (evt.currentTarget as VisualElement).userData;
      SelectTab((int)clickedTabIndex);
      evt.StopPropagation();
      this.MarkDirtyRepaint();
    }

    private void OnTabClicked(MouseDownEvent evt)
    {
      //Debug.Log(evt.propagationPhase);
      //Debug.Log(evt.currentTarget);
      var clickedTabIndex = (evt.currentTarget as VisualElement).userData;
      SelectTab((int)clickedTabIndex);
      evt.StopPropagation();
      this.MarkDirtyRepaint();
    }

    public void SelectTab(int index)
    {
      // TODO
      // Add class to selected tab
      // Add class to not selected tabs

      // Add class to selected content
      // Add class to not selected content

      var tabsCount = TabsCount;

      if (index >= 0 && index < tabsCount)
      {
        // Show/Hide tabs
        var children = tabsContainer.Children().ToList();
        for (int i = 0; i < tabsCount; i++)
        {
          var isSelectedTab = i == index;
          children[i].RemoveFromClassList(isSelectedTab ? tabDeselectedClassName : tabSelectedClassName);
          children[i].AddToClassList(isSelectedTab ? tabSelectedClassName : tabDeselectedClassName);
        }

        // Show/Hide tabs content
        children = tabsContentContainer.Children().ToList();
        for (int i = 0; i < tabsCount; i++)
        {
          if (children.Count > i)
          {
            children[i].Show(i == index);
          }
        }

        if (index != selectedTabIndex)
        {
          selectedTabIndex = index;
          onTabChanged?.Invoke(index);
        }

        //#if UNITY_EDITOR
        //        if (!Application.isPlaying)
        //        {
        //          UnityEditor.EditorApplication.delayCall += UpdateFirstContentSelectableNavigation;
        //          UnityEditor.EditorApplication.delayCall += () => Tabs[selectedTabIndex]?.Focus();
        //        }
        //        else
        //        {
        //          CoroutineRunner.Instance.InvokeDelayedByFrames(UpdateFirstContentSelectableNavigation);
        //          CoroutineRunner.Instance.InvokeDelayedByFrames(() => Tabs[selectedTabIndex]?.Focus());
        //        }
        //#else
        //        CoroutineRunner.Instance.InvokeDelayedByFrames(UpdateFirstContentSelectableNavigation);
        //        CoroutineRunner.Instance.InvokeDelayedByFrames(() => Tabs[selectedTabIndex]?.Focus());
        //#endif

        ScheduleUtility.InvokeDelayedByFrames(UpdateFirstContentSelectableNavigation);
        ScheduleUtility.InvokeDelayedByFrames(() => Tabs[selectedTabIndex]?.Focus());
      }
    }

    private IEnumerator SelectTabRoutine()
    {
      yield return null;
    }

    private void UpdateTabsNavigation()
    {
      //return;
      for (int i = 0; i < TabsCount; i++)
      {
        var selectable = Tabs[i];

        int nextTabIndex = (i + 1) % TabsCount;
        int previousTabIndex = i - 1;
        if (previousTabIndex < 0)
        {
          do
          {
            previousTabIndex = TabsCount + previousTabIndex;
          } while (previousTabIndex < 0);
        }

        selectable.UnregisterCallback<NavigationMoveEvent>(SelectFirstSelectable);
        selectable.RegisterCallback<NavigationMoveEvent>(SelectFirstSelectable);

        switch (tabNavigation)
        {
          case TabNavigationMode_UIT.HorizontalTop:
            selectable.UnregisterCallback<NavigationMoveEvent>(NavigateToTabLeftRight);
            selectable.RegisterCallback<NavigationMoveEvent>(NavigateToTabLeftRight);
            break;
          case TabNavigationMode_UIT.HorizontalBottom:
            selectable.UnregisterCallback<NavigationMoveEvent>(NavigateToTabLeftRight);
            selectable.RegisterCallback<NavigationMoveEvent>(NavigateToTabLeftRight);
            break;
          case TabNavigationMode_UIT.VerticalLeft:
            selectable.UnregisterCallback<NavigationMoveEvent>(NavigateToTabUpDown);
            selectable.RegisterCallback<NavigationMoveEvent>(NavigateToTabUpDown);
            break;
          case TabNavigationMode_UIT.VerticalRight:
            selectable.UnregisterCallback<NavigationMoveEvent>(NavigateToTabUpDown);
            selectable.RegisterCallback<NavigationMoveEvent>(NavigateToTabUpDown);
            break;
          default:
            break;
        }
      }
    }

    private void SelectFirstSelectable(NavigationMoveEvent evt)
    {
      bool select = false;

      if ((tabNavigation == TabNavigationMode_UIT.HorizontalTop || tabNavigation == TabNavigationMode_UIT.HorizontalBottom) &&
           evt.direction == NavigationMoveEvent.Direction.Down)
      {
        select = true;
      }
      else if (tabNavigation == TabNavigationMode_UIT.VerticalLeft &&
           evt.direction == NavigationMoveEvent.Direction.Right)
      {
        select = true;
      }
      else if (tabNavigation == TabNavigationMode_UIT.VerticalRight && 
        evt.direction == NavigationMoveEvent.Direction.Left)
      {
        select = true;
      }

      if (select)
      {
        var selectedIndex = SelectedTabIndex;
        if (TabsContent.Count <= selectedIndex) { return; }
        var activeContent = TabsContent[selectedIndex];
        var focusables = activeContent.Query().Where(v => v.focusable);
        var firstContentSelectable = focusables != null ? focusables.First() : null;
        if (firstContentSelectable == null) { return; }
        ScheduleUtility.InvokeDelayedByFrames(() => { firstContentSelectable.Focus(); });
        //firstContentSelectable.Focus();
      }
    }

    private void NavigateToTabLeftRight(NavigationMoveEvent evt)
    {
      var target = evt.currentTarget as VisualElement;
      var tabIndex = Tabs.IndexOf(target);
      if (tabIndex < 0 || tabIndex >= Tabs.Count) { return; }

      if (evt.direction == NavigationMoveEvent.Direction.Left)
      {
        NavigateToTabFromIndex(tabIndex, -1);
      }
      else if (evt.direction == NavigationMoveEvent.Direction.Right)
      {
        NavigateToTabFromIndex(tabIndex, 1);
      }
      //evt.StopPropagation();
      //this.MarkDirtyRepaint();
    }

    private void NavigateToTabUpDown(NavigationMoveEvent evt)
    {
      var target = evt.currentTarget as VisualElement;
      var tabIndex = Tabs.IndexOf(target);
      if (tabIndex < 0 || tabIndex >= Tabs.Count) { return; }

      if (evt.direction == NavigationMoveEvent.Direction.Up)
      {
        NavigateToTabFromIndex(tabIndex, -1);
      }
      else if (evt.direction == NavigationMoveEvent.Direction.Down)
      {
        NavigateToTabFromIndex(tabIndex, 1);
      }
    }

    private void NavigateToTabFromIndex(int index, int offset)
    {
      if (TabsCount < 0) { return; }

      var newIndex = index + offset;

      while (newIndex < 0)
      {
        newIndex = TabsCount - newIndex;
      }

      while (newIndex >= TabsCount)
      {
        newIndex = newIndex - TabsCount;
      }

      // TODO Why does this need a frame delay?
      // For some reason it won't work otherwise?
      // Is there something by Unity preventing this
      // from happening in the same/current frame?
      //#if UNITY_EDITOR
      //      if (!Application.isPlaying)
      //      {
      //        UnityEditor.EditorApplication.delayCall += () => FocusTab(newIndex, autoSelectOnTabNavigation);
      //      }
      //      else
      //      {
      //        CoroutineRunner.Instance.InvokeDelayedByFrames(() => FocusTab(newIndex, autoSelectOnTabNavigation));
      //      }
      //#else
      //      CoroutineRunner.Instance.InvokeDelayedByFrames(() => FocusTab(newIndex, autoSelectOnTabNavigation));
      //#endif

      ScheduleUtility.InvokeDelayedByFrames(() => FocusTab(newIndex, autoSelectOnTabNavigation));
    }

    private void FocusTab(int index, bool selectTab)
    {
      if (index >= 0 && index < Tabs.Count)
      {
        var tab = Tabs[index];
        tab?.Focus();

        if (selectTab)
        {
          SelectTab(index);
        }
      }
    }

    private void UpdateFirstContentSelectableNavigation()
    {
      //return;
      var selectedIndex = SelectedTabIndex;
      if (TabsContent.Count <= selectedIndex) { return; }
      var activeContent = TabsContent[selectedIndex];
      var focusables = activeContent.Query().Where(v => v.focusable);
      var firstContentSelectable = focusables != null ? focusables.First() : null;
      if (firstContentSelectable == null) { return; }
      //ConsoleLogger.Log($"First focusable: {firstContentSelectable}");

      firstContentSelectable.UnregisterCallback<NavigationMoveEvent>(SelectCurrentTabOnUp);
      firstContentSelectable.RegisterCallback<NavigationMoveEvent>(SelectCurrentTabOnUp);

      //for (int i = 0; i < TabsCount; i++)
      //{
      //  var tabSelectables = tabs[i].GetComponentsInChildren<Selectable>();
      //  //int nextTabIndex = (tabIndex + 1) % TabsCount;
      //  //int previousTabIndex = tabIndex - 1;
      //  //if (previousTabIndex < 0)
      //  //{
      //  //  do
      //  //  {
      //  //    previousTabIndex = TabsCount + previousTabIndex;
      //  //  } while (previousTabIndex < 0);
      //  //}

      //  foreach (var s in tabSelectables)
      //  {
      //    var nav = s.navigation;
      //    nav.mode = Navigation.Mode.Explicit;

      //    //nav.selectOnLeft = tabs[previousTabIndex].GetComponentInChildren<Selectable>();
      //    //nav.selectOnRight = tabs[nextTabIndex].GetComponentInChildren<Selectable>();
      //    //nav.selectOnUp = null;

      //    // Make the tab select the first item in its content
      //    // when clicking the corresponding button for its direction

      //    switch (tabNavigation)
      //    {
      //      case TabNavigationMode.HorizontalTop:
      //        nav.selectOnDown = firstContentSelectable;
      //        break;
      //      case TabNavigationMode.HorizontalBottom:
      //        nav.selectOnUp = firstContentSelectable;
      //        break;
      //      case TabNavigationMode.VerticalLeft:
      //        nav.selectOnRight = firstContentSelectable;
      //        break;
      //      case TabNavigationMode.VerticalRight:
      //        nav.selectOnLeft = firstContentSelectable;
      //        break;
      //      default:
      //        break;
      //    }

      //    s.navigation = nav;

      //    // Make the first item in the tab content
      //    // select the tab when clicking up
      //    if (i == selectedIndex && firstContentSelectable != null)
      //    {
      //      var navigation = firstContentSelectable.navigation;
      //      navigation.mode = Navigation.Mode.Explicit;
      //      navigation.selectOnUp = s;

      //      firstContentSelectable.navigation = navigation;
      //    }
      //  }
      //}
    }

    private void SelectCurrentTabOnUp(NavigationMoveEvent evt)
    {
      if (evt.direction == NavigationMoveEvent.Direction.Up)
      {
        //#if UNITY_EDITOR
        //        if (!Application.isPlaying)
        //        {
        //          UnityEditor.EditorApplication.delayCall += () => FocusTab(SelectedTabIndex, false);
        //        }
        //        else
        //        {
        //          CoroutineRunner.Instance.InvokeDelayedByFrames(() => FocusTab(SelectedTabIndex, false));
        //        }
        //#else
        //        CoroutineRunner.Instance.InvokeDelayedByFrames(() => FocusTab(SelectedTabIndex, false));
        //#endif
        ScheduleUtility.InvokeDelayedByFrames(() => FocusTab(SelectedTabIndex, false));
      }
    }
  }
}