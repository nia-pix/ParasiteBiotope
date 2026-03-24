using UnityEngine;
using UnityEngine.UI; // UIをいじるために必要
using DG.Tweening;    // フェードインアニメーションに必要

public class StorySequencer : MonoBehaviour
{
    [Header("TDの映像スクリーン")]
    public RawImage predationScreen; // さっき作ったPredationScreenを入れる

    [Header("タイミング設定")]
    public float waitTime = 10.0f;   // 何秒後に切り替えるか
    public float fadeDuration = 3.0f; // 何秒かけてじわ〜っと出すか

    void Start()
    {
        // 1. 最初はスクリーンを「透明」にして見えなくしておく
        // (SetActive(false)だと通信が切れちゃうから、透明度を0にするのがコツ！)
        if (predationScreen != null)
        {
            predationScreen.color = new Color(1, 1, 1, 0); 
            
            // 2. タイマーセット！ waitTime秒後に "StartPredation" を実行
            Invoke("StartPredation", waitTime);
        }
    }

    void StartPredation()
    {
        Debug.Log("捕食開始...");
        
        // 3. 透明度（Alpha）を 0 から 1 にして、映像を浮かび上がらせる
        if (predationScreen != null)
        {
            predationScreen.DOFade(1.0f, fadeDuration).SetEase(Ease.InOutSine);
        }
    }
}