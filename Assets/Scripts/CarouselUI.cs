using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class CarouselUI : MonoBehaviour
{
    [Header("Carousel Settings")]
    public ScrollRect scrollRect;
    public float snapSpeed = 5f;
    public float scrollSensitivity = 10f;
    
    [Header("Item Settings")]
    public float itemSpacing = 50f;
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);
    public float scaleTransitionSpeed = 7f;
    
    // Internal variables
    private RectTransform contentRectTransform;
    private RectTransform viewportRectTransform;
    private int itemCount;
    private float[] itemPositions;
    private int currentIndex = 0;
    private bool isDragging = false;
    private Coroutine snapCoroutine;
    private Button[] itemButtons;
    
    private void Awake()
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect reference is missing!");
            return;
        }
        
        contentRectTransform = scrollRect.content;
        viewportRectTransform = scrollRect.viewport as RectTransform;
        
        // Setup scroll events
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        
        // Adjust scroll sensitivity
        scrollRect.scrollSensitivity = scrollSensitivity;
    }
    
    private void Start()
    {
        // Setup carousel after a frame to ensure UI layout is computed
        StartCoroutine(SetupCarouselDelayed());
    }
    
    private IEnumerator SetupCarouselDelayed()
    {
        // Wait for one frame to ensure RectTransforms are properly initialized
        yield return null;
        
        SetupCarousel();
        
        // Start at first item
        SetSelectedItem(0, true);
    }
    
    private void SetupCarousel()
    {
        // Count only the actual content items (not padding/spacers)
        itemCount = contentRectTransform.childCount;
        
        if (itemCount == 0)
        {
            Debug.LogWarning("No items found in the carousel content!");
            return;
        }
        
        // Get all buttons and cache them
        itemButtons = new Button[itemCount];
        for (int i = 0; i < itemCount; i++)
        {
            Transform child = contentRectTransform.GetChild(i);
            itemButtons[i] = child.GetComponent<Button>();
            
            // Set initial scale to normal
            child.localScale = normalScale;
            
            // Add button click listener
            int index = i; // Cache the index for the lambda
            if (itemButtons[i] != null)
            {
                itemButtons[i].onClick.AddListener(() => OnItemClicked(index));
            }
        }
        
        // Calculate positions for each item
        CalculateItemPositions();
    }
    
    private void CalculateItemPositions()
    {
        itemPositions = new float[itemCount];
        
        float viewportWidth = viewportRectTransform.rect.width;
        float contentWidth = contentRectTransform.rect.width;
        
        // If there's only one item, center it
        if (itemCount == 1)
        {
            itemPositions[0] = 0.5f;
            return;
        }
        
        // Calculate normalized positions (0-1)
        for (int i = 0; i < itemCount; i++)
        {
            RectTransform itemRect = contentRectTransform.GetChild(i) as RectTransform;
            float halfItemWidth = itemRect.rect.width / 2;
            
            // Position from left edge plus half item width to center
            float rawPos = itemRect.anchoredPosition.x + halfItemWidth;
            
            // Convert to normalized position (0-1)
            itemPositions[i] = Mathf.Clamp01(rawPos / contentWidth);
        }
    }
    
    private void OnScrollValueChanged(Vector2 position)
    {
        if (!isDragging && snapCoroutine == null)
        {
            // Find and select the closest item
            int closestIndex = GetClosestItemIndex(position.x);
            UpdateItemScales(closestIndex);
        }
    }
    
    private int GetClosestItemIndex(float normalizedPosition)
    {
        float minDistance = float.MaxValue;
        int closestIndex = 0;
        
        for (int i = 0; i < itemCount; i++)
        {
            float distance = Mathf.Abs(itemPositions[i] - normalizedPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }
    
    private void UpdateItemScales(int selectedIndex)
    {
        for (int i = 0; i < itemCount; i++)
        {
            Transform item = contentRectTransform.GetChild(i);
            Vector3 targetScale = (i == selectedIndex) ? selectedScale : normalScale;
            
            // Smooth scaling transition
            StartCoroutine(SmoothScale(item, targetScale));
        }
    }
    
    private IEnumerator SmoothScale(Transform item, Vector3 targetScale)
    {
        Vector3 startScale = item.localScale;
        float t = 0;
        
        while (t < 1f)
        {
            t += Time.deltaTime * scaleTransitionSpeed;
            item.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        
        item.localScale = targetScale;
    }
    
    private void OnItemClicked(int index)
    {
        SetSelectedItem(index, true);
    }
    
    public void SetSelectedItem(int index, bool animate = true)
    {
        if (index < 0 || index >= itemCount)
        {
            Debug.LogWarning($"Invalid item index: {index}. Should be between 0 and {itemCount-1}");
            return;
        }
        
        currentIndex = index;
        UpdateItemScales(currentIndex);
        
        // Stop any running snap coroutine
        if (snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
        }
        
        // Snap to the new position
        if (animate)
        {
            snapCoroutine = StartCoroutine(SmoothSnapTo(index));
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = itemPositions[index];
        }
    }
    
    private IEnumerator SmoothSnapTo(int index)
    {
        float startPosition = scrollRect.horizontalNormalizedPosition;
        float targetPosition = itemPositions[index];
        float t = 0;
        
        while (t < 1f)
        {
            t += Time.deltaTime * snapSpeed;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        
        scrollRect.horizontalNormalizedPosition = targetPosition;
        snapCoroutine = null;
    }
    
    // Public methods for controlling the carousel externally
    
    public void NextItem()
    {
        SetSelectedItem((currentIndex + 1) % itemCount);
    }
    
    public void PreviousItem()
    {
        SetSelectedItem((currentIndex - 1 + itemCount) % itemCount);
    }
    
    public void GoToItem(int index)
    {
        SetSelectedItem(Mathf.Clamp(index, 0, itemCount - 1));
    }
    
    // Implement drag handling
    
    public void OnBeginDrag()
    {
        isDragging = true;
        if (snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
            snapCoroutine = null;
        }
    }
    
    public void OnEndDrag()
    {
        isDragging = false;
        int closestIndex = GetClosestItemIndex(scrollRect.horizontalNormalizedPosition);
        SetSelectedItem(closestIndex);
    }
    
    // Add event trigger components in the inspector and connect to these methods
    // or add them in code with the following setup method
    
    public void SetupEventTriggers()
    {
        // Add event triggers if not added in the Inspector
        EventTrigger eventTrigger = scrollRect.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = scrollRect.gameObject.AddComponent<EventTrigger>();
        }
        
        // Add begin drag event
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDrag(); });
        eventTrigger.triggers.Add(beginDragEntry);
        
        // Add end drag event
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDrag(); });
        eventTrigger.triggers.Add(endDragEntry);
    }
} 