using UnityEngine;
using UnityEngine.UI; // UIをいじるための呪文

public class SimpleGlitchUI : MonoBehaviour
{
    [Header("激しさ設定")]
    [Tooltip("揺れる幅（大きくすると激しくなる）")]
    public float shakeAmount = 5.0f; 

    [Tooltip("グリッチが起きる確率（0.0〜1.0）")]
    [Range(0, 1)]
    public float glitchChance = 0.1f;

    [Header("色変え設定")]
    public bool useColorGlitch = true;
    public Color glitchColor = Color.cyan; //

    private RectTransform _rectTransform;
    private Vector2 _originalPos;
    private RawImage _rawImage;
    private Color _originalColor;

    void Start()
    {
        // 最初の場所と色を覚えておく
        _rectTransform = GetComponent<RectTransform>();
        _originalPos = _rectTransform.anchoredPosition;
        
        _rawImage = GetComponent<RawImage>();
        if (_rawImage != null) _originalColor = _rawImage.color;
    }

    void Update()
    {
        // 1. まず元の位置に戻す（リセット）
        _rectTransform.anchoredPosition = _originalPos;
        if (_rawImage != null) _rawImage.color = _originalColor;

        // 2. サイコロを振って、運が悪ければバグらせる
        if (Random.value < glitchChance)
        {
            // 位置をランダムにずらす（振動）
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);
            _rectTransform.anchoredPosition += new Vector2(x, y);

            // 色をちょっと変える（点滅）
            if (useColorGlitch && _rawImage != null)
            {
                // 元の色と、グリッチ色をランダムに混ぜる
                _rawImage.color = Color.Lerp(_originalColor, glitchColor, Random.Range(0.2f, 0.8f));
            }
        }
    }
}