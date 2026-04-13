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
   public float spawnHeight = 200f;  

    void Start()
    {
        SpawnParasites();
    }

    void SpawnParasites()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            
            float randomX = Random.Range(-cityRange.x, cityRange.x);
            float randomZ = Random.Range(-cityRange.y, cityRange.y);
            Vector3 skyPos = new Vector3(randomX, spawnHeight, randomZ);

            
            RaycastHit hit;
            if (Physics.Raycast(skyPos, Vector3.down, out hit, 1000f, buildingLayer))
            {
                
                GameObject parasite = Instantiate(parasitePrefab, hit.point + hit.normal * 0.1f, Quaternion.identity);
                
               
                parasite.transform.up = hit.normal;
            }
        }
    }

  
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, spawnHeight, 0), new Vector3(cityRange.x * 2, 0, cityRange.y * 2));
    }
}

