using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Helper script that can set a RectTransform's preferred width and/or height " +
               "based on the available widht/height of its parent. A HorizontalLayoutGroup for the width and a " +
              "VerticalLayoutGroup for the height is required to be on the parent for this script to work.")]
  [RequireComponent(typeof(RectTransform))]
  public class PercentageBasedPreferredWidthAndHeight : MonoBehaviour, ILayoutElement
  {
    [SerializeField]
    [Tooltip("Whether the width should be set.")]
    protected bool useWidth = false;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The width percentage based on the total available width.")]
    protected float percentageWidth = 0.5f;

    [Space(10)]

    [SerializeField]
    [Tooltip("Whether the height should be set.")]
    protected bool useHeight = false;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The height percentage based on the total available height.")]
    protected float percentageHeight = 0.5f;

    public float preferredWidth
    {
      get
      {
        if (!useWidth) { return -1; }

        var parent = transform.parent;
        if (parent == null) { return -1; }

        var parentRect = parent.GetComponent<RectTransform>();
        if (parentRect == null) { return -1; }

        float parentWidth = parentRect.rect.width;

        var layoutGroup = parent.GetComponent<LayoutGroup>();

        if (layoutGroup != null)
        {
          var padding = layoutGroup.padding;
          parentWidth -= padding.left;
          parentWidth -= padding.right;
          var childCount = parent.childCount;

          if (childCount > 1 && layoutGroup is HorizontalLayoutGroup h)
          {
            parentWidth -= (childCount - 1) * h.spacing;
          }
        }

        float preferredWidth = parentWidth * PercentageWidth;
        return preferredWidth;
      }
    }

    public float preferredHeight
    {
      get
      {
        if (!UseHeight) { return -1; }

        var parent = transform.parent;
        if (parent == null) { return -1; }

        var parentRect = parent.GetComponent<RectTransform>();
        if (parentRect == null) { return -1; }

        float parentHeight = parentRect.rect.height;

        var layoutGroup = parent.GetComponent<LayoutGroup>();

        if (layoutGroup != null)
        {
          var padding = layoutGroup.padding;
          parentHeight -= padding.top;
          parentHeight -= padding.bottom;
          var childCount = parent.childCount;

          if (childCount > 1 && layoutGroup is VerticalLayoutGroup v)
          {
            parentHeight -= (childCount - 1) * v.spacing;
          }
          //{
          //if (layoutGroup is HorizontalLayoutGroup h)
          //{
          //  parentHeight -= (childCount - 1) * h.spacing;
          //}
          //else if (layoutGroup is VerticalLayoutGroup v)
          //{
          //  parentHeight -= (childCount - 1) * v.spacing;
          //}
          //}
        }

        float preferredHeight = parentHeight * PercentageHeight;
        return preferredHeight;
      }
    }

    public float minWidth { get { return -1; } }

    public float minHeight { get { return -1; } }

    public float flexibleWidth { get { return -1; } }

    public float flexibleHeight { get { return -1; } }

    public int layoutPriority { get { return 1; } }

    public float PercentageWidth
    {
      get => percentageWidth;
      set => percentageWidth = Mathf.Clamp01(value);
    }

    public float PercentageHeight
    {
      get => percentageHeight;
      set => percentageHeight = Mathf.Clamp01(value);
    }

    public bool UseHeight
    {
      get => useHeight;
      set => useHeight = value;
    }

    public void CalculateLayoutInputHorizontal() { }

    public void CalculateLayoutInputVertical() { }
  }
}