using UnityEngine;
using System.Collections.Generic;

public class CityOverlay : MonoBehaviour
{
    [Header("重ねたいマテリアル（ワイヤーフレーム）")]
    public Material overlayMaterial;

    [Header("ズラす量（チラつき防止）")]
    public Vector3 offset = new Vector3(0.02f, 0.02f, 0.02f);

    // ↓ 変なボタン変数は削除して、シンプルにしました！

    // ★魔法の呪文：これを書くと「︙」メニューに追加される！
    [ContextMenu("★ここを押して生成！★")] 
    void GenerateOverlay()
    {
        if (overlayMaterial == null)
        {
            Debug.LogError("マテリアルがセットされてないよ！");
            return;
        }

        List<GameObject> targets = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (!child.name.Contains("_Overlay"))
            {
                targets.Add(child.gameObject);
            }
        }

        int count = 0;
        foreach (GameObject target in targets)
        {
            GameObject clone = Instantiate(target, transform);
            clone.name = target.name + "_Overlay";

            Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                Material[] newMats = new Material[rend.sharedMaterials.Length];
                for (int i = 0; i < newMats.Length; i++)
                {
                    newMats[i] = overlayMaterial;
                }
                rend.sharedMaterials = newMats;
            }

            clone.transform.position += offset;
            count++;
        }

        Debug.Log($"完了！ {count} 個のビルにワイヤーフレームを重ねたよ！✨");
    }
}