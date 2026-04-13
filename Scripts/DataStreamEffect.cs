using UnityEngine;
using TMPro; // 文字をいじる場所

public class DataStreamEffect : MonoBehaviour
{
    [Header("設定")]
    public bool isHexCode = true; 
    public float updateSpeed = 0.05f; 

    private TextMeshProUGUI _text;
    private float _timer;
    private string _randomString;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        
        if (_timer > updateSpeed)
        {
            if (isHexCode)
            {
               
                _text.text = "MEM_DUMP: " + GenerateHex(12);
            }
            else
            {
                // 緯度経度
                float lat = 35.689f + Random.Range(-0.01f, 0.01f);
                float lon = 139.691f + Random.Range(-0.01f, 0.01f);
                _text.text = $"SCANNING...\nLAT: {lat:F6}\nLON: {lon:F6}";
            }
            _timer = 0;
        }
    }

   
    string GenerateHex(int length)
    {
        string hex = "";
        string chars = "0123456789ABCDEF";
        for (int i = 0; i < length; i++)
        {
            hex += chars[Random.Range(0, chars.Length)] + chars[Random.Range(0, chars.Length)] + " ";
        }
        return hex;
    }
}
