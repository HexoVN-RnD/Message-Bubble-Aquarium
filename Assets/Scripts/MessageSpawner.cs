using TMPro;
using UnityEngine;

/// <summary>
/// Khi nhận được message, Instantiate 1 prefab tại vị trí chỉ định
/// và gán nội dung vào TextMeshPro bên trong prefab.
/// </summary>
public class MessageSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("Prefab phải có TextMeshPro ở bất kỳ đâu trong children")]
    [SerializeField] private GameObject messagePrefab;

    [Header("Spawn")]
    [SerializeField] private Collider spawnArea;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    public System.Action OnMessageShown;

    public void ShowMessage(string message)
    {
        if (messagePrefab == null)
        {
            Debug.LogWarning("[MessageSpawner] Chưa gán Message Prefab!");
            return;
        }

        GameObject instance = SpawnBubble();

        OnMessageShown?.Invoke();

        AssignText(message, instance);

        static void AssignText(string message, GameObject instance)
        {
            TextMeshPro tmp = instance.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = message;
            }
            else
            {
                Debug.LogWarning("[MessageSpawner] Không tìm thấy TextMeshPro trong prefab!");
            }
        }
    }

    private GameObject SpawnBubble()
    {
        Vector3 position = new Vector3
        {
            x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            y = this.transform.position.y,
            z = this.transform.position.z
        };
        GameObject instance = Instantiate(messagePrefab, position, Quaternion.identity);
        return instance;
    }
}
