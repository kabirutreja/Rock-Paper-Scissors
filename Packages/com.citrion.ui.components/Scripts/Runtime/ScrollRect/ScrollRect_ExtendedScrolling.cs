using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.UI
{
  public class ScrollRect_ExtendedScrolling : ScrollRect, IPointerEnterHandler, IPointerExitHandler
  {
    //public int numberOfItemsToShow = 5;

    protected RectTransform rectTransform;

    protected bool DoHandleScroll { get; set; } = false;

    protected bool CanScroll { get; set; } = false;

    protected bool HasScrollInput
    {
      get
      {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxis(SCROLL_AXIS) != 0;
#elif ENABLE_INPUT_SYSTEM
        var scroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue();
        return scroll.x != 0 || scroll.y != 0;
#endif
      }
    }

    protected RectTransform RectTransform
    {
      get
      {
        if (rectTransform == null)
        {
          rectTransform = GetComponent<RectTransform>();
        }
        return rectTransform;
      }
      set => rectTransform = value;
    }

    protected Vector2 scrollDelta = Vector2.zero;

    protected const string SCROLL_AXIS = "Mouse ScrollWheel";

    protected GameObject currentSelectedObject = null;

    protected float lastCheck = 0;

    protected static List<ScrollRect_ExtendedScrolling> orderedScrollRects =
      new List<ScrollRect_ExtendedScrolling>();

    public static List<ScrollRect_ExtendedScrolling> activeScrollRects =
      new List<ScrollRect_ExtendedScrolling>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      activeScrollRects.Clear();
      orderedScrollRects.Clear();
    }

    public static bool HasPriority(ScrollRect_ExtendedScrolling scrollRect)
    {
      //ConsoleLogger.Log(activeScrollRects[0].name);
      return activeScrollRects?.Count > 0 && activeScrollRects[0] == scrollRect;
    }

    //protected override void Awake()
    //{
    //  base.Awake();
    //  rectTransform = GetComponent<RectTransform>();
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
      Enter();
    }

    private void Enter()
    {
      StopAllCoroutines();
      DoHandleScroll = true;
      activeScrollRects.Remove(this);
      activeScrollRects.Add(this);
      // TODO Make this more efficient by caching the event system?
      currentSelectedObject = EventSystem.current.currentSelectedGameObject;
      //ConsoleLogger.Log($"Added {this.name}");
      StartCoroutine(HandleScrolling());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      //Exit();
    }

    public void Exit()
    {
      DoHandleScroll = false;
      activeScrollRects.Remove(this);
      //ConsoleLogger.Log($"Removed {this.name}");
    }

    public bool ContainsPointer()
    {
      if (RectTransform != null)
      {
        Vector2 localMousePosition = RectTransform.InverseTransformPoint(MousePosition);
        return RectTransform.rect.Contains(localMousePosition);
      }
      return false;
    }

    protected Vector2 MousePosition
    {
      get
      {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
        var control = UnityEngine.InputSystem.Mouse.current.position;
        return control.ReadValue();
#endif
      }
    }

    protected IEnumerator HandleScrolling()
    {
      while (DoHandleScroll)
      {
        if (/*HasPriority(this) && */HasScrollInput)
        {
          PointerEventData pointerData = new PointerEventData(EventSystem.current);

          if (Time.time != lastCheck)
          {
            pointerData.position = MousePosition;
            orderedScrollRects = GetComponentsBelowScreenPosition<ScrollRect_ExtendedScrolling>(pointerData, true);
          }

          if (orderedScrollRects != null && orderedScrollRects.Count > 0 &&
              orderedScrollRects[orderedScrollRects.Count - 1/*0*/] == this)
          {

            scrollDelta.y = 0;
#if ENABLE_LEGACY_INPUT_MANAGER
            scrollDelta.y = Input.GetAxis(SCROLL_AXIS);
#elif ENABLE_INPUT_SYSTEM
            var scroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue();
            // We multiply the actual scroll value with to get about the same
            // value as with the old input system. This means there are about the
            // same steps for the scroll view for both input systems.
#if UNITY_6000_0_OR_NEWER
            scrollDelta.y = scroll.y * 0.095f;
#else
            scrollDelta.y = scroll.y * 0.0008f;
#endif
#endif
            pointerData.scrollDelta = scrollDelta;

            CanScroll = true;
            OnScroll(pointerData);
            CanScroll = false;
          }
          else if (!orderedScrollRects.Contains(this))
          {
            Exit();
          }
        }
        yield return null;
      }
    }

    public override void OnScroll(PointerEventData data)
    {
      if (CanScroll)
      {
        data.scrollDelta *= scrollSensitivity;
        base.OnScroll(data);
      }
    }

    public static List<T> GetComponentsBelowScreenPosition<T>(PointerEventData eventData, bool sortByDepth = false) where T : MonoBehaviour
    {
      List<RaycastResult> raycastResults = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, raycastResults);
      List<T> components = new List<T>();
      if (sortByDepth)
      {
        raycastResults = raycastResults.OrderByDescending(result => result.depth).ToList();
      }
      raycastResults.ForEach(result =>
      {
        // Check for the desired component on the object hit
        T component = result.gameObject.GetComponent<T>();
        // Check if a component of type T was found
        if (component != null && component.enabled)
        {
          components.Add(component);
        }
      });
      return components;
    }

    //public override void CalculateLayoutInputHorizontal()
    //{
    //  base.CalculateLayoutInputHorizontal();
    //  CalculateLayoutInputSizeForAxis(0);
    //  viewport.sizeDelta = new Vector2(1000, 300);
    //}

    //public override void CalculateLayoutInputVertical()
    //{
    //  CalculateLayoutInputSizeForAxis(1);
    //  viewport.sizeDelta = new Vector2(viewport.rect.width, viewport.rect.height);
    //}

    //protected void CalculateLayoutInputSizeForAxis(int axis)
    //{
    //  // Calculate the size of the content based on the specified number of items
    //  if (content == null)
    //    return;

    //  //float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);
    //  //float size = (axis == 0 ? rectTransform.rect.size.x : rectTransform.rect.size.y)/* - combinedPadding*/;
    //  //int constraintCount = (axis == 0 ? constraintCountX : constraintCountY);

    //  //// Calculate the item size by taking into account the spacing and constraint count
    //  //float itemSize = (size - spacing * (constraintCount - 1)) / constraintCount;
    //  //float contentSize = itemSize * numberOfItemsToShow + spacing * (numberOfItemsToShow - 1) + combinedPadding;

    //  if (axis == 0)
    //    SetLayoutInputForAxis(contentSize, contentSize, -1, axis);
    //  else
    //    SetLayoutInputForAxis(contentSize, contentSize, -1, axis);
    //}

    //protected override void OnCanvasHierarchyChanged()
    //{
    //  base.OnCanvasHierarchyChanged();
    //  //ResizeViewport();
    //  ConsoleLogger.Log("Hierachy Changed!");
    //}

    //public override void LayoutComplete()
    //{
    //  base.LayoutComplete();
    //  ConsoleLogger.Log("Layout complete!");
    //  ResizeViewport();
    //}

    //[ContextMenu("Resize")]
    //private void ResizeViewport()
    //{
    //  List<Transform> enabledChildren = new List<Transform>();
    //  var childCount = content.childCount;
    //  for (int i = 0; i < childCount; i++)
    //  {
    //    var child = content.GetChild(i);
    //    if (child.gameObject.activeSelf)
    //    {
    //      enabledChildren.Add(child);
    //    }
    //  }

    //  var itemCount = enabledChildren.Count;

    //  float height = 0;

    //  var layoutGroup = content.GetComponent<LayoutGroup>();
    //  if (layoutGroup != null)
    //  {
    //    height += layoutGroup.padding.top;
    //    if (numberOfItemsToShow >= itemCount)
    //    {
    //      height += layoutGroup.padding.bottom;
    //    }
    //  }

    //  float spacing = 0;

    //  if (layoutGroup is HorizontalLayoutGroup h)
    //  {
    //    spacing = h.spacing;
    //  }
    //  else if (layoutGroup is VerticalLayoutGroup v)
    //  {
    //    spacing = v.spacing;
    //  }

    //  var spacingAmount = Mathf.Min(numberOfItemsToShow, itemCount);
    //  if (spacingAmount == itemCount) { spacingAmount--; }
    //  if (spacingAmount < 0) { spacingAmount = 0; }
    //  height += spacingAmount * spacing;

    //  for (int i = 0; i < numberOfItemsToShow && i < itemCount; i++)
    //  {
    //    var itemHeight = enabledChildren[i].GetComponent<RectTransform>()?.rect.height;
    //    if (itemHeight != null)
    //    {
    //      height += (float)itemHeight;
    //    }
    //  }

    //  //viewport.sizeDelta = new Vector2(viewport.sizeDelta.x, viewportHeight);

    //  var rectTransform = GetComponent<RectTransform>();
    //  if (rectTransform != null)
    //  {
    //    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    //  }

    //  // Notify the Scroll Rect that the size has changed, so it recalculates the scrollbars and other parameters
    //  CalculateLayoutInputVertical();
    //}
  }
}