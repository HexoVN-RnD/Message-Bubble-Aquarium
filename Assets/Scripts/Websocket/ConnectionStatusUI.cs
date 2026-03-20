using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Hiển thị trạng thái kết nối WebSocket lên UI.
/// Gắn vào một Text/TMP_Text GameObject trong Canvas.
/// </summary>
public class ConnectionStatusUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text statusText;
    public Image statusDot; // Circle Image để thể hiện trạng thái màu sắc

    [Header("Colors")]
    public Color connectedColor = new Color(0.2f, 0.9f, 0.4f);
    public Color disconnectedColor = new Color(0.9f, 0.2f, 0.2f);
    public Color connectingColor = new Color(0.9f, 0.8f, 0.1f);

    void OnEnable()
    {
        WebSocketManager.OnConnected += HandleConnected;
        WebSocketManager.OnDisconnected += HandleDisconnected;
    }

    void OnDisable()
    {
        WebSocketManager.OnConnected -= HandleConnected;
        WebSocketManager.OnDisconnected -= HandleDisconnected;
    }

    void Start()
    {
        SetStatus("Đang kết nối...", connectingColor);
    }

    void HandleConnected()
    {
        SetStatus("✓ Đã kết nối", connectedColor);
    }

    void HandleDisconnected()
    {
        SetStatus("✗ Mất kết nối - Đang thử lại...", disconnectedColor);
    }

    void SetStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
        if (statusDot != null)
            statusDot.color = color;
    }
}
