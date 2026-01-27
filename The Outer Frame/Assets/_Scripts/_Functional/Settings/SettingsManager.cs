using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] DropdownField qualityDropdown;
    int quality = 5;

    private void Start()
    {
        QualitySettings.SetQualityLevel(quality, true);
    }

    public void ShowMenu()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ConfirmBtn()
    {
        ConfirmQuality();
    }

    void SetQuality(int qualityIndex)
    {
        Debug.Log($"Set quality: {qualityIndex}");
        quality = qualityIndex;
        
    }

    void ConfirmQuality()
    {
        QualitySettings.SetQualityLevel(quality, true);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
