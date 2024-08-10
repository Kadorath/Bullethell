using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GreyscaleEffect : MonoBehaviour {

    public float intensity;
    private Material material;

    // Creates a private material used to the effect
    void Awake ()
    {
        material = new Material(Shader.Find("Hidden/BWDiffuse"));
        Debug.Log(material);
    }
    
    // Postprocess the image
    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        Debug.Log("OnRenderImage called");
        if (intensity == 0)
        {
            Graphics.Blit (source, destination);
            return;
        }

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit (source, destination, material);
    }
}