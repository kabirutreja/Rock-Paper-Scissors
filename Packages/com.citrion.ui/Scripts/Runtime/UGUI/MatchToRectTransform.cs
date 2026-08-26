using CitrioN.Common;
using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Allows the automatic matching of certain RectTransform values
  /// for the RectTransform component of this GameObject to one that is
  /// provided.
  /// The matching will occur every frame.
  /// </summary>
  [AddTooltips]
  [HeaderInfo("Allows the automatic matching of certain RectTransform values for the RectTransform component " +
              "of this GameObject to the one referenced. The matching will occur every frame.")]
  public class MatchToRectTransform : MonoBehaviour
  {
    [Tooltip("The RectTransform to match the size of.")]
    public RectTransform rectToMatch;

    [Tooltip("The operation that defines what values should be matched.")]
    public RectMatchOperation matchOperation;

    // The RectTransform component of this GameObject
    private RectTransform rectTransform;

    private void Start()
    {
      rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
      if (rectTransform != null)
      {
        //rectTransform.sizeDelta = new Vector2(rectToMatch.rect.width, rectTransform.rect.height);
        rectTransform.MatchValuesToRect(rectToMatch, matchOperation);
      }
    }
  }
}