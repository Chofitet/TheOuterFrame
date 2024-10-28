using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialAssigner))]
public class MaterialAssignerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Referencia al script de MaterialAssigner
        MaterialAssigner materialAssigner = (MaterialAssigner)target;

        // Muestra el campo para asignar el sprite
        materialAssigner.photo = (Sprite)EditorGUILayout.ObjectField("Sprite", materialAssigner.photo, typeof(Sprite), false);

        if (GUILayout.Button("Assign Material"))
        {
            // Verifica que haya un sprite asignado
            if (materialAssigner.photo != null)
            {
                AssignMaterial(materialAssigner);
            }
            else
            {
                Debug.LogWarning("Please assign a sprite first.");
            }
        }
    }

    private void AssignMaterial(MaterialAssigner materialAssigner)
    {
        // Obtener el MeshRenderer del objeto
        MeshRenderer meshRenderer = materialAssigner.GetComponent<MeshRenderer>();

        if (meshRenderer.materials.Length < 2) return;

            if (meshRenderer != null)
        {
            // Crea un nuevo material
            Material newMaterial = new Material(Shader.Find("Sprites/Diffuse")); // Usa el shader adecuado para sprites

            // Asigna la textura del sprite al material
            newMaterial.mainTexture = materialAssigner.photo.texture;

            
            Material[] materials = new Material[2];
            materials[0] = meshRenderer.material;  // Conserva el primer material actual
            materials[1] = newMaterial;  // Asigna el nuevo material en el segundo slot
            meshRenderer.materials = materials;
           
            // Guarda el nuevo material como un asset
            string materialPath = "Assets/Materials/" + materialAssigner.photo.name + "_Material.mat";
            AssetDatabase.CreateAsset(newMaterial, materialPath);
            AssetDatabase.SaveAssets();

            Debug.Log("Material successfully created and assigned.");
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on the object.");
        }
    }
}
