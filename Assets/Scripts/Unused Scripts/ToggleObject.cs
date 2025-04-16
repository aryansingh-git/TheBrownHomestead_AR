using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject targetObject;

    // Call this method to turn the object ON
    public void TurnOnObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    // Call this method to turn the object OFF
    public void TurnOffObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    // Optional: Toggle method
    public void ToggleGameObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}