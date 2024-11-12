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

        // Verificamos que haya al menos dos materiales en el renderer
        if (renderer.materials.Length > 1)
        {
            material = renderer.materials[1];
        }
        else
        {
            Debug.LogWarning("No se encontró un segundo material en el renderer.");
        }

        // Buscamos el shader y verificamos si fue encontrado
        diffuse = Shader.Find("Legacy Shaders/Diffuse");
        if (diffuse == null)
        {
            Debug.LogError("No se encontró el shader 'Legacy Shaders/Diffuse'.");
        }
    }

    public void ChangeToDiffuse(Component sender, object obj)
    {
        // Verificamos si el material y el shader existen antes de aplicar el cambio
        if (material != null && diffuse != null)
        {
            material.shader = diffuse;
        }
        else
        {
            Debug.LogWarning("No se pudo cambiar el shader porque el material o el shader no están inicializados.");
        }
    }
}
