using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    const int TEXTURE_SIZE = 1024;

    public float extendsIslandOffset = 1; //Je sais pas encore ce que c'est

    private RenderTexture extendsIslandRenderTexture; 
    private RenderTexture uvIslandRenderTexture;
    private RenderTexture maskRenderTexture;
    private RenderTexture supportTexture;

    private Renderer rend;

    private int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandRenderTexture;
    public RenderTexture getExtend() => extendsIslandRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;


    private void Start()
    {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandRenderTexture.filterMode = FilterMode.Bilinear;

        extendsIslandRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendsIslandRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode = FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendsIslandRenderTexture); 

        //PaintManager.instance.initTextures(this); 
    }


    private void OnDisable()
    {
        maskRenderTexture.Release();
        uvIslandRenderTexture.Release();
        extendsIslandRenderTexture.Release();
        supportTexture.Release(); 
    }


}
