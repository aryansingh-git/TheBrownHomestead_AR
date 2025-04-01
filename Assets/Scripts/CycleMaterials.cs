using UnityEngine;

public class ChangeToSpecificMaterial : MonoBehaviour
{
    // Drag your room object here
    public GameObject roomModel;

    // Drag your materials here
    public Material[] roomMaterials;

    private Renderer roomRenderer;

    void Start()
    {
        roomRenderer = roomModel.GetComponent<Renderer>();

        // Optionally set an initial material
        if (roomMaterials.Length > 0)
            roomRenderer.material = roomMaterials[0];
    }

    // Call this method with the index of the material you want to change to
    public void ChangeMaterialTo(int materialIndex)
    {
        if (roomMaterials.Length == 0 || materialIndex < 0 || materialIndex >= roomMaterials.Length)
            return;

        roomRenderer.material = roomMaterials[materialIndex];
    }
}
