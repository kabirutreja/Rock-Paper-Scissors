using UnityEngine;
using System.Collections;

public class SlideDownUI : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float startYOffset = 400f; // how far above screen it starts
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 targetPosition;
    private Coroutine slideRoutine;

    void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        targetPosition = panelRect.anchoredPosition;
    }

    void OnEnable()
    {
        // Start above the target position
        panelRect.anchoredPosition = targetPosition + new Vector2(0, startYOffset);

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        slideRoutine = StartCoroutine(SlideIn());
    }

    IEnumerator SlideIn()
    {
        float t = 0f;
        Vector2 startPos = panelRect.anchoredPosition;

        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime; // unscaled in case you pause with Time.timeScale = 0
            float progress = easeCurve.Evaluate(Mathf.Clamp01(t / slideDuration));
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, progress);
            yield return null;
        }

        panelRect.anchoredPosition = targetPosition;
    }
}
