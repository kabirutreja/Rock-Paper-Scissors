using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the dynamic vertical resizing of a 'ScrollRect' viewport to match the size of specified children count.")]
  public class AdaptScrollRectViewportVerticallyForContentChildren : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The 'ScrollRect' component to adapt the viewport for.")]
    protected ScrollRect scrollRect;

    [SerializeField]
    [Tooltip("The amount of children to show in the viewport. " +
             "The viewport will be sized so the specified amount of children will " +
             "fit. Won't work with differently sized children!")]
    protected int itemsToShow = 5;

    private void Reset()
    {
      if (scrollRect == null)
      {
        scrollRect = GetComponent<ScrollRect>();
      }
    }

    private void OnEnable()
    {
      // Resize a frame delayed because items
      // might be on enable
      this.InvokeDelayedByFrames(ResizeViewport);
    }

    [ContextMenu("Resize")]
    private void ResizeViewport()
    {
      if (scrollRect == null) { return; }
      var content = scrollRect.content;
      if (content == null) { return; }

      List<Transform> enabledChildren = new List<Transform>();
      var childCount = content.childCount;
      for (int i = 0; i < childCount; i++)
      {
        var child = content.GetChild(i);
        if (child.gameObject.activeSelf)
        {
          enabledChildren.Add(child);
        }
      }
      var itemCount = enabledChildren.Count;

      float height = 0;

      var layoutGroup = content.GetComponent<LayoutGroup>();
      if (layoutGroup != null)
      {
        height += layoutGroup.padding.top;
        if (itemsToShow >= itemCount)
        {
          height += layoutGroup.padding.bottom;
        }
      }

      float spacing = 0;

      if (layoutGroup is VerticalLayoutGroup v)
      {
        spacing = v.spacing;
      }
      else if (layoutGroup is HorizontalLayoutGroup h)
      {
        // TODO Should support be added for a horizontal layout?
        //spacing = h.spacing;
      }

      var spacingAmount = Mathf.Min(itemsToShow, itemCount);
      if (spacingAmount == itemCount) { spacingAmount--; }
      if (spacingAmount < 0) { spacingAmount = 0; }
      height += spacingAmount * spacing;

      for (int i = 0; i < itemsToShow && i < itemCount; i++)
      {
        var rectTransformComponent = enabledChildren[i].GetComponent<RectTransform>();
        if (rectTransformComponent == null) { continue; }
        var itemHeight = rectTransformComponent.rect.height;
        height += (float)itemHeight;
      }

      var rectTransform = GetComponent<RectTransform>();
      if (rectTransform != null)
      {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
      }

      scrollRect.CalculateLayoutInputVertical();
    }
  }
}