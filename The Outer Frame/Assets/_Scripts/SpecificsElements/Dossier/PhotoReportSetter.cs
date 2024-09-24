using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoReportSetter : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] Image _image;
    [SerializeField] DinamicMaterialAssigner PhotoTexture;

    public void Set(string n, Sprite i)
    {

        gameObject.SetActive(true);
        _name.text = n;
        _image.sprite = i;
        PhotoTexture.AssignMaterial(i);
    }

}
