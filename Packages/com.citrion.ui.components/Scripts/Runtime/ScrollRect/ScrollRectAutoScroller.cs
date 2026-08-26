using CitrioN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the automatic scrolling to a selected UI element in a scroll rect's content.")]
  [RequireComponent(typeof(ScrollRect))]
  public class ScrollRectAutoScroller : MonoBehaviour
  {
#if UNITY_EDITOR
    internal static List<ScrollRectAutoScroller> activeAutoScrollers = new List<ScrollRectAutoScroller>();
#endif

    [SerializeField]
    [Tooltip("The reference to the scroll rect for which to handle auto scrolling. " +
             "This will automatically scroll to the currently selected object if it " +
             "is part of this ScrollRects content.")]
    protected ScrollRect scrollRect;

    [SerializeField]
    [Tooltip("Optional additional offset that is applied to the scroll position. " +
             "Depending on the closest edge it will either add or substract the offset.")]
    protected float additionalOffset = 0;

    [SerializeField]
    [Tooltip("Should the root of selected object inside the " +
             "scroll rect content be searched?\n\n" +
             "If true it will use the object directly parented " +
             "to the content of the managed scroll rect that is " +
             "the parent of the currently selected object.\n\n" +
             "In the case of a complex hierarchy it is possible to" +
             "add a 'FindAutoScrollRootBlocker' to an object in the" +
             "parent hierarchy to prevent the root searching from" +
             "going any higher than necessary.\n\n" +
             "Enabling this can be useful if your compound object" +
             "has larger dimensions than the child object which was selected." +
             "This allows the auto scrolling to bring the compound object " +
             "into view instead of the selected child.")]
    protected bool findRootOfSelectable = false;

    protected Coroutine autoScrollingRoutine = null;

    protected GameObject previousSelectedObject = null;

    protected bool CanAutoScroll => scrollRect != null && scrollRect.isActiveAndEnabled && CanScrollVertical || CanScrollHorizontal;

    protected bool CanScrollVertical => scrollRect != null && scrollRect.verticalScrollbar != null && scrollRect.verticalScrollbar.isActiveAndEnabled;
    protected bool CanScrollHorizontal => scrollRect != null && scrollRect.horizontalScrollbar != null && scrollRect.horizontalScrollbar.isActiveAndEnabled;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      activeAutoScrollers.Clear();
    }
