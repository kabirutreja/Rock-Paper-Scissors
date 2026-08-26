using UnityEngine;

namespace CitrioN.UI
{
  public class RescaleRectTransform : MonoBehaviour
  {
    #region Public Variables
    public enum ScaleMethod { MatchWidthToHeight, MatchHeightToWidth }

    public ScaleMethod scaleMethod;
    #endregion

    #region Private Variables
    [Header("Serialized Data"), Space(10)]
    [SerializeField] private RectTransform rectTransform;
    #endregion

    private void Reset()
    {
      rectTransform = GetComponent<RectTransform>();
    }

    #region Methods
    private void Start()
    {
      rectTransform = GetComponent<RectTransform>();
      Rescale();
    }

    private void Update()
    {
      Rescale();
    }

    [ContextMenu("Rescale")]
    public void Rescale()
    {
      if (rectTransform == null)
      {
        return;
      }

      // Default
      Vector2 newWidthAndHeight = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

      if (scaleMethod == ScaleMethod.MatchWidthToHeight)
      {
        newWidthAndHeight = new Vector2(rectTransform.rect.height, rectTransform.rect.height);
      }
      else if (scaleMethod == ScaleMethod.MatchHeightToWidth)
      {
        newWidthAndHeight = new Vector2(rectTransform.rect.width, rectTransform.rect.width);
      }

      rectTransform.sizeDelta = newWidthAndHeight;
    }
    #endregion
  }
}