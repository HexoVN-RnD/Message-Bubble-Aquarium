using UnityEngine;

public class TopWaterCollision : MonoBehaviour
{
    public GameObject uiPrefab; // Kéo thả UI Prefab (Canvas Element) vào đây
    private Canvas mainCanvas;

    private void Start()
    {
        // Tìm Canvas trong Scene để làm cha cho UI mới
        mainCanvas = FindFirstObjectByType<Canvas>();
        Debug.Log($"[TopWaterCollision] Canvas found: {mainCanvas != null}");
        Debug.Log($"[TopWaterCollision] UI Prefab assigned: {uiPrefab != null}");
        Debug.Log($"[TopWaterCollision] Script enabled: {enabled}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[TopWaterCollision] OnCollisionEnter - Object name: {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("TopWaterSurface"))
        {
            Debug.Log("Va chạm với mặt nước trên cùng!");

            if (mainCanvas == null)
            {
                Debug.LogError("[TopWaterCollision] mainCanvas is NULL! Cannot create UI.");
                return;
            }

            if (uiPrefab == null)
            {
                Debug.LogError("[TopWaterCollision] uiPrefab is NULL! Assign it in Inspector.");
                return;
            }

            // 1. Lấy điểm va chạm đầu tiên
            Vector3 hitPoint = collision.contacts[0].point;

            // 2. Chuyển từ vị trí 3D sang vị trí trên màn hình (Pixels)
            Vector2 screenPos = Camera.main.WorldToScreenPoint(hitPoint);

            // 3. Tạo UI prefab
            GameObject uiInstance = Instantiate(uiPrefab, mainCanvas.transform);

            // 4. Gán vị trí cho UI (sử dụng RectTransform thay vì transform.position)
            RectTransform rectTransform = uiInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPos;

            Debug.Log($"[TopWaterCollision] UI created at screen position: {screenPos}");
        }
        else
        {
            Debug.Log($"[TopWaterCollision] Tag mismatch! Expected 'TopWaterSurface', got '{collision.gameObject.tag}'");
        }
    }
}
