using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [Header("成長設定")]
    public float growthTime = 1.5f; 
    public Vector3 targetScale = Vector3.one; 
    public float randomSize = 0.5f; 

    void Start()
    {
        
        transform.localScale = Vector3.zero;

        
        float randomFactor = Random.Range(1.0f - randomSize, 1.0f + randomSize);
        targetScale *= randomFactor;

        //  成長開始
        StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        float timer = 0;

        while (timer < growthTime)
        {
            timer += Time.deltaTime;
            float progress = timer / growthTime;

           
            float curve = 1f - Mathf.Pow(1f - progress, 3);

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, curve);
            yield return null;
        }

        
        transform.localScale = targetScale;
    }
}
