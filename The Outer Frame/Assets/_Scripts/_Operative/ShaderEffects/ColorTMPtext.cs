using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorTMPtext : MonoBehaviour
{
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    public void ChangeColor1()
    {
        GetComponent<TMP_Text>().color = color1;
    }

    public void ChangeColor2()
    {
        GetComponent<TMP_Text>().color = color2;
    }
}
