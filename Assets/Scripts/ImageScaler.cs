using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ImageScaler : MonoBehaviour
{
    public Vector3 selectedScale = new Vector3(0.866114676f, 0.240728736f, 1.02791202f);
    public Vector3 deselectedScale;
    public float transitionSpeed = 10f;

    private void Start()
    {
        deselectedScale = selectedScale / 2;
        ScaleImages(transform.GetChild(0)); // Initial selection
        AttachButtonListeners();
    }

    void AttachButtonListeners()
    {
        foreach (Transform child in transform)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnImageSelected(btn.transform));
            }
        }
    }

    public void OnImageSelected(Transform selected)
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Vector3 targetScale = child == selected ? selectedScale : deselectedScale;
            StartCoroutine(SmoothScale(child, targetScale));
        }
    }

    IEnumerator SmoothScale(Transform target, Vector3 endScale)
    {
        while (Vector3.Distance(target.localScale, endScale) > 0.01f)
        {
            target.localScale = Vector3.Lerp(target.localScale, endScale, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        target.localScale = endScale;
    }

    void ScaleImages(Transform selected)
    {
        foreach (Transform child in transform)
        {
            child.localScale = child == selected ? selectedScale : deselectedScale;
        }
    }
}