using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BioDataController : MonoBehaviour
{
    [Header("UI Parts (Inspectorでセットしてね)")]
    public Slider pollutionSlider;       
    public Image pollutionFillImage;     
    public TextMeshProUGUI obstacleText; 
    public TextMeshProUGUI powerText;    
    public TextMeshProUGUI pollutionText;
    public LineRenderer powerLine;       

    [Header("Settings")]
    public Color safeColor = Color.green;
    public Color dangerColor = new Color(0.5f, 0, 1f); 
    
    private float[] powerHistory; 
    private int historySize = 50; 

    void Start()
    {
        // 波形の初期化
        powerHistory = new float[historySize];
        if (powerLine != null) powerLine.positionCount = historySize;
    }

   
    public void UpdateMonitor(float powerValue, float pollutionValue, int bicycleCount)
    {
        
        if (pollutionSlider != null)
        {
            
            float normalizedPollution = Mathf.Clamp01(pollutionValue / 50.0f);
            pollutionSlider.value = normalizedPollution;
            
           
            if (pollutionFillImage != null)
            {
                pollutionFillImage.color = Color.Lerp(safeColor, dangerColor, normalizedPollution);
            }
        }
        
        
        if (pollutionText != null) pollutionText.text = $"PM2.5: {pollutionValue:F1}";
        if (powerText != null) powerText.text = $"POWER: {powerValue:F0} kW";

       
        if (obstacleText != null)
        {
            obstacleText.text = $"BIKES: {bicycleCount}";
        }

        
        UpdateWaveform(powerValue);
    }

    void UpdateWaveform(float newValue)
    {
        if (powerLine == null) return;

       
        for (int i = 0; i < historySize - 1; i++)
        {
            powerHistory[i] = powerHistory[i + 1];
        }
        
        
        float height = (newValue / 5000.0f) * 5.0f; 
        powerHistory[historySize - 1] = height;

        
        for (int i = 0; i < historySize; i++)
        {
            
            float x = (i * 1.0f) - (historySize / 2.0f); 
            float y = powerHistory[i]; 
            
            
            powerLine.SetPosition(i, new Vector3(x, y, 0)); 
        }
    }
}
