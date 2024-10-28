using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicMaterialAssigner : MonoBehaviour
{
    public void AssignMaterial(Sprite sprite)
    {
        // Obtener el MeshRenderer del objeto
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!sprite) return;
        if (meshRenderer.materials.Length < 2) return;

        if (meshRenderer != null)
        {
            // Crea un nuevo material en tiempo de ejecución
            Material newMaterial = new Material(Shader.Find("Sprites/Diffuse")); // Usamos un shader para sprites

            // Asigna la textura del sprite al nuevo material
            newMaterial.mainTexture = sprite.texture;

            Material[] materials = new Material[2];
            materials[0] = meshRenderer.material; 
            materials[1] = newMaterial; 
            meshRenderer.materials = materials;

            Debug.Log("Material created and assigned at runtime." + sprite.name);
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on the object.");
        }
    }
}
