using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.UI
{
  public enum AspectRatioMatcherMode
  {
    WidthToHeight,
    HeightToWidth
  }

  // TODO Does it need to be an ILayoutElement?
  // Is the OnRectTransformDimensionsChange method not sufficient by itself?
  // Or is it possible to just set the minWidth value on this one
  // instead of a references LayoutElement?
  [ExecuteAlways]
  [AddTooltips]
  [HeaderInfo("Matches a RectTransform's size based on the specified criterias. " +
              "Can for example force a RectTransform to be squared by matching its width to its height with a ratio of 1.")]
  public class AspectRatioMatcher : MonoBehaviour, ILayoutElement
  {
    [SerializeField]
    [Tooltip("The layout element for which to change the width/height.")]
    protected LayoutElement layoutElement;
    [SerializeField]
    [Tooltip("How the matching should take place.")]
    protected AspectRatioMatcherMode matchingMode = AspectRatioMatcherMode.WidthToHeight;
    [SerializeField]
    [Tooltip("The size ratio for the matching.")]
    protected float ratio = 1f;

    protected RectTransform rectTransform;

    protected RectTransform RectTransform
    {
      get
      {
        if (rectTransform == null)
        {
          CacheRectTransform();
        }
        return rectTransform;
      }
      set => rectTransform = value;
    }

    public float minWidth => -1;

    public float preferredWidth => -1;

    public float flexibleWidth => -1;

    public float minHeight => -1;

    public float preferredHeight => -1;

    public float flexibleHeight => -1;

    public int layoutPriority => -1;

    private void Reset()
    {
      CacheRectTransform();
    }

    private void Awake()
    {
      CacheRectTransform();
    }

    private void CacheRectTransform()
    {
      if (rectTransform == null)
      {
        rectTransform = GetComponent<RectTransform>();
      }
    }

    private void OnRectTransformDimensionsChange()
    {
      if (layoutElement != null)
      {
        var minWidth = GetMinWidth();
        if (minWidth >= 0)
        {
          layoutElement.minWidth = minWidth;
        }

        var minHeight = GetMinHeight();
        if (minHeight >= 0)
        {
          layoutElement.minHeight = minHeight;
        }
      }
    }

    protected float GetMinWidth()
    {
      if (RectTransform != null && matchingMode == AspectRatioMatcherMode.WidthToHeight)
      {
        return RectTransform.rect.height * ratio;
      }
      return -1;
    }

    protected float GetMinHeight()
    {
      if (RectTransform != null && matchingMode == AspectRatioMatcherMode.HeightToWidth)
      {
        return RectTransform.rect.width * ratio;
      }
      return -1;
    }

    public void CalculateLayoutInputHorizontal() { }

    public void CalculateLayoutInputVertical() { }
  }
}