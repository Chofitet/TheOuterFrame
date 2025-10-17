using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Scanner : MonoBehaviour {

	public float area;
//	public bool horizontal;
	private Material material_a, material_b;

	// Creates a private material used to the effect
	void Awake ()
	{
		material_a = new Material( Shader.Find("Hidden/Shift") );
		material_b = new Material( Shader.Find("Hidden/Shift") );
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
        material_a.SetFloat("_ValueY", area);
        material_b.SetFloat("_ValueY", -area);

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

        Graphics.Blit(source, temp, material_a);

        Graphics.Blit(temp, destination, material_b);

        RenderTexture.ReleaseTemporary(temp);
    }
}
