using UnityEngine;
using UnityEngine.UI; 

public class SimpleGlitchUI : MonoBehaviour
{
    [Header("激しさ設定")]
    [Tooltip("揺れる幅")]
    public float shakeAmount = 5.0f; 

    [Tooltip("0.0〜1.0")]
    [Range(0, 1)]
    public float glitchChance = 0.1f;

    [Header("色変え")]
    public bool useColorGlitch = true;
    public Color glitchColor = Color.cyan; //

    private RectTransform _rectTransform;
    private Vector2 _originalPos;
    private RawImage _rawImage;
    private Color _originalColor;

    void Start()
    {
       
        _rectTransform = GetComponent<RectTransform>();
        _originalPos = _rectTransform.anchoredPosition;
        
        _rawImage = GetComponent<RawImage>();
        if (_rawImage != null) _originalColor = _rawImage.color;
    }

    void Update()
    {
      
        _rectTransform.anchoredPosition = _originalPos;
        if (_rawImage != null) _rawImage.color = _originalColor;

       
        if (Random.value < glitchChance)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);
            _rectTransform.anchoredPosition += new Vector2(x, y);

            if (useColorGlitch && _rawImage != null)
            {
           
                _rawImage.color = Color.Lerp(_originalColor, glitchColor, Random.Range(0.2f, 0.8f));
            }
        }
    }
}
