using UnityEngine;

public class WaterCollisionUI : MonoBehaviour
{
    public GameObject uiPrefab;   // Prefab UI (Image, Text,...)
    public Canvas canvas;         // Canvas chứa UI
    public Camera mainCamera;     // Camera chính

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnUIAtObject(other.transform.position);
        }
    }

    private void SpawnUIAtObject(Vector3 worldPos)
    {
        Debug.Log("Spawning UI at world position: " + worldPos);
        // Chuyển từ world → screen
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // Instantiate UI
        GameObject ui = Instantiate(uiPrefab, canvas.transform);

        // Gán vị trí UI
        RectTransform rect = ui.GetComponent<RectTransform>();
        rect.position = screenPos;
    }
}