using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HeaderInfo("Updates the UI navigation for a UGUI settings menu. " +
              "The navigation will be updated when the settings menu is opened or " +
              "if any of the input elements of the settings menu are enabled or disabled.")]
  public abstract class SettingsMenu_UGUI_NavigationSetter : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The settings menu to set the navigation for.")]
    protected SettingsMenu_UGUI menu;

    [SerializeField]
    [Tooltip("Whether the first selectable should be selected when the navigation gets updated. " +
             "This can be used to ensure there is a valid auto selection when the menu becomes enabled.")]
    protected bool selectFirstSelectableOnRefresh = false;

    protected float lastRefresh = 0;

    protected virtual void Awake()
    {
      if (menu == null)
      {
        menu = GetComponent<SettingsMenu_UGUI>();
      }
    }

    protected virtual void OnEnable()
    {
      if (menu != null)
      {
        menu.OnOpen.AddListener(SetMenuNavigation);
        if (menu.IsOpen)
        {
          SetMenuNavigation();
        }
      }
    }

    protected virtual void OnDisable()
    {
      if (menu != null)
      {
        menu.OnOpen.RemoveListener(SetMenuNavigation);
      }
    }

    public void SetMenuNavigation()
    {
      //SetMenuNavigationInternal();
      // We delay by a frame to make sure the event system has been initialized
      this.InvokeDelayedByFrames(() => SetMenuNavigationInternal(), 2);
    }

    protected virtual void SetMenuNavigationInternal(bool selectFirstSelectable = true)
    {
      if (menu == null) { return; }
      var inputElements = menu.InputElements;

      if (inputElements.Count > 0)
      {
        Selectable firstSelectable = null;
        List<Selectable> selectableElements = new List<Selectable>();
        Dictionary<Selectable, RectTransform> selectableRoots = new Dictionary<Selectable, RectTransform>();

        foreach (var elem in inputElements)
        {
          if (elem == null || !elem.gameObject.activeInHierarchy) { continue; }
          var s = elem.GetComponentInChildren<Selectable>();
          if (s == null || !s.isActiveAndEnabled) { continue; }
          var root = s.GetComponentInParent<SettingObject>();
          if (root == null) { continue; }
          // Check if it is the correct setting object
          if (root == elem.GetComponent<SettingObject>())
          {
            selectableElements.AddIfNotContains(s);
            selectableRoots.AddOrUpdateDictionaryItem(s, elem);
          }
        }

        //var selectableInputElements = inputElements.Where(elem 
        //  => elem != null && elem.GetComponentInChildren<Selectable>(includeInactive: false) != null)
        //                         .Where(e => e.gameObject.activeInHierarchy).ToList();
        //var selectableElements = selectableInputElements.Select(elem 
        //  => elem.GetComponentInChildren<Selectable>(includeInactive: false)).ToList();

        for (int i = 0; i < selectableElements.Count; i++)
        {
          //var selectableRectTransform = selectableInputElements[i];
          //var selectableRectTransform = selectableElements[i].GetComponent<RectTransform>();
          //selectableRoots.AddOrUpdateDictionaryItem(selectableElements[i], selectableRectTransform);

          //var onEnableDisable = selectableRectTransform.gameObject.AddOrGetComponent<OnEnableDisableEvents>();
          if (selectableRoots.TryGetValue(selectableElements[i], out var selectableRectTransform))
          {
            var onEnableDisable = selectableRectTransform.gameObject.AddOrGetComponent<OnEnableDisableEvents>();
            onEnableDisable.OnDisableEvent.RemoveListener(RefreshMenuNavigation);
            onEnableDisable.OnDisableEvent.AddListener(RefreshMenuNavigation);
            onEnableDisable.OnEnableEvent.RemoveListener(RefreshMenuNavigation);
            onEnableDisable.OnEnableEvent.AddListener(RefreshMenuNavigation);
          }
        }

        var elementsCount = selectableElements.Count;

        for (int i = elementsCount - 1; i >= 0; i--)
        {
          var selectable = selectableElements[i];
          firstSelectable = selectable;

          var navigation = selectable.navigation;
          navigation.mode = Navigation.Mode.Explicit;
          navigation.selectOnLeft = selectable.FindSelectableOnLeft();
          navigation.selectOnRight = selectable.FindSelectableOnRight();
          navigation.selectOnUp = GetNextInputElement(menu, selectable, selectableElements, selectableRoots, false);
          navigation.selectOnDown = GetNextInputElement(menu, selectable, selectableElements, selectableRoots, true);
          selectable.navigation = navigation;
        }

        if (selectFirstSelectable && firstSelectable != null && EventSystem.current != null)
        {
          if (EventSystem.current != null)
          {
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
          }
          else
          {
            this.InvokeDelayedByFrames(() =>
            {
              if (EventSystem.current != null)
              {
                EventSystem.current.SetSelectedGameObject(firstSelectable?.gameObject);
              }
            });
          }
        }
      }
    }

    [ContextMenu("Refresh Menu Navigation")]
    protected virtual void RefreshMenuNavigation()
    {
      if (lastRefresh != Time.time || lastRefresh == 0)
      {
        lastRefresh = Time.time;

        // We wait until the next frame because the content
        // may be modified within the current frame.
        // This ensures we select the corrent selectable
        this.InvokeDelayedByFrames(() => SetMenuNavigationInternal(selectFirstSelectableOnRefresh), 2);
      }
    }

    protected abstract Selectable GetNextInputElement(SettingsMenu_UGUI menu, Selectable origin,
      List<Selectable> inputElements, Dictionary<Selectable, RectTransform> selectableRoots, bool next = true);

    protected virtual string GetSettingsParent(Selectable selectable,
      Dictionary<Selectable, RectTransform> selectableRoots, SettingsMenu_UGUI menu)
    {
      if (selectableRoots.TryGetValue(selectable, out var root))
      {
        if (menu.SettingObjects.TryGetValue(root, out var settingHolder))
        {
          return settingHolder.InputElementProviderSettings.ParentIdentifier;
        }
      }
      return null;
    }
  }
}