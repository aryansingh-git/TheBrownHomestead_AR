using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class RoomPlacement : MonoBehaviour
{
    public GameObject roomPrefab;
    private GameObject spawnedRoom;
    private ARRaycastManager raycastManager;

    private Vector2 screenCenter;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        if (spawnedRoom != null) return;

        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            spawnedRoom = Instantiate(roomPrefab, hitPose.position, hitPose.rotation);
        }
    }
}