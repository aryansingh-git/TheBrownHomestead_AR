using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe_Menu : MonoBehaviour
{
    [Header("UI References")]
    // Reference to the Scrollbar (from a ScrollRect or standalone Scrollbar)
    public Scrollbar scrollbar;

    [Header("Scaling Settings")]
    // Scale for the item that is selected (snapped to its position)
    public Vector2 selectedScale = new Vector2(1f, 1f);
    // Scale for the other (non-selected) items
    public Vector2 nonSelectedScale = new Vector2(0.8f, 0.8f);
    // Speed for snapping and scaling interpolation
    public float lerpSpeed = 0.1f;

    // Internal variables:
    private float scroll_pos = 0f; // Current scroll position value
    private float[] pos;          // Array of target snap positions (0 to 1)
    private int itemCount;        // Number of carousel items

    void Start()
    {
        // Cache the number of items (each carousel item is assumed to be a child of this GameObject)
        itemCount = transform.childCount;

        // Create an array for the snap positions.
        // For example, if there are 4 items, the positions will be [0, 0.33, 0.66, 1]
        pos = new float[itemCount];
        float distance = 1f / (itemCount - 1);
        for (int i = 0; i < itemCount; i++)
        {
            pos[i] = distance * i;
        }

        // *** Set the default selection to the rightmost item ***
        // (The rightmost item corresponds to the highest scrollbar value, which is 1, or pos[itemCount - 1])
        scrollbar.value = pos[itemCount - 1];
        scroll_pos = pos[itemCount - 1];
    }

    void Update()
    {
        // If the user is dragging (mouse button down), update the current scroll position.
        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.value;
        }
        else
        {
            // When not dragging, snap to the closest target position.
            for (int i = 0; i < pos.Length; i++)
            {
                // Calculate a snapping range (half the distance between items)
                float snapRange = (1f / (itemCount)) * 0.5f;
                if (scroll_pos < pos[i] + snapRange && scroll_pos > pos[i] - snapRange)
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], lerpSpeed);
                }
            }
        }

        // Scale the carousel items based on which one is closest to its snap position.
        for (int i = 0; i < pos.Length; i++)
        {
            float snapRange = (1f / (itemCount)) * 0.5f;
            if (scroll_pos < pos[i] + snapRange && scroll_pos > pos[i] - snapRange)
            {
                // This is the "selected" item: scale it up.
                Transform selectedItem = transform.GetChild(i);
                selectedItem.localScale = Vector2.Lerp(selectedItem.localScale, selectedScale, lerpSpeed);

                // Scale down all the other items.
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j != i)
                    {
                        Transform otherItem = transform.GetChild(j);
                        otherItem.localScale = Vector2.Lerp(otherItem.localScale, nonSelectedScale, lerpSpeed);
                    }
                }
            }
        }
    }
}