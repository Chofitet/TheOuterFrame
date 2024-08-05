using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] Color endColor;
    Color startColor;
    [SerializeField][Range(0, 10)] float speed;
    Renderer rend;

    bool isblinking;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
        startColor = rend.material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        if (!isblinking) return;
        Color auxColor;
        auxColor = Color.Lerp(startColor, endColor, Mathf.PingPong(speed * Time.time, 1));
        rend.material.SetColor("_EmissionColor", auxColor);
    }

    public void ActiveBlink(Component sender, object obj)
    {
        isblinking = true;
    }

    public void TurnOffLigth(Component sender, object obj)
    {
        isblinking = false;
        Color auxColor;
        auxColor = Color.Lerp(rend.material.GetColor("_EmissionColor"), startColor, speed * Time.time);
        rend.material.SetColor("_EmissionColor", auxColor);
    }

    public void TurnOnLigth(Component sender, object obj)
    {
        if(obj != null)
        {
            Color newColor = (Color)obj;
            endColor = newColor;
        }
        
        isblinking = false;
        Color auxColor;
        auxColor = Color.Lerp(rend.material.GetColor("_EmissionColor"), endColor, speed * Time.time);
        rend.material.SetColor("_EmissionColor", auxColor);
    }

}
