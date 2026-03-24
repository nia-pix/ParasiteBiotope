using UnityEngine;
using TMPro; // 文字をいじるための機能

using System; // 日付計算のための便利な機能

public class TimeManager : MonoBehaviour
{
    [Header("UIのテキストをここにセット")]
    public TextMeshProUGUI timeText;

    [Header("開始する日時 (yyyy/MM/dd HH:mm:ss)")]
    // ここで好きな日付からスタートできるよ！
    public string startDateData = "2025/02/01 10:00:00"; 
    
    [Header("時間の速さ (倍速)")]
    // 1.0なら現実と同じ。60.0なら1秒で1分進む。3600なら1秒で1時間！
    public float timeSpeedMultiplier = 3600.0f; 

    private DateTime currentDateTime;

    void Start()
    {
        // 文字列を「日付データ」に変換してセット！
        if (!DateTime.TryParse(startDateData, out currentDateTime))
        {
            currentDateTime = DateTime.Now; // エラーなら現在時刻にする
        }
    }

    void Update()
    {
        // 時間を進める（現実の1秒 × 倍率 ＝ 進む秒数）
        double secondsToAdd = Time.deltaTime * timeSpeedMultiplier;
        currentDateTime = currentDateTime.AddSeconds(secondsToAdd);

        // --- 文字の表示を更新 ---
        // yyyy(年) . MM(月) . dd(日)   HH(時) : mm(分)
        timeText.text = currentDateTime.ToString("yyyy.MM.dd   HH:mm");
    }
}
