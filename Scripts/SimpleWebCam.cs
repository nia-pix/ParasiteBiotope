using UnityEngine;
using UnityEngine.UI;

public class SimpleWebCam : MonoBehaviour
{
    public RawImage rawImage; // 映像を映す場所
    private WebCamTexture webCamTexture; // カメラのデータ

    void Start()
    {
        // PCに繋がってるカメラを探す
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // 最初のカメラを使う（ここの数字を変えるとカメラが切り替わるよ）
            webCamTexture = new WebCamTexture(devices[0].name, 1920, 1080, 30);
            
            // RawImageに貼り付ける
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;

            // 録画（再生）スタート！
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("no camera");
        }
    }
}