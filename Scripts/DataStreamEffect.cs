using UnityEngine;
using TMPro; // 文字をいじるための呪文

public class DataStreamEffect : MonoBehaviour
{
    [Header("設定")]
    public bool isHexCode = true; // チェック入れると「F4 A1...」、外すと「数字」になる
    public float updateSpeed = 0.05f; // 数字が変わる速さ（小さいほど速い）

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

        // 指定した時間ごとに文字を書き換える
        if (_timer > updateSpeed)
        {
            if (isHexCode)
            {
                // 16進数パターン（謎のコード風）
                _text.text = "MEM_DUMP: " + GenerateHex(12);
            }
            else
            {
                // 緯度経度スキャン風
                float lat = 35.689f + Random.Range(-0.01f, 0.01f);
                float lon = 139.691f + Random.Range(-0.01f, 0.01f);
                _text.text = $"SCANNING...\nLAT: {lat:F6}\nLON: {lon:F6}";
            }
            _timer = 0;
        }
    }

    // ランダムな16進数（0F A3...）を作る魔法
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
