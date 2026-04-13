using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SystemDirector : MonoBehaviour
{
    [Header("Controllers")]
    public BioDataController bioMonitor; 
    public MapController mapController;     
    public Transform logContent;          
    public GameObject logTextPrefab;        

    [Header("Data Files(CSV)")]
    public TextAsset accidentFile;  // 事故データ
    public TextAsset powerFile;     // 電力データ
    public TextAsset pollutionFile; // 汚染データ
    

    [Header("設定")]
    public GameObject[] plantPrefabs; // 植物
    public float timeSpeed = 1000.0f; // 時間の進む速さ
    public DateTime currentTime;      // 現在の時間
    // 内部
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
        
        LoadAccidents();
        LoadSideData(powerFile, powerData);
        LoadSideData(pollutionFile, pollutionData);

      
        if (accidents.Count > 0) currentTime = accidents[0].time;
        
        Log("SYSTEM ONLINE.");
        Log("LOADING CITY DATA... COMPLETE.");
    }

    void Update()
    {
       
        currentTime = currentTime.AddMinutes(Time.deltaTime * timeSpeed);

        while (currentAccidentIndex < accidents.Count)
        {
            AccidentData data = accidents[currentAccidentIndex];
            if (data.time <= currentTime)
            {
                SpawnPlant(data); // 植物生やす
                
                
                if (mapController != null) mapController.SpawnBlip(data.lat, data.lon);
                
                // ログ
                string type = (data.dead > 0) ? "<color=red>FATAL</color>" : "<color=yellow>INJURY</color>";
                Log($"[{currentTime:HH:mm}] {type} ACCIDENT DETECTED at {data.lat:F4}, {data.lon:F4}");

                currentAccidentIndex++;
            }
            else break;
        }

        
        string key = currentTime.ToString("yyyy/M/d H:00"); 

        float currentPower = 0;
        float currentPollution = 0;

        if (powerData.ContainsKey(key)) currentPower = powerData[key];
        if (pollutionData.ContainsKey(key)) currentPollution = pollutionData[key];

        
        if (bioMonitor != null)
        {
            
            bioMonitor.UpdateMonitor(currentPower, currentPollution, currentAccidentIndex);
        }
    }

    
    void SpawnPlant(AccidentData data)
    {
        if (plantPrefabs == null || plantPrefabs.Length == 0) return;
        GameObject prefab = plantPrefabs[UnityEngine.Random.Range(0, plantPrefabs.Length)];
        
      
        Vector3 pos = ConvertGeoToUnity(data.lat, data.lon);
        
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        

        float scale = (data.dead > 0) ? 3.0f : 1.0f;
        obj.transform.localScale *= scale;
    }

    void LoadAccidents()
    {
        if (accidentFile == null) return;
        string[] lines = accidentFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) 
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 10) continue;
            try {
                
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
                    dead = 0, injured = 0 // CSVから読む
                });
            } catch {}
        }
        accidents.Sort();
    }

    
    void LoadSideData(TextAsset csv, Dictionary<string, float> dict)
    {
        if (csv == null) return;
        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(','); 
            if (cols.Length < 2) continue;
            try {
                
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
        
        // 古いの消す
        if (logContent.childCount > 20) Destroy(logContent.GetChild(0).gameObject);
    }

    Vector3 ConvertGeoToUnity(double lat, double lon)
    {
        
        double originLat = 35.658034;
        double originLon = 139.701636;
        float x = (float)((lon - originLon) * 91000.0);
        float z = (float)((lat - originLat) * 111000.0);
        return new Vector3(x, 0, z);
    }
}
