using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class TimeLoopSpawner : MonoBehaviour
{
    [Header("--- UI References ---")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI detailText;
    public TextMeshProUGUI bicycleText;
    public TextMeshProUGUI pollutionText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI geoText;
    public TextMeshProUGUI hexCodeText;

    [Header("--- File 1: Accident Data (Events) ---")]
    public TextAsset accidentCsvFile; // 事故データ (accidents.csv)
    // 事故データの列番号（自分のCSVに合わせて調整してね！）
    public int acc_Year = 10;
    public int acc_Month = 11;
    public int acc_Day = 12;
    public int acc_Hour = 13;
    public int acc_Minute = 14;
    public int acc_Lat = 26; // Z列あたり？確認してね
    public int acc_Lon = 27; // AA列あたり？
    public int acc_Dead = 5;
    public int acc_Injured = 6;

    [Header("--- File 2: Env Data (Background) ---")]
    public TextAsset envCsvFile; // 環境データ (allcomp.csv / env_data.csv)
    // 環境データの列番号（allcompに合わせてね！）
    public int env_Year = 0;   // A列がTimeIDなら使わないかも？ 日付がある列
    public int env_Month = 0;  // ※allcompは日付と時間が分かれてる？
    // 面倒だから「TimeID」を使わず、行番号で管理するか、
    // あるいは「日時」をちゃんと読むか。
    // ★今回は簡単にするため、allcompの「A列(TimeID)」と「J,K,L列」を使うよ！
    
    // allcompの列番号設定
    public int col_Env_TimeID = 0; // A列
    public int col_Env_Power = 8;  // I列 (9番目なのでインデックスは8)
    public int col_Env_Temp = 9;   // J列
    public int col_Env_Rain = 10;  // K列
    public int col_Env_PM25 = 11;  // L列

    [Header("--- Settings ---")]
    public float timeSpeed = 1000.0f;
    public GameObject[] prefabsToSpawn;

    // 内部データ構造
    private List<AccidentData> accidentList = new List<AccidentData>();
    private List<EnvData> envList = new List<EnvData>();
    
    private DateTime currentTime;
    private int accidentCursor = 0;
    
    // データ保持用のクラス
    class AccidentData : IComparable<AccidentData> {
        public DateTime time;
        public Vector3 position;
        public int dead;
        public int injured;
        public double lat, lon;
        public int CompareTo(AccidentData other) { return time.CompareTo(other.time); }
    }

    class EnvData {
        public string timeID; // マッチング用のID (例: "2023-4-1-10")
        public float power;
        public float temp;
        public float rain;
        public float pm25;
    }

    void Start()
    {
        LoadAccidents(); // 事故データを読む
        LoadEnvironment(); // 環境データを読む

        if (accidentList.Count > 0)
            currentTime = accidentList[0].time;
        else
            currentTime = new DateTime(2023, 1, 1, 0, 0, 0);
    }

    void Update()
    {
        // 1. 時間を進める
        currentTime = currentTime.AddMinutes(Time.deltaTime * timeSpeed);

        // 2. 時計更新
        if (dateText != null) dateText.text = currentTime.ToString("yyyy.MM.dd\nHH:mm");

        // 3. 環境データを検索して適用（今の時間に合うデータを探す）
        UpdateEnvironmentUI();

        // 4. 自転車などのランダム演出
        UpdateRandomEffects();

        // 5. 事故発生チェック
        SpawnAccidents();
    }

    // --- 事故データを読み込む ---
    void LoadAccidents()
    {
        if (accidentCsvFile == null) return;
        string[] lines = accidentCsvFile.text.Split('\n');
        
        for (int i = 1; i < lines.Length; i++) {
            string[] row = lines[i].Split(',');
            if (row.Length < 20) continue; // 列が足りない行は飛ばす

            try {
                int y = int.Parse(row[acc_Year]);
                int m = int.Parse(row[acc_Month]);
                int d = int.Parse(row[acc_Day]);
                int h = int.Parse(row[acc_Hour]);
                int min = int.Parse(row[acc_Minute]);
                
                AccidentData data = new AccidentData();
                data.time = new DateTime(y, m, d, h, min, 0);
                
                // 座標変換 (緯度経度 -> Unity座標) ※計算式は仮
                double lat = double.Parse(row[acc_Lat]);
                double lon = double.Parse(row[acc_Lon]);
                data.lat = lat;
                data.lon = lon;
                // ここで本来は Mapbox等の変換式を入れる
                data.position = new Vector3((float)(lon - 139.7) * 1000, 0, (float)(lat - 35.6) * 1000); 

                data.dead = int.Parse(row[acc_Dead]);
                data.injured = int.Parse(row[acc_Injured]);

                accidentList.Add(data);
            } catch { continue; }
        }
        accidentList.Sort(); // 時間順に並べ替え
        Debug.Log($"事故データ読み込み完了: {accidentList.Count}件");
    }

    // --- 環境データ(allcomp)を読み込む ---
    void LoadEnvironment()
    {
        if (envCsvFile == null) return;
        string[] lines = envCsvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) {
            string[] row = lines[i].Split(',');
            if (row.Length < 10) continue;

            try {
                EnvData env = new EnvData();
                env.timeID = row[col_Env_TimeID]; // A列のTimeIDをそのまま使う
                
                // 数値が #N/A だったり空だったりする場合の対策
                float.TryParse(row[col_Env_Power], out env.power);
                float.TryParse(row[col_Env_Temp], out env.temp);
                float.TryParse(row[col_Env_Rain], out env.rain);
                float.TryParse(row[col_Env_PM25], out env.pm25);

                envList.Add(env);
            } catch { continue; }
        }
        Debug.Log($"環境データ読み込み完了: {envList.Count}件");
    }

    // --- 今の時間に合う環境データをUIに出す ---
    void UpdateEnvironmentUI()
    {
        // 今の時間から IDを作る (例: 2023-4-1-10)
        string currentID = $"{currentTime.Year}-{currentTime.Month}-{currentTime.Day}-{currentTime.Hour}";
        
        // リストの中からIDが一致するものを探す (Find)
        // ※毎回検索すると重いけど、データ数数千件ならPCなら余裕！
        EnvData currentEnv = envList.Find(x => x.timeID == currentID);

        if (currentEnv != null)
        {
            if (powerText != null) powerText.text = $"POWER: {currentEnv.power:N0} kW";
            if (pollutionText != null) pollutionText.text = $"PM2.5: {currentEnv.pm25:F1}";
            // 雨データを使って何かするならここで！
        }
    }

    void UpdateRandomEffects()
    {
        if (bicycleText != null)
        {
             float noise = Mathf.PerlinNoise(Time.time * 0.5f, 0.0f); 
             int bikeCount = Mathf.RoundToInt(Mathf.Lerp(500, 1500, noise));
             bicycleText.text = $"BIKES: {bikeCount}";
        }
    }

    void SpawnAccidents()
    {
        while (accidentCursor < accidentList.Count)
        {
            AccidentData data = accidentList[accidentCursor];
            if (data.time <= currentTime)
            {
                // 植物生成！
                GameObject prefab = prefabsToSpawn[UnityEngine.Random.Range(0, prefabsToSpawn.Length)];
                Instantiate(prefab, data.position, Quaternion.identity);
                
                // ログ更新
                if (detailText != null)
                    detailText.text = $"{currentTime:MM/dd HH:mm}\nDead:{data.dead} Injured:{data.injured}";
                
                // 緯度経度更新
                if (geoText != null) geoText.text = $"LAT: {data.lat:F5}\nLON: {data.lon:F5}";
                
                // Hex ID演出
                if (hexCodeText != null) 
                    hexCodeText.text = $"ID: 0x{UnityEngine.Random.Range(0,999999):X6}";

                accidentCursor++;
            }
            else
            {
                break;
            }
        }
    }
}