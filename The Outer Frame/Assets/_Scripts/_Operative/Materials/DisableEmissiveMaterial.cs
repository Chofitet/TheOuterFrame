using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEmissiveMaterial : MonoBehaviour
{
    private Material material;

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
            Debug.LogWarning("No se encontr� un segundo material en el renderer.");
        }
    }

    public void DisableEmission(Component sender, object obj)
    {
        if (material != null)
        {
            // Seteamos el color emisivo en negro
            material.SetColor("_EmissionColor", Color.black);

            // Deshabilitamos el keyword de emission
            material.DisableKeyword("_EMISSION");

            Debug.Log("Emisi�n desactivada en el material.");
        }
        else
        {
            Debug.LogWarning("No se pudo desactivar la emisi�n porque el material no est� inicializado.");
        }
    }
}
