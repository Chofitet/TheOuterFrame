using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PhotoLightRuntimeAdjuster : MonoBehaviour
{
    Light photoLight;

    [Header("Distancia → Emission")]
    public float nearDistance = 0.92f;
    public float farDistance = 1.3f;
    public float nearEmission = -0.5f;
    public float farEmission = 0.6f;

    [Header("Suavizado DOTween")]
    public float adjustDuration = 0.2f;

    private Renderer rend;
    private Material photoMaterial;
    private Tweener emissionTween;
    Color originalColor;
    bool isOnBoard = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // Instanciamos solo el material de la foto (índice 1)
        Material[] mats = rend.materials;
        photoMaterial = mats[1];
        mats[1] = photoMaterial;
        rend.materials = mats;
        originalColor = photoMaterial.color;
    }

    private void Start()
    {
        GameObject lightObj = GameObject.Find("Directional Light Board"); 
        if (lightObj != null)
        {
            photoLight = lightObj.GetComponent<Light>();
        }
        else
        {
            Debug.LogWarning("No se encontró la luz con ese nombre.");
        }
    }

    void Update()
    {
        CalculateEmission();
    }

    public void OnPutInBoard(Component sender,object obj)
    {
        if (!gameObject.activeSelf) return;
        isOnBoard = true;
        Debug.Log("onBoard");
    }

    void CalculateEmission()
    {
        if (!isOnBoard) return;
        if (photoLight == null) return;

        // Distancia a la luz
        float dist = Vector3.Distance(transform.position, photoLight.transform.position);
        dist = Mathf.Clamp(dist,nearDistance,farDistance);

        Debug.Log(dist);

        // Interpolación lineal entre valores dados
        float t = Mathf.InverseLerp(nearDistance, farDistance, dist);
        float targetEmission = Mathf.Lerp(nearEmission, farEmission, t);
        targetEmission = Mathf.Clamp(targetEmission, nearEmission, farEmission);

        Color newcolor = originalColor * targetEmission;

        photoMaterial.SetColor("_EmissionColor", newcolor);
    }
}