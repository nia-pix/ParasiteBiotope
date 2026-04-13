using UnityEngine;

public class CityPainter : MonoBehaviour
{
    [Header("ランダムに使うマテリアル")]
    public Material[] randomMaterials; 

    [ContextMenu("bldgを一括適用する")] 
    public void ApplyToBuildingsOnly()
    {
        if (randomMaterials == null || randomMaterials.Length == 0)
        {
            Debug.LogError("マテリアルがセットされていない");
            return;
        }

       
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        int count = 0;

        foreach (Renderer r in renderers)
        {
           
            if (r.gameObject.name.Contains("bldg") == false && r.gameObject.name.Contains("BLDG") == false)
            {
                continue; 
            }

            
            int dice = Random.Range(0, randomMaterials.Length);
            Material selectedMat = randomMaterials[dice];

            Material[] newMats = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = selectedMat;
            }
            r.sharedMaterials = newMats;
            count++;
        }

        Debug.Log("地面は無視して、" + count + "個のビルだけ塗り替え成功");
    }
}
