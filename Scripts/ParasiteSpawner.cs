using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParasiteSpawner : MonoBehaviour
{
    [Header("設定")]
    public GameObject parasitePrefab;
    public int spawnCount = 50;
    public Vector2 cityRange = new Vector2(500,500);
    public LayerMask buildingLayer;
   public float spawnHeight = 200f;  // 空の高さ

    void Start()
    {
        SpawnParasites();
    }

    void SpawnParasites()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 1. 空の上のランダムな位置を決める
            float randomX = Random.Range(-cityRange.x, cityRange.x);
            float randomZ = Random.Range(-cityRange.y, cityRange.y);
            Vector3 skyPos = new Vector3(randomX, spawnHeight, randomZ);

            // 2. 空から真下にレイザー（Ray）を撃つ
            RaycastHit hit;
            if (Physics.Raycast(skyPos, Vector3.down, out hit, 1000f, buildingLayer))
            {
                // 3. ビルに当たったら、その場所にSphere（種）を生成！
                // hit.point (当たった場所) + hit.normal (壁の向き) * 0.1f (ちょっと浮かす)
                GameObject parasite = Instantiate(parasitePrefab, hit.point + hit.normal * 0.1f, Quaternion.identity);
                
                // 4. 壁の向きに合わせて回転させておく（重要！）
                parasite.transform.up = hit.normal;
            }
        }
    }

    // 範囲を視覚化（デバッグ用）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0), new Vector3(cityRange.x * 2, 0, cityRange.y * 2));
    }
}

