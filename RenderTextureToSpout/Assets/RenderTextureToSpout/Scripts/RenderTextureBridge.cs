using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TextureEvent : UnityEvent<Texture> { }

public class RenderTextureBridge : MonoBehaviour
{

    RenderTexture output;
    public TextureEvent onCreateOutput;

    void OnRenderImage(RenderTexture s, RenderTexture d)
    {
        if (!IsValidOutput(s))
            CreateOutput(s);

        Graphics.CopyTexture(s, output);
        Graphics.Blit(s, d);
    }

    bool IsValidOutput(RenderTexture s)
    {
        return output != null && s.width == output.width && s.height == output.height;
    }

    void CreateOutput(RenderTexture s)
    {
        if (output != null)
            output.Release();
        output = new RenderTexture(s.width, s.height, s.depth, s.format);
        output.Create();
        onCreateOutput.Invoke(output);
    }
}
