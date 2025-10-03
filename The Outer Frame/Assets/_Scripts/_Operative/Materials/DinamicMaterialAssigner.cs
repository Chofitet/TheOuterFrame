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

        bool isTransparent = HasTransparency(sprite);

        // Crear material estándar
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = sprite.texture;

        if (isTransparent)
        {
            // Configurar transparente
            newMaterial.SetFloat("_Mode", 3);
            newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            newMaterial.SetInt("_ZWrite", 0);
            newMaterial.DisableKeyword("_ALPHATEST_ON");
            newMaterial.EnableKeyword("_ALPHABLEND_ON");
            newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            newMaterial.renderQueue = 3000;

            newMaterial.SetFloat("_Metallic", 0f);
        }
        else
        {
            // Configurar opaco
            newMaterial.SetFloat("_Metallic", 1f);
        }

        newMaterial.SetFloat("_Glossiness", 0.25f);

        Material[] materials = new Material[2];
        materials[0] = meshRenderer.material;
        materials[1] = newMaterial;
        meshRenderer.materials = materials;

        Debug.Log($"Material creado para {sprite.name}, transparente: {isTransparent}");
    }

    bool HasTransparency(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null) return false;

        Texture2D tex = sprite.texture;
        int w = tex.width;
        int h = tex.height;

        // Puntos de muestreo (esquinas + centro)
        Vector2Int[] points = new Vector2Int[]
        {
        new Vector2Int(0, 0),
        new Vector2Int(w - 1, 0),
        new Vector2Int(0, h - 1),
        new Vector2Int(w - 1, h - 1),
        new Vector2Int(w / 2, h / 2),
        };

        foreach (var p in points)
        {
            Color32 c = tex.GetPixel(p.x, p.y);
            if (c.a < 255) return true;
        }

        return false;
    }
}

