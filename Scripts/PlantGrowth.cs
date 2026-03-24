using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [Header("成長設定")]
    public float growthTime = 1.5f; // 何秒かけて育つか
    public Vector3 targetScale = Vector3.one; // 最終的な大きさ
    public float randomSize = 0.5f; // 大きさのばらつき

    void Start()
    {
        // 1. 最初はサイズを0（見えない状態）にする
        transform.localScale = Vector3.zero;

        // 2. ランダムに最終サイズを少し変える（自然に見せるコツ！）
        float randomFactor = Random.Range(1.0f - randomSize, 1.0f + randomSize);
        targetScale *= randomFactor;

        // 3. 成長開始！
        StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        float timer = 0;

        while (timer < growthTime)
        {
            timer += Time.deltaTime;
            float progress = timer / growthTime;

            // イージング（動きにメリハリをつける計算）
            // 最初は早く、最後はゆっくり（EaseOut）
            float curve = 1f - Mathf.Pow(1f - progress, 3);

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, curve);
            yield return null;
        }

        // 最後にサイズを確定
        transform.localScale = targetScale;
    }
}