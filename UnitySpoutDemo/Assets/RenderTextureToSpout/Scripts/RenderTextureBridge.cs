using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TextureEvent : UnityEvent<Texture> { }

public class RenderTextureBridge : MonoBehaviour
{

    RenderTexture texture;
    public TextureEvent onCreateTexture;

    void Start()
    {
        var cam = GetComponent<Camera>();
        texture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
        texture.Create();
        onCreateTexture.Invoke(texture);
    }

    void OnRenderImage(RenderTexture s, RenderTexture d)
    {
        Graphics.CopyTexture(s, texture);
        Graphics.Blit(s, d);
    }
}
