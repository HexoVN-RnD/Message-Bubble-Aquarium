using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu dùng TextMeshPro; đổi thành UnityEngine.UI.Text nếu dùng Legacy Text

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

    [Header("Prefab Settings")]
    [Tooltip("Prefab chứa Panel, Image, và TextMeshPro")]
    public GameObject dataPanelPrefab;

    // ─── Dữ liệu nhận từ WebSocket ───
    private string _text;
    private byte[] _imageBytes;
    private Sprite _sprite;
    private Canvas _targetCanvas;

    // ─── UI References (sẽ được tạo động) ───
    private GameObject _uiPanel;
    private bool _uiShown = false;

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

    IEnumerator LoadImageFromBytes(byte[] bytes)
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

    void OnCollisionEnter(Collision collision)
    {
        if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || collision.gameObject.CompareTag(collisionTag)))
        {
            ShowUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || other.CompareTag(collisionTag)))
        {
            ShowUI();
        }
    }

    // ─────────────────────────────────────────────
    //  VA CHẠM 2D (bỏ comment nếu dùng 2D)
    // ─────────────────────────────────────────────

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || collision.gameObject.CompareTag(collisionTag)))
    //         ShowUI();
    // }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!_uiShown && (string.IsNullOrEmpty(collisionTag) || other.CompareTag(collisionTag)))
    //         ShowUI();
    // }

    // ─────────────────────────────────────────────
    //  HIỂN THỊ UI TRÊN CANVAS
    // ─────────────────────────────────────────────

    void ShowUI()
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
        _uiPanel = Instantiate(dataPanelPrefab, _targetCanvas.transform, false);

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

    IEnumerator AutoDismiss()
    {
        yield return new WaitForSeconds(autoDismissDelay);
        HideUI();
    }

    void OnDestroy()
    {
        HideUI();
    }
}
