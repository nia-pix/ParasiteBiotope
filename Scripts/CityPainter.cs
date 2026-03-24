using UnityEngine;

public class CityPainter : MonoBehaviour
{
    [Header("ランダムに使いたいマテリアル")]
    public Material[] randomMaterials; 

    [ContextMenu("★ビルだけ（bldg）を一括適用する★")] 
    public void ApplyToBuildingsOnly()
    {
        if (randomMaterials == null || randomMaterials.Length == 0)
        {
            Debug.LogError("マテリアルがセットされていません！");
            return;
        }

        // 全部のRendererを見つけるけど...
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        int count = 0;

        foreach (Renderer r in renderers)
        {
            // ★ここがガード機能！
            // 名前に "bldg" (小文字) も "BLDG" (大文字) も入ってないなら、無視して次へ！
            if (r.gameObject.name.Contains("bldg") == false && r.gameObject.name.Contains("BLDG") == false)
            {
                continue; // 地面や道路はここでさようなら！
            }

            // --- ここから下はビルだけの処理 ---
            
            // サイコロを振る
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

        Debug.Log("地面は無視して、" + count + "個のビルだけ塗り替えました！成功！");
    }
}