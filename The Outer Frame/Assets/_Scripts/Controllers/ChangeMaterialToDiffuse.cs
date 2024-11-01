using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialToDiffuse : MonoBehaviour
{
    Material material;
    Shader diffuse;
    private void OnEnable()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer.materials.Length > 1) material = renderer.materials[1];

        diffuse = Shader.Find("Legacy Shaders/Diffuse");
    }

    public void ChangeToDiffuse(Component sender, object obj)
    {
        material.shader = diffuse;
    }
}
