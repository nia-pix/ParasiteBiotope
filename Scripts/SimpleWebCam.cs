using UnityEngine;
using UnityEngine.UI;

public class SimpleWebCam : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture; 

    void Start()
    {
        // PCに繋がってるカメラ
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            
            webCamTexture = new WebCamTexture(devices[0].name, 1920, 1080, 30);
            
           
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;

           
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("no camera");
        }
    }
}
