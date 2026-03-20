using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifetime = 2f; // Thời gian sống (giây)

    private void Start()
    {
        // Hủy object sau khoảng thời gian 'lifetime'
        Destroy(gameObject, lifetime);
    }
}
