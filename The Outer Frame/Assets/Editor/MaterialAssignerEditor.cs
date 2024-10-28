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

        if (meshRenderer == null || meshRenderer.materials.Length < 2)
        {
            Debug.LogWarning("No MeshRenderer found on the object or not enough material slots.");
            return;
        }

        // Ruta del material basado en el nombre de la textura
        string materialPath = "Assets/Materials/" + materialAssigner.photo.name + "_Material.mat";

        // Intenta cargar el material existente
        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

        Material newMaterial;
        if (existingMaterial != null)
        {
            // Si el material ya existe, úsalo
            newMaterial = existingMaterial;
            Debug.Log("Material found and assigned.");
        }
        else
        {
            // Si el material no existe, crea uno nuevo
            newMaterial = new Material(Shader.Find("Sprites/Default"));
            newMaterial.mainTexture = materialAssigner.photo.texture;

            // Guarda el nuevo material como un asset
            AssetDatabase.CreateAsset(newMaterial, materialPath);
            AssetDatabase.SaveAssets();

            Debug.Log("New material created and assigned.");
        }

        // Asigna los materiales al MeshRenderer
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.materials[0]; // Conserva el primer material actual
        materials[1] = newMaterial; // Asigna el nuevo o existente material en el segundo slot
        meshRenderer.materials = materials;
    }
}
