using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPMessage3D : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshPro;
    [SerializeField] private List<Color> colorPalette; // Danh sách màu trong Inspector
    [SerializeField] private float displayTime = 2f;    // Thời gian giữ màu
    [SerializeField] private float fadeDuration = 1f;   // Thời gian mờ dần

    private void Start()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        Color randomColor = colorPalette[Random.Range(0, colorPalette.Count)];
        _textMeshPro.color = randomColor;


        StartCoroutine(TextFade());
    }

    private IEnumerator TextFade()
    {
        yield return new WaitForSeconds(displayTime);

        // Fade out
        float elapsed = 0f;
        Color startColor = _textMeshPro.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            _textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

    }
}
