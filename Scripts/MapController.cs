using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform mapRect; // 地図のRectTransform
    public GameObject blipPrefab; // さっき作った赤い点
    public Transform blipContainer; // 点を入れる親 (BlipContainer)

    [Header("Map Bounds (渋谷エリアの端っこ)")]
    // ※Googleマップとかで渋谷の地図をスクショした範囲の緯度経度を入れる
    public double topLat = 35.665;    // 上端の緯度
    public double bottomLat = 35.655; // 下端の緯度
    public double leftLon = 139.695;  // 左端の経度
    public double rightLon = 139.705; // 右端の経度

    public void SpawnBlip(double lat, double lon)
    {
        // 緯度経度を 0.0〜1.0 の割合に変換
        float y = (float)((lat - bottomLat) / (topLat - bottomLat));
        float x = (float)((lon - leftLon) / (rightLon - leftLon));

        // 範囲外なら表示しない
        if (x < 0 || x > 1 || y < 0 || y > 1) return;

        // 点を生成
        GameObject blip = Instantiate(blipPrefab, blipContainer);
        
        // 地図上の位置に合わせる
        RectTransform rt = blip.GetComponent<RectTransform>();
        
        // アンカーを左下に設定（計算しやすくするため）
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        
        // 座標決定
        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;
        rt.anchoredPosition = new Vector2(x * mapWidth, y * mapHeight);

        // ※演出：数秒で消えるようにしてもいい
        Destroy(blip, 5.0f); // 5秒で消滅
    }
}
