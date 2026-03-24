using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SystemDirector : MonoBehaviour
{
    [Header("--- 🎮 Controllers (リンクさせる) ---")]
    public BioDataController bioMonitor; // 右画面のグラフ管理
    public MapController mapController;     // 左上の地図管理
    public Transform logContent;            // 真ん中のログの親 (Content)
    public GameObject logTextPrefab;        // ログの文字プレハブ（あとで作る）

    [Header("--- 📂 Data Files (CSV) ---")]
    public TextAsset accidentFile;  // 事故データ
    public TextAsset powerFile;     // 電力データ
    public TextAsset pollutionFile; // 汚染データ
    // 自転車データがあればここに追加

    [Header("--- ⚙️ Settings ---")]
    public GameObject[] plantPrefabs; // 3D植物
    public float timeSpeed = 1000.0f; // 時間の進む速さ
    public DateTime currentTime;      // 現在の時間（Unity上で確認用）

    // 内部データ
    private List<AccidentData> accidents = new List<AccidentData>();
    private Dictionary<string, float> powerData = new Dictionary<string, float>();
    private Dictionary<string, float> pollutionData = new Dictionary<string, float>();
    private int currentAccidentIndex = 0;

    // データ構造
    class AccidentData : IComparable<AccidentData>
    {
        public DateTime time;
        public Vector3 position;
        public double lat, lon;
        public int dead, injured;
        public int CompareTo(AccidentData other) { return time.CompareTo(other.time); }
    }

    void Start()
    {
        // 1. 全データを読み込む（ロード）
        LoadAccidents();
        LoadSideData(powerFile, powerData);
        LoadSideData(pollutionFile, pollutionData);

        // 開始時間をセット（事故データの最初に合わせる）
        if (accidents.Count > 0) currentTime = accidents[0].time;
        
        Log("SYSTEM ONLINE.");
        Log("LOADING CITY DATA... COMPLETE.");
    }

    void Update()
    {
        // 時間を進める
        currentTime = currentTime.AddMinutes(Time.deltaTime * timeSpeed);

        // --- A. 事故のチェック（植物生成 & 地図） ---
        while (currentAccidentIndex < accidents.Count)
        {
            AccidentData data = accidents[currentAccidentIndex];
            if (data.time <= currentTime)
            {
                SpawnPlant(data); // 植物生やす
                
                // ★地図を光らせる！
                if (mapController != null) mapController.SpawnBlip(data.lat, data.lon);
                
                // ★ログを出す！
                string type = (data.dead > 0) ? "<color=red>FATAL</color>" : "<color=yellow>INJURY</color>";
                Log($"[{currentTime:HH:mm}] {type} ACCIDENT DETECTED at {data.lat:F4}, {data.lon:F4}");

                currentAccidentIndex++;
            }
            else break;
        }

        // --- B. 環境データの更新（モニター2） ---
        // 今の時間に近いデータを探す（キーは "yyyy/M/d H:00" 形式と仮定）
        string key = currentTime.ToString("yyyy/M/d H:00"); 

        float currentPower = 0;
        float currentPollution = 0;

        if (powerData.ContainsKey(key)) currentPower = powerData[key];
        if (pollutionData.ContainsKey(key)) currentPollution = pollutionData[key];

        // ★右画面のUIを更新！
        if (bioMonitor != null)
        {
            // 自転車データは仮で「事故数」を入れている（データがあれば差し替えて！）
            bioMonitor.UpdateMonitor(currentPower, currentPollution, currentAccidentIndex);
        }
    }

    // --- 🛠️ 内部機能 (触らなくてOK) ---

    void SpawnPlant(AccidentData data)
    {
        if (plantPrefabs == null || plantPrefabs.Length == 0) return;
        GameObject prefab = plantPrefabs[UnityEngine.Random.Range(0, plantPrefabs.Length)];
        
        // ※座標変換は簡易版（前のTimeLoopSpawnerと同じロジックを使ってね）
        Vector3 pos = ConvertGeoToUnity(data.lat, data.lon);
        
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        
        // 巨大化演出
        float scale = (data.dead > 0) ? 3.0f : 1.0f;
        obj.transform.localScale *= scale;
    }

    void LoadAccidents()
    {
        if (accidentFile == null) return;
        string[] lines = accidentFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 1行目はヘッダー飛ばす
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 10) continue;
            try {
                // 列番号はCSVに合わせて調整してね！(ここでは仮置き)
                int y = int.Parse(cols[10]); 
                int m = int.Parse(cols[11]);
                int d = int.Parse(cols[12]);
                int h = int.Parse(cols[13]);
                int min = int.Parse(cols[14]);
                double lat = double.Parse(cols[68]); // 緯度
                double lon = double.Parse(cols[69]); // 経度
                
                accidents.Add(new AccidentData {
                    time = new DateTime(y, m, d, h, min, 0),
                    lat = lat, lon = lon,
                    dead = 0, injured = 0 // 必要ならCSVから読む
                });
            } catch {}
        }
        accidents.Sort();
    }

    // 電力や汚染データを "2023/1/1 10:00" -> 3500 みたいな辞書にする
    void LoadSideData(TextAsset csv, Dictionary<string, float> dict)
    {
        if (csv == null) return;
        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(','); // 区切り文字注意！スペース区切りなら ' '
            if (cols.Length < 2) continue;
            try {
                // 日付と時間が別列なら結合する処理が必要
                // ここではシンプルな "Date Time, Value" 形式を想定
                // ミメちゃんのExcelデータに合わせて調整するよ！
                string timeKey = cols[0] + " " + cols[1]; // 日付 + 時間
                float val = float.Parse(cols[cols.Length-1]); // 一番後ろの値
                if (!dict.ContainsKey(timeKey)) dict.Add(timeKey, val);
            } catch {}
        }
    }

    void Log(string message)
    {
        if (logContent == null || logTextPrefab == null) return;
        GameObject logObj = Instantiate(logTextPrefab, logContent);
        logObj.GetComponent<TextMeshProUGUI>().text = message;
        
        // ログが溜まりすぎないように古いのを消す
        if (logContent.childCount > 20) Destroy(logContent.GetChild(0).gameObject);
    }

    Vector3 ConvertGeoToUnity(double lat, double lon)
    {
        // 前のスクリプトと同じ座標変換ロジックをここにコピペして！
        // (簡易版)
        double originLat = 35.658034;
        double originLon = 139.701636;
        float x = (float)((lon - originLon) * 91000.0);
        float z = (float)((lat - originLat) * 111000.0);
        return new Vector3(x, 0, z);
    }
}
