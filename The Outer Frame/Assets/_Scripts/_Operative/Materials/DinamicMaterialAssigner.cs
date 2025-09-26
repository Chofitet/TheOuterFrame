using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicMaterialAssigner : MonoBehaviour
{
    public void AssignMaterial(Sprite sprite)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!sprite) return;
        if (meshRenderer == null || meshRenderer.materials.Length < 2) return;

        // Crear material estándar
        Material newMaterial = new Material(Shader.Find("Standard"));

        // Asignar la textura del sprite
        newMaterial.mainTexture = sprite.texture;

        // Ajustar parámetros de Standard Shader
        newMaterial.SetFloat("_Metallic", 1f);
        newMaterial.SetFloat("_Glossiness", 0.25f);

        // Asignar materiales al MeshRenderer
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.material;
        materials[1] = newMaterial;
        meshRenderer.materials = materials;

        Debug.Log("Material Standard creado y asignado: " + sprite.name);
    }
}
