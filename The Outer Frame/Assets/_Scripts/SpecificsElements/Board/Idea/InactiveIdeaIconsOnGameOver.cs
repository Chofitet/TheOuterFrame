using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InactiveIdeaIconsOnGameOver : MonoBehaviour
{
    [Header("UI Icons")]
    [SerializeField] GameObject LampOn;
    [SerializeField] GameObject LampOff;
    [SerializeField] GameObject Croos;
    [SerializeField] GameObject Check;

    [Header("Material Icons")]
    [SerializeField] GameObject LampOnMaterial;
    [SerializeField] GameObject LampOnMaterial2;
    [SerializeField] GameObject LampOffMaterial;
    [SerializeField] GameObject LampOffMaterial2;
    [SerializeField] GameObject CroosMaterial;
    [SerializeField] GameObject CheckMaterial;

    public void ReplaceAllUIIconsToMaterialIcons(Component sender, object obj)
    {
        if (LampOn.activeSelf && LampOn.GetComponent<Image>().color.a == 1)
        {
            LampOn.SetActive(false);
            LampOnMaterial.SetActive(true);
            LampOnMaterial2.SetActive(true);
        }
        if (LampOff.activeSelf && LampOn.GetComponent<Image>().color.a == 1)
        {
            LampOff.SetActive(false);
            LampOffMaterial.SetActive(true);
            LampOffMaterial2.SetActive(true);
        }
        if (Croos.activeSelf)
        {
            Croos.SetActive(false);
            CroosMaterial.SetActive(true);
        }
        if(Check.activeSelf)
        {
            Check.SetActive(false);
            CheckMaterial.SetActive(true);
        }
    }
}