#endif

    private void Reset()
    {
      CacheReferences();
    }

    private void Awake()
    {
      CacheReferences();
    }

    private void OnEnable()
    {
      //this.InvokeDelayedByFrames(StartAutoScroll);
      StartAutoScroll();
    }

    private void OnDisable()
    {
      StopAutoScroll();
    }

    public void StartAutoScroll()
    {
      // Check if the auto scrolling is already in process
      if (autoScrollingRoutine != null) { return; }

      autoScrollingRoutine = StartCoroutine(AutoScrollRoutine());
    }

    public void StopAutoScroll()
    {
      this.StopCoroutineSafe(autoScrollingRoutine);
      autoScrollingRoutine = null;

#if UNITY_EDITOR
      activeAutoScrollers.Remove(this);
#endif
      //canScrollVertical = false;
      //canScrollHorizontal = false;
      //isAutoScrolling = false;
    }

    private IEnumerator AutoScrollRoutine()
    {
      // 2 frame delay to ensure that any
      // layout updates finished property
      yield return null;
      yield return null;

      // Reset the internally selected object
      previousSelectedObject = null;

#if UNITY_EDITOR
      activeAutoScrollers.AddIfNotContains(this);
#endif
      ConsoleLogger.Log($"Auto scrolling routine running for {name}", Common.LogType.Debug);

      while (CanAutoScroll)
      {
        ConsoleLogger.Log($"Auto scrolling routine running for {name}", Common.LogType.Debug);

        var currentSelectedObject = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (scrollRect != null && currentSelectedObject != null && currentSelectedObject != previousSelectedObject)
        {
          GameObject managedSelectedObject = currentSelectedObject;
          //var selectableRoot = currentSelectedObject.GetComponentInParent<SelectableRoot>();

          if (findRootOfSelectable)
          {
            var root = FindAutoScrollRoot(currentSelectedObject.transform, scrollRect.content);
            if (root != null)
            {
              managedSelectedObject = root.gameObject;
            }
          }

          bool isChildOfContent = managedSelectedObject.transform.IsChildOf(scrollRect.content);
          // Check if the selected object is part of any related scrollbar
          //bool isScrollbar = currentSelectedObject.transform.IsChildOf(scrollRect.verticalScrollbar.transform) ||
          //                   currentSelectedObject.transform.IsChildOf(scrollRect.horizontalScrollbar.transform);

          var childRectTransform = managedSelectedObject.GetComponent<RectTransform>();
          if (/*!isScrollbar &&*/ isChildOfContent && childRectTransform != null/* && childRectTransform.IsChildOfWithBlocker(scrollRect.transform)*/)
          {
            bool canScroll = false;
            var child = childRectTransform.transform;
            var parent = scrollRect.transform;
            ScrollRectAutoScroller childAutoScroller = null;

            do
            {
              // TODO Should this instead look for a scroll rect?
              childAutoScroller = child.GetComponent<ScrollRectAutoScroller>();

              // Handle the case another ScrollRectAutoScroller is found
              if (childAutoScroller != null && childAutoScroller != this)
              {
                break;
              }

              if (child == parent)
              {
                canScroll = true;
                break;
              }

              child = child.parent;
            } while (child != null);

            if (canScroll)
            {
              ScrollTo(childRectTransform);
            }
          }
        }

        previousSelectedObject = currentSelectedObject;

        yield return null;
      }

      StopAutoScroll();
    }

    public void ScrollTo(RectTransform childRectTransform)
    {
      if (childRectTransform == null) { return; }
      if (scrollRect == null || scrollRect.viewport == null || scrollRect.content == null) { return; }

      // Check if the provided transform is actually a child of the scroll content
      if (!childRectTransform.IsChildOf(scrollRect.content)) { return; }

      // Check if the child is already fully visible
      if (IsRectTransformFullyInsideViewport(childRectTransform, scrollRect,
                                             out var topIsClosest, out var rightIsClosest)) { return; }

      ConsoleLogger.Log($"Scrolling to {childRectTransform.name}", Common.LogType.Debug);
      var additionaloffset = new Vector2(0, scrollRect.viewport.rect.height * (topIsClosest ? 0.0f : 1.0f));
      additionaloffset.x = scrollRect.viewport.rect.width * -1;// (rightIsClosest ? -1.0f : -1.0f);

      var itemHeightOffset = new Vector2(0, childRectTransform.rect.height / 2);
      var itemWidthOffset = new Vector2(childRectTransform.rect.width / 2, 0);
      var transformedPosition = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
                              - (Vector2)scrollRect.transform.InverseTransformPoint(childRectTransform.position);
      var anchoredPosition = transformedPosition - additionaloffset;
      anchoredPosition += itemHeightOffset * (topIsClosest ? -1 : 1);
      anchoredPosition += itemWidthOffset * -1;//(rightIsClosest ? -2 : -2);
      if (additionalOffset > 0)
      {
        anchoredPosition += new Vector2(additionalOffset * (rightIsClosest ? -1 : -1),
                                        additionalOffset * (topIsClosest ? -1 : 1));
      }
      scrollRect.content.anchoredPosition = anchoredPosition;
    }

    private void ScrollToChild(int childIndex)
    {
      if (scrollRect == null || scrollRect.content == null) { return; }

      var childCount = scrollRect.content.childCount;

      if (childIndex < 0) { childIndex = 0; }
      else if (childIndex >= childCount) { childIndex = childCount - 1; }

      var child = scrollRect.content.GetChild(childIndex);
      if (child != null)
      {
        ScrollTo(child.GetComponent<RectTransform>());
      }
    }

    private void CacheReferences()
    {
      if (scrollRect == null)
      {
        scrollRect = GetComponent<ScrollRect>();
      }
    }

    private Transform FindAutoScrollRoot(Transform transform, Transform searchUntil = null)
    {
      Transform root;
      FindAutoScrollRootBlocker findAutoScrollRootBlocker = null;

      do
      {
        root = transform;
        transform = transform.parent;
        findAutoScrollRootBlocker = transform != null ? transform.GetComponent<FindAutoScrollRootBlocker>() : null;

      } while (transform != null && findAutoScrollRootBlocker == null &&
              (searchUntil == null || transform != searchUntil));

      return root;
    }

    private bool IsRectTransformFullyInsideViewport(RectTransform rectTransform, ScrollRect scrollRect,
                                                    out bool topIsClosest, out bool rightIsClosest)
    {
      Vector3[] corners = new Vector3[4];
      rectTransform.GetWorldCorners(corners);
      var rectBottomLeft = corners[0];
      var rectTopRight = corners[2];
      var rectHeight = rectTopRight.y - rectBottomLeft.y;
      var rectWidth = rectTopRight.x - rectBottomLeft.x;
      var rectMidPointVertical = rectBottomLeft.y + rectHeight / 2;
      var rectMidPointHorizontal = rectBottomLeft.x + rectWidth / 2;

      RectTransform viewportRect = scrollRect.viewport;
      Vector3[] viewPortCorners = new Vector3[4];
      viewportRect.GetWorldCorners(viewPortCorners);
      var viewPortBottomLeft = viewPortCorners[0];
      var viewPortTopRight = viewPortCorners[2];

      topIsClosest = Mathf.Abs(viewPortTopRight.y - rectMidPointVertical) <
                     Mathf.Abs(viewPortBottomLeft.y - rectMidPointVertical);

      rightIsClosest = Mathf.Abs(viewPortTopRight.x - rectMidPointHorizontal) <
                       Mathf.Abs(viewPortBottomLeft.x - rectMidPointVertical);

      // Check if the rect resides in the viewport
      // Using custom code because Unity buitin API calls didn't work properly
      bool fullyInside =
        (rectBottomLeft.x >= viewPortBottomLeft.x || Mathf.Approximately(rectBottomLeft.x, viewPortBottomLeft.x)) &&
        (rectTopRight.x <= viewPortTopRight.x || Mathf.Approximately(rectTopRight.x, viewPortTopRight.x)) &&
        (rectBottomLeft.y >= viewPortBottomLeft.y || Mathf.Approximately(rectBottomLeft.y, viewPortBottomLeft.y)) &&
        (rectTopRight.y <= viewPortTopRight.y || Mathf.Approximately(rectTopRight.y, viewPortTopRight.y));
      return fullyInside;
    }
  }
}