using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkMaterialEffect : MonoBehaviour
{
    Color OriginalColor;
    [SerializeField] [Range(0, 10)] float speed = 1; // Velocidad del parpadeo
    Renderer rend;
    bool isblinking;
    float startEmissionIntensity = 2.416924F; // Intensidad inicial del Emission
    float maxEmissionIntensity; // Máxima intensidad de Emission
    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");

        // Obtiene el valor de intensidad actual del Emission (usado en el max)
        TurnOffLight(null, null);
    }

    private void Update()
    {
        if (!isblinking) return;

        // Calcula el nuevo valor de la intensidad usando PingPong entre 0 y la intensidad máxima
        float intensity = Mathf.PingPong(speed * Time.time, maxEmissionIntensity);

        // Multiplica el color de emisión actual por la intensidad calculada
        rend.material.SetColor("_EmissionColor", rend.material.GetColor("_EmissionColor") * intensity);
    }

    public void ActiveBlink(Component sender, object obj)
    {
        isblinking = true;
    }

    public void TurnOffLight(Component sender, object obj)
    {
        isblinking = false;

        // Restaura la intensidad a la inicial
        rend.material.SetColor("_EmissionColor", rend.material.GetColor("_EmissionColor") * 0);
    }

    public void TurnOnLight(Component sender, object obj)
    {
        isblinking = false;

       maxEmissionIntensity = startEmissionIntensity; 

        // Aplica la intensidad máxima al encender la luz
        rend.material.SetColor("_EmissionColor", rend.material.GetColor("_EmissionColor") * maxEmissionIntensity);
    }
}
