using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening;   

public class StorySequencer : MonoBehaviour
{
    [Header("TDの映像スクリーン")]
    public RawImage predationScreen;

    [Header("タイミング設定")]
    public float waitTime = 10.0f;   
    public float fadeDuration = 3.0f;
    void Start()
    {
        
        if (predationScreen != null)
        {
            predationScreen.color = new Color(1, 1, 1, 0); 
            
          
            Invoke("StartPredation", waitTime);
        }
    }

    void StartPredation()
    {
        Debug.Log("捕食開始...");
        
        
        if (predationScreen != null)
        {
            predationScreen.DOFade(1.0f, fadeDuration).SetEase(Ease.InOutSine);
        }
    }
}
