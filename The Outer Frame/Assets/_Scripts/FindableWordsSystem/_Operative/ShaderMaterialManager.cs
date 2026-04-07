using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderMaterialManager : MonoBehaviour
{
    [SerializeField] List<Material> material = new List<Material>();
    [SerializeField] List<Material> BoldMaterial = new List<Material>();

    public Material GetHighLigthMaterial(string originalMaterial)
    {

        foreach (Material mat in material)
        {
           if(mat.name.Contains(originalMaterial))
           {
                return mat;
           }
        }

        return null;
    }

    public Material GetBoldMaterial(string originalMaterial)
    {
        foreach (Material mat in BoldMaterial)
        {
            if (mat.name.Contains(originalMaterial))
            {
                return mat;
            }
        }

        return null;
    }

    public Material GetFirstMat()
    {
        return material[0];
    }
}
