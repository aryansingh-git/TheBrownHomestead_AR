using UnityEngine;
using System.Collections;

public class ToggleScaleUIElement : MonoBehaviour
{
    // Factor to enlarge (1.5 means 150% of the original)
    public float scaleMultiplier = 1.1f;

    // Duration for the scale animation in seconds.
    public float animationDuration = 0.3f;

    // Store the original scale of the UI element.
    private Vector3 originalScale;

    // Track whether the UI element is currently enlarged.
    private bool isEnlarged = false;

    // Cache the RectTransform if available.
    private RectTransform rectTransform;

    void Start()
    {
        // Try to get the RectTransform which is common for UI elements.
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }
        else
        {
            originalScale = transform.localScale;
        }
    }

    // This method toggles the scale when called.
    public void ToggleScale()
    {
        // Determine which target scale to animate to.
        Vector3 targetScale = isEnlarged ? originalScale : originalScale * scaleMultiplier;
        Transform targetTransform = rectTransform != null ? rectTransform : transform;

        // Start the animation coroutine.
        StartCoroutine(AnimateScale(targetTransform, targetTransform.localScale, targetScale, animationDuration));

        // Toggle the state.
        isEnlarged = !isEnlarged;
    }

    // Coroutine to animate the scaling smoothly over a given duration.
    IEnumerator AnimateScale(Transform target, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(fromScale, toScale, elapsed / duration);
            yield return null;
        }
        target.localScale = toScale;
    }
}