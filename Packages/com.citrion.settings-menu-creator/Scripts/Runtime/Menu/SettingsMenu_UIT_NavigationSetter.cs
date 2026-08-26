using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HeaderInfo("Manages the UI navigation for UI elements of a UI Toolkit settings menu. " +
              "The navigation will be handled using the 'NavigationMoveEvent'.")]
  public class SettingsMenu_UIT_NavigationSetter : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The settings menu to manage the navigation for.")]
    protected SettingsMenu_UIT menu;

    protected List<VisualElement> selectableInputElements = new List<VisualElement>();

    // Child => Root relation
    Dictionary<VisualElement, VisualElement> selectableRoots = new Dictionary<VisualElement, VisualElement>();

    private void Awake()
    {
      if (menu == null)
      {
        menu = GetComponent<SettingsMenu_UIT>();
      }
    }

    private void OnEnable()
    {
      if (menu != null)
      {
        menu.OnOpen.AddListener(UpdateSelectionForMenu);
      }
    }

    private void OnDisable()
    {
      if (menu != null)
      {
        menu.OnOpen.RemoveListener(UpdateSelectionForMenu);
      }
    }

    private void UpdateSelectionForMenu()
    {
      ConsoleLogger.Log("Updating menu selection for UI Toolkit settings menu", Common.LogType.Debug);
      SetupMenuNavigation();
    }

    //protected void SetupMenuNavigation()
    //{
    //  // TODO Finish implementation

    //  if (inputElements.Count > 0)
    //  {
    //    VisualElement firstSelectable = null;
    //    selectableInputElements.Clear();
    //    inputElements.ForEach(elem =>
    //    {
    //      var s = elem.Q(className: ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);
    //      if (s != null && s.focusable)
    //      {
    //        selectableInputElements.AddIfNotContains(s);
    //      }
    //      else if (elem.focusable)
    //      {
    //        selectableInputElements.AddIfNotContains(elem);
    //      }
    //    });

    //    var elementsCount = selectableInputElements.Count;

    //    for (int i = elementsCount - 1; i >= 0; i--)
    //    {
    //      var selectable = selectableInputElements[i];

    //      //if (selectable != null)
    //      {
    //        firstSelectable = selectable;

    //        selectable.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
    //        selectable.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
    //      }
    //    }

    //    firstSelectable?.Focus();
    //  }
    //}

    protected void SetupMenuNavigation()
    {
      //return;
      if (menu == null) { return; }
      var inputElements = menu.InputElements;

      if (inputElements.Count > 0)
      {
        VisualElement firstSelectable = null;

        selectableInputElements.Clear();
        selectableRoots.Clear();
        //var selectableInputElements = inputElements.Where(elem
        //  => elem != null && elem.GetComponentInChildren<Selectable>(includeInactive: true) != null).ToList();
        //var selectableElements = selectableInputElements.Select(elem
        //  => elem.GetComponentInChildren<Selectable>(includeInactive: true)).ToList();

        inputElements.ForEach(elem =>
        {
          var s = elem.Q(className: ProviderUtility_UIT.INPUT_ELEMENT_SELECTABLE_CLASS);
          if (s != null && s.focusable)
          {
            if (selectableInputElements.AddIfNotContains(s))
            {
              selectableRoots.AddOrUpdateDictionaryItem(s, elem);
            }
          }
          else if (elem.focusable)
          {
            if (selectableInputElements.AddIfNotContains(elem))
            {
              selectableRoots.AddOrUpdateDictionaryItem(elem, elem);
            }
          }
        });

        var elementsCount = selectableInputElements.Count;

        for (int i = elementsCount - 1; i >= 0; i--)
        {
          var selectable = selectableInputElements[i];

          if (selectable != null)
          {
            firstSelectable = selectable;

            selectable.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
            selectable.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
          }
        }

        // We need to delay the focus by a frame because otherwise
        // Unity doesn't seem to recognize it as focused and will
        // not process any events such as the NavigationMoveEvent.
        this.InvokeDelayedByFrames(() => firstSelectable?.Focus());
      }
    }

    private void OnNavigationMove(NavigationMoveEvent evt)
    {
      //ConsoleLogger.Log($"{evt.direction} on {evt.target}");
      switch (evt.direction)
      {
        case NavigationMoveEvent.Direction.None:
          break;
        case NavigationMoveEvent.Direction.Left:
          break;
        case NavigationMoveEvent.Direction.Up:
          FocusNextInputElement(evt.currentTarget as VisualElement, false);
          break;
        case NavigationMoveEvent.Direction.Right:
          break;
        case NavigationMoveEvent.Direction.Down:
          FocusNextInputElement(evt.currentTarget as VisualElement);
          break;
#if UNITY_2022_2_OR_NEWER
        case NavigationMoveEvent.Direction.Next:
          break;
        case NavigationMoveEvent.Direction.Previous:
          break;
#endif
      }
      //evt.PreventDefault();
    }

    protected virtual void FocusNextInputElement(VisualElement origin, bool next = true)
    {
      if (origin == null) { return; }
      var index = selectableInputElements.IndexOf(origin);
      if (index < 0) { return; }
      VisualElement nextElement = null;
      var increment = next ? 1 : -1;
      var elementsCount = selectableInputElements.Count;
      var maxAttempts = selectableInputElements.Count;
      var attempts = 0;
      string settingParent = GetSettingsParent(origin);
      if (string.IsNullOrEmpty(settingParent)) { return; }
      VisualElement parent;
      StyleEnum<DisplayStyle> display;

      do
      {
        attempts++;
        index = index + increment;
        // TODO Refactor this to use modulo for shorter code?
        if (index < 0)
        {
          do
          {
            index = elementsCount + index;
          } while (index < 0);
        }
        else if (index >= elementsCount)
        {
          do
          {
            var remainingCount = index - elementsCount;
            index = remainingCount;
          } while (index >= elementsCount);
        }

        nextElement = selectableInputElements[index];
        string parentClass = GetSettingsParent(nextElement);
        parent = nextElement.GetFirstAncestorWithClass(parentClass);
        display = parent.style.display;

      } while (attempts <= maxAttempts && (nextElement == null || !nextElement.visible || display == DisplayStyle.None));

      //index = Mathf.Clamp(index + (next ? 1 : -1), 0, inputElements.Count - 1);
      //inputElements[index]?.Focus();
      //nextElement?.Focus();
      ScheduleUtility.InvokeDelayedByFrames(() => nextElement?.Focus());
    }

    private string GetSettingsParent(VisualElement selected)
    {
      if (selectableRoots.TryGetValue(selected, out var root))
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
