using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextSwitcher : MonoBehaviour
{
    // Array of Text objects that should be faded out and turned off
    public Text[] textsToTurnOff;

    // The Text object (or UI element) that you want to fade in and turn on.
    public Text textToTurnOn;

    // Duration (in seconds) for the fade animations.
    public float fadeDuration = 1f;

    // This method should be hooked up to your Button's OnClick event.
    public void ToggleText()
    {
        StartCoroutine(ToggleTextsCoroutine());
    }

    // Coroutine that fades out each text in textsToTurnOff and fades in textToTurnOn.
    IEnumerator ToggleTextsCoroutine()
    {
        // Fade out and disable each text object in the array.
        foreach (Text t in textsToTurnOff)
        {
            if (t.gameObject.activeSelf)
            {
                yield return StartCoroutine(FadeText(t, t.color.a, 0f, fadeDuration));
                t.gameObject.SetActive(false);
            }
        }

        // Enable the text object to be turned on.
        textToTurnOn.gameObject.SetActive(true);

        // Set its alpha to 0 before beginning the fade in.
        Color startColor = textToTurnOn.color;
        startColor.a = 0f;
        textToTurnOn.color = startColor;

        // Fade in the textToTurnOn.
        yield return StartCoroutine(FadeText(textToTurnOn, 0f, 1f, fadeDuration));
    }

    // Coroutine to interpolate the alpha value of the given Text component.
    IEnumerator FadeText(Text textComp, float startAlpha, float endAlpha, float duration)
    {
        Color originalColor = textComp.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            textComp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        // Ensure the final alpha is set exactly.
        textComp.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}