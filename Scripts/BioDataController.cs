using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BioDataController : MonoBehaviour
{
    [Header("UI Parts (Inspectorでセットしてね)")]
    public Slider pollutionSlider;       // 汚染ゲージ
    public Image pollutionFillImage;     // ゲージの中身の色
    public TextMeshProUGUI obstacleText; // 自転車カウントのテキスト
    public TextMeshProUGUI powerText;    // ★追加: 電力の数値テキスト
    public TextMeshProUGUI pollutionText;// ★追加: 汚染の数値テキスト
    public LineRenderer powerLine;       // 電力の波形

    [Header("Settings")]
    public Color safeColor = Color.green;
    public Color dangerColor = new Color(0.5f, 0, 1f); // 毒の紫色

    // 内部データ
    private float[] powerHistory; // 波形の履歴
    private int historySize = 50; // 点の数

    void Start()
    {
        // 波形の初期化
        powerHistory = new float[historySize];
        if (powerLine != null) powerLine.positionCount = historySize;
    }

    // ★ここが大事！SystemDirectorから呼ばれる命令
    public void UpdateMonitor(float powerValue, float pollutionValue, int bicycleCount)
    {
        // 1. 汚染ゲージの更新
        if (pollutionSlider != null)
        {
            // 仮に最大50として計算（データの大きさに合わせて調整してね）
            float normalizedPollution = Mathf.Clamp01(pollutionValue / 50.0f);
            pollutionSlider.value = normalizedPollution;
            
            // 色の変化（緑 -> 紫）
            if (pollutionFillImage != null)
            {
                pollutionFillImage.color = Color.Lerp(safeColor, dangerColor, normalizedPollution);
            }
        }
        
        // ★数値テキストの更新（ここも追加！）
        if (pollutionText != null) pollutionText.text = $"PM2.5: {pollutionValue:F1}";
        if (powerText != null) powerText.text = $"POWER: {powerValue:F0} kW";

        // 2. 障害物カウントの更新
        if (obstacleText != null)
        {
            obstacleText.text = $"BIKES: {bicycleCount}";
        }

        // 3. 電力波形の更新（心電図エフェクト）
        UpdateWaveform(powerValue);
    }

    void UpdateWaveform(float newValue)
    {
        if (powerLine == null) return;

        // 履歴を1つ左にずらす
        for (int i = 0; i < historySize - 1; i++)
        {
            powerHistory[i] = powerHistory[i + 1];
        }
        
        // 新しい値を右端に入れる（高さ調整：5000kWで最大になるようにしてみた）
        float height = (newValue / 5000.0f) * 5.0f; 
        powerHistory[historySize - 1] = height;

        // LineRendererに座標をセット
        for (int i = 0; i < historySize; i++)
        {
            // x: 横に並べる, y: 電力の高さ
            // 画面の真ん中あたりに来るようにオフセット調整
            float x = (i * 1.0f) - (historySize / 2.0f); 
            float y = powerHistory[i]; 
            
            // Z軸は0でOK（Canvasの設定による）
            powerLine.SetPosition(i, new Vector3(x, y, 0)); 
        }
    }
}
