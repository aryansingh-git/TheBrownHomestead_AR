using UnityEngine;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(XROrigin))]
public class RecenterOrigin : MonoBehaviour
{
    public Transform target;

    public XROrigin xrOrigin;

    private void Awake()
    {
        xrOrigin = GetComponent<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("XROrigin component not found on this GameObject.");
        }
    }

    // Attach this method directly to your UI Button's OnClick event
    public void Recenter()
    {
        if (xrOrigin != null && target != null)
        {
            xrOrigin.MoveCameraToWorldLocation(target.position);
            xrOrigin.MatchOriginUpCameraForward(target.up, target.forward);
        }
        else
        {
            Debug.LogWarning("XROrigin or Target is not set.");
        }
    }
}
