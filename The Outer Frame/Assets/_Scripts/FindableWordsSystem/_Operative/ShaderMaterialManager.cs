using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderMaterialManager : MonoBehaviour
{
    [SerializeField] List<Material> material = new List<Material>(); 

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


}
