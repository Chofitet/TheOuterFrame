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

        bool isATransparencePhoto = IsMostlyColorSampled(sprite, new Color32(250, 249, 241,255));

        // Crear material estándar
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = sprite.texture;

        if (isTransparent && !isATransparencePhoto)
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
            newMaterial.SetFloat("_Glossiness", 0.25f);
        }
        else if (isATransparencePhoto)
        {
            newMaterial.SetFloat("_Metallic", 0f);
            newMaterial.SetFloat("_Glossiness", 0.4f);
        }
        else
        {
            // Configurar opaco
            newMaterial.SetFloat("_Metallic", 1f);
            newMaterial.SetFloat("_Glossiness", 0.5f);
        }

        

        Material[] materials = new Material[2];
        materials[0] = meshRenderer.material;
        materials[1] = newMaterial;
        meshRenderer.materials = materials;

       // Debug.Log($"Material creado para {sprite.name}, transparente: {isTransparent}");
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

    bool IsMostlyColorSampled(
    Sprite sprite,
    Color targetColor,
    float threshold = 0.6f,
    float tolerance = 10f,
    int sampleGridSize = 16 // 16x16 = 256 samples
)
    {
        if (sprite == null || sprite.texture == null) return false;

        Texture2D tex = sprite.texture;
        Rect rect = sprite.textureRect;

        int matchCount = 0;
        int totalSamples = sampleGridSize * sampleGridSize;

        for (int y = 0; y < sampleGridSize; y++)
        {
            for (int x = 0; x < sampleGridSize; x++)
            {
                // Coordenadas normalizadas dentro del sprite
                float u = (x + 0.5f) / sampleGridSize;
                float v = (y + 0.5f) / sampleGridSize;

                int texX = Mathf.FloorToInt(rect.x + u * rect.width);
                int texY = Mathf.FloorToInt(rect.y + v * rect.height);

                Color32 c = tex.GetPixel(texX, texY);

                if (IsColorSimilar(c, targetColor, tolerance))
                {
                    matchCount++;
                }
            }
        }

        float ratio = (float)matchCount / totalSamples;
        return ratio >= threshold;
    }

    bool IsColorSimilar(Color32 a, Color32 b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) <= tolerance &&
               Mathf.Abs(a.g - b.g) <= tolerance &&
               Mathf.Abs(a.b - b.b) <= tolerance;
    }
}

