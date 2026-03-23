using System.Collections;
using TMPro; // Nếu dùng TextMeshPro; đổi thành UnityEngine.UI.Text nếu dùng Legacy Text
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gắn script này vào GameObject Prefab.
/// Lưu dữ liệu nhận từ WebSocket và hiển thị lên Canvas khi va chạm.
/// </summary>
[RequireComponent(typeof(Collider))] // hoặc Collider2D nếu dùng 2D
public class ObjectDataHolder : MonoBehaviour
{
    [Header("Collision UI Settings")]
    [Tooltip("Tên tag của object sẽ trigger hiển thị UI (ví dụ: 'Player' hoặc 'Ground')")]
    public string collisionTag = "Player";
    [Tooltip("Tự động ẩn UI sau bao nhiêu giây (0 = không tự ẩn)")]
    public float autoDismissDelay = 5f;

    [SerializeField] private MeshRenderer renderer;

    [Header("Prefab Settings")]
    [Tooltip("Prefab chứa Panel, Image, và TextMeshPro")]
    public GameObject dataPanelPrefab;
    public GameObject effectPrefab;

    // ─── Dữ liệu nhận từ WebSocket ───
    private string _text;
    private byte[] _imageBytes;
    private Sprite _sprite;
    private Canvas _targetCanvas;

    // ─── UI References (sẽ được tạo động) ───
    private GameObject _uiPanel;
    private bool _uiShown = false;
    private Collider _collidingObject;
    private Camera _mainCamera;

    // ─── Gọi từ WebSocketManager sau khi Instantiate ───
    public void Initialize(ReceivedData data, Canvas canvas)
    {
        _text = data.text;
        _imageBytes = data.imageBytes;
        _targetCanvas = canvas;

        if (_imageBytes != null && _imageBytes.Length > 0)
            StartCoroutine(LoadImageFromBytes(_imageBytes));
    }

    // ─────────────────────────────────────────────
    //  LOAD TEXTURE TỪ BYTES
    // ─────────────────────────────────────────────

    private IEnumerator LoadImageFromBytes(byte[] bytes)
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool loaded = tex.LoadImage(bytes); // Hỗ trợ PNG/JPG
        if (loaded)
        {
            _sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
            Debug.Log($"[ObjectDataHolder] ✅ Đã load ảnh: {tex.width}x{tex.height}");
        }
        else
        {
            Debug.LogWarning("[ObjectDataHolder] ❌ Không thể load ảnh từ bytes.");
        }
        yield return null;
    }

    // ─────────────────────────────────────────────
    //  VA CHẠM 3D
    // ─────────────────────────────────────────────

    private void OnCollisionEnter(Collision collision)
    {
        if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || collision.gameObject.CompareTag(collisionTag)))
        {
            _collidingObject = collision.collider;
            InstantiateEffect();
            TriggerWaterEffect();
            renderer.enabled = false;
            ShowUI();
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || other.CompareTag(collisionTag)))
    //    {
    //        _collidingObject = other;
    //        TriggerWaterEffect();
    //        ShowUI();
    //    }
    //}

    private void TriggerWaterEffect()
    {
        VolumeBlender blender = FindAnyObjectByType<VolumeBlender>();
        if (blender != null)
        {
            blender.TriggerWaterEffect();
        }
        else
        {
            Debug.LogWarning("[ObjectDataHolder] VolumeBlender not found in scene!");
        }
    }

    private void InstantiateEffect()
    {
        if (effectPrefab != null)
        {
            Vector3 effectPosition = transform.position; // Hoặc có thể là vị trí va chạm nếu cần
            GameObject effectInstance = Instantiate(effectPrefab, effectPosition, Quaternion.identity);
        }
    }


    private void ShowUI()
    {
        if (_targetCanvas == null)
        {
            Debug.LogWarning("[ObjectDataHolder] Chưa gán targetCanvas!");
            return;
        }

        if (dataPanelPrefab == null)
        {
            Debug.LogWarning("[ObjectDataHolder] Chưa gán dataPanelPrefab!");
            return;
        }

        _uiShown = true;
        InstantiateWithObjectPosition();
        void InstantiateWithObjectPosition()
        {
            // 1. Tạo UI Prefab
            _uiPanel = Instantiate(dataPanelPrefab);

            // 2. Gán cha (Canvas). Dùng false để giữ nguyên thông số Local của Prefab
            _uiPanel.transform.SetParent(_targetCanvas.transform, false);

            // 3. Lấy vị trí của CHÍNH VẬT THỂ NÀY (World Space)
            Vector3 worldPos = transform.position;

            // 4. Chuyển sang tọa độ màn hình
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // KIỂM TRA: Nếu vật thể nằm sau lưng Camera (z < 0), ScreenPoint vẫn trả về giá trị
            // nhưng thường sẽ bị đảo ngược hoặc sai lệch.
            if (screenPos.z > 0)
            {
                screenPos.z = 0; // Canvas Overlay không dùng trục Z
                _uiPanel.transform.position = screenPos;
                _uiPanel.SetActive(true);
            }
            else
            {
                // Nếu vật thể ở sau lưng camera, tạm ẩn UI đi
                _uiPanel.SetActive(false);
            }
        }

        AssignComponent();
        void AssignComponent()
        {
            // Tìm Image component và gán sprite
            Image imageComponent = _uiPanel.GetComponentInChildren<Image>(true);
            if (imageComponent != null && _sprite != null)
            {
                imageComponent.sprite = _sprite;
                imageComponent.preserveAspect = true;
            }

            // Tìm TextMeshProUGUI component và gán text
            TMP_Text textComponent = _uiPanel.GetComponentInChildren<TMP_Text>(true);
            if (textComponent != null && !string.IsNullOrEmpty(_text))
            {
                textComponent.text = _text;
            }

            // Tìm Button để xử lý sự kiện đóng
            Button closeButton = _uiPanel.GetComponentInChildren<Button>(true);
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideUI);
            }
        }

        if (autoDismissDelay > 0)
            StartCoroutine(AutoDismiss());

        Debug.Log($"[ObjectDataHolder] 🖼️ Hiển thị UI: '{_text}'");


    }

    public void HideUI()
    {
        if (_uiPanel != null)
        {
            Destroy(_uiPanel);
            _uiPanel = null;
            _uiShown = false;
        }
    }

    private IEnumerator AutoDismiss()
    {
        yield return new WaitForSeconds(autoDismissDelay);
        HideUI();
    }

    private void OnDestroy()
    {
        HideUI();
    }
}
