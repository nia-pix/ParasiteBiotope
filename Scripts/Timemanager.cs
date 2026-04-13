using UnityEngine;
using TMPro; 

using System; // 日付計算

public class TimeManager : MonoBehaviour
{
    [Header("UIのテキストセット")]
    public TextMeshProUGUI timeText;

    [Header("開始する日時")]
    
    public string startDateData = "2025/02/01 10:00:00"; 
    
    [Header("時間の速さ")]
    
    public float timeSpeedMultiplier = 3600.0f; 

    private DateTime currentDateTime;

    void Start()
    {
       
        if (!DateTime.TryParse(startDateData, out currentDateTime))
        {
            currentDateTime = DateTime.Now;
        }
    }

    void Update()
    {
        
        double secondsToAdd = Time.deltaTime * timeSpeedMultiplier;
        currentDateTime = currentDateTime.AddSeconds(secondsToAdd);

        
        timeText.text = currentDateTime.ToString("yyyy.MM.dd   HH:mm");
    }
}
