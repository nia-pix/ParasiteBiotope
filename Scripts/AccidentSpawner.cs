using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccidentSpawner : MonoBehaviour
{
    [Header("設定")]
    public TextAsset csvFile;
    public GameObject objectToSpawn;
    
    [Header("CSVの列番号")]
    public int latColumnIndex = 68; // BQ列
    public int lonColumnIndex = 69; // BR列

    [Header("地図の基準点")]
    public double originLat = 35.658034; 
    public double originLon = 139.701636;

    [Header("レイキャスト設定")]
    public float rayStartHeight = 300.0f; // レーザーを撃ち始める高さ

    void Start()
    {
        if (csvFile == null || objectToSpawn == null) return;
        SpawnFromCSV();
    }

    void SpawnFromCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        int count = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] data = line.Split(',');
            if (data.Length <= latColumnIndex || data.Length <= lonColumnIndex) continue;

            try
            {
                double lat = double.Parse(data[latColumnIndex]);
                double lon = double.Parse(data[lonColumnIndex]);

                // 1. まず上空の座標を計算
                Vector3 skyPos = ConvertGeoToUnity(lat, lon);
                skyPos.y = rayStartHeight; // 高いところからスタート

                // 2. 下に向かってレーザー発射！ (Raycast)
                RaycastHit hit;
                // "out hit" という箱に、ぶつかった場所の情報が入るよ
                if (Physics.Raycast(skyPos, Vector3.down, out hit, 1000.0f))
                {
                    // ぶつかった場所（hit.point）に生成！
                    Instantiate(objectToSpawn, hit.point, Quaternion.identity, this.transform);
                    count++;
                }
                else
                {
                    // 地面がない場所（地図の外とか）なら、とりあえず地面(Y=0)に置く
                    Vector3 groundPos = skyPos;
                    groundPos.y = 0;
                    Instantiate(objectToSpawn, groundPos, Quaternion.identity, this.transform);
                }
            }
            catch
            {
                // エラーは無視
            }
        }
        Debug.Log("完了！ " + count + " 個のデータを配置しました");
    }

    Vector3 ConvertGeoToUnity(double lat, double lon)
    {
        double latDiff = lat - originLat;
        double lonDiff = lon - originLon;
        double latToMeter = 111000.0; 
        double lonToMeter = 91000.0;

        float x = (float)(lonDiff * lonToMeter);
        float z = (float)(latDiff * latToMeter);
        
        return new Vector3(x, 0, z); // Yはあとで決めるので適当
    }
}