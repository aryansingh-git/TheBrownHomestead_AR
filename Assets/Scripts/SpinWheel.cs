using UnityEngine;
using System.Collections;

public class SpinWheel : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float rotationSensitivity = 0.3f;
    public float snapAngle = 90f; // Angle increments to snap to
    public float snapSpeed = 10f; // Speed of snapping animation

    private Vector2 previousTouchPosition;
    private bool isSwiping = false;
    private Coroutine snapCoroutine;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            previousTouchPosition = Input.mousePosition;
            isSwiping = true;
            if (snapCoroutine != null) StopCoroutine(snapCoroutine);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
            SnapToNearestAngle();
        }
        else if (isSwiping)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            RotateWheel(previousTouchPosition, currentTouchPosition);
            previousTouchPosition = currentTouchPosition;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousTouchPosition = touch.position;
                isSwiping = true;
                if (snapCoroutine != null) StopCoroutine(snapCoroutine);
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 currentTouchPosition = touch.position;
                RotateWheel(previousTouchPosition, currentTouchPosition);
                previousTouchPosition = currentTouchPosition;
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isSwiping)
            {
                isSwiping = false;
                SnapToNearestAngle();
            }
        }
#endif
    }

    void RotateWheel(Vector2 previousPos, Vector2 currentPos)
    {
        float swipeDelta = currentPos.y - previousPos.y;
        float rotationAmount = swipeDelta * rotationSensitivity;
        transform.Rotate(0, 0, rotationAmount);
    }

    void SnapToNearestAngle()
    {
        float currentZ = transform.eulerAngles.z;
        float targetAngle = Mathf.Round(currentZ / snapAngle) * snapAngle;

        if (snapCoroutine != null) StopCoroutine(snapCoroutine);
        snapCoroutine = StartCoroutine(SmoothSnapRotation(targetAngle));
    }

    IEnumerator SmoothSnapRotation(float targetAngle)
    {
        float currentAngle = transform.eulerAngles.z;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * snapSpeed;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, elapsed);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle);
    }
}
