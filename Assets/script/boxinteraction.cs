using UnityEngine;
using System.Collections;

public class boxinteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float raycastDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public ParticleSystem destroyEffect;
    private ParticleSystem DestroyA;
    private float pstime = 4f;

    [Header("Go Closer Warning")]
    public CanvasGroup goCloserCanvasGroup; // assign the CanvasGroup on your "Go Closer" UI
    public float minInteractDistance = 3f;  // matches raycastDistance ideally
    public float maxDetectDistance = 10f;   // furthest range box is "detected" but too far
    public float blinkDuration = 0.6f;      // how long one blink takes (fade in + out)
    public LayerMask boxLayerMask = ~0;

    private Playerpower powerSystem;
    private Coroutine blinkRoutine;

    void Start()
    {
        powerSystem = GetComponent<Playerpower>();

        if (goCloserCanvasGroup != null)
            goCloserCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            ShootRaycast();
        }
    }

    void ShootRaycast()
    {
        // Raycast from camera forward
        if (Physics.Raycast(Camera.main.transform.position,
                           Camera.main.transform.forward,
                           out RaycastHit hit,
                           raycastDistance))
        {
            if (hit.collider.CompareTag("box"))
            {
                Debug.Log("Hit box: " + hit.collider.name);
                Box box = hit.collider.GetComponent<Box>();

                if (box != null)
                {
                    DestroyA = Instantiate(destroyEffect, hit.point, Quaternion.identity);
                    Destroy(DestroyA.gameObject, pstime);

                    box.DestroyBox();
                    powerSystem.AddRandomPower();
                    return; // successfully interacted, no need to check "too far" case
                }
            }
        }

        // Raycast missed or didn't hit a box directly in range -> check if a box is nearby but too far
        CheckGoCloser();
    }

    void CheckGoCloser()
    {
        float closestDist = GetClosestBoxDistance();

        if (closestDist >= minInteractDistance && closestDist <= maxDetectDistance)
        {
            TriggerGoCloserBlink();
        }
    }

    float GetClosestBoxDistance()
    {
        Collider[] boxes = Physics.OverlapSphere(transform.position, maxDetectDistance, boxLayerMask);
        float closest = -1f;

        foreach (Collider col in boxes)
        {
            if (!col.CompareTag("box")) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (closest < 0f || dist < closest)
                closest = dist;
        }

        return closest;
    }

    void TriggerGoCloserBlink()
    {
        if (goCloserCanvasGroup == null) return;

        // Restart the blink if one is already playing
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkOnce());
    }

    IEnumerator BlinkOnce()
    {
        float halfDuration = blinkDuration / 2f;
        float t = 0f;

        // Fade in
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            goCloserCanvasGroup.alpha = Mathf.Clamp01(t / halfDuration);
            yield return null;
        }
        goCloserCanvasGroup.alpha = 1f;

        // Fade out
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            goCloserCanvasGroup.alpha = 1f - Mathf.Clamp01(t / halfDuration);
            yield return null;
        }
        goCloserCanvasGroup.alpha = 0f;

        blinkRoutine = null;
    }
}