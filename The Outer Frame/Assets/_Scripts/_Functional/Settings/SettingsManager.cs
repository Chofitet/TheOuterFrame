using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    void Start()
    {
        SetResolutionDrowDown();

        IsFullScreen = Screen.fullScreen;
        FullScreenToggle.isOn = IsFullScreen;
    }

    public void ShowMenu(Component sender, object obj)
    {
        VisivilityOfMenu(true);
    }

    public void VisivilityOfMenu(bool x)
    {
        Menu.SetActive(x);
    }

    public void ApplyBtn()
    {
        ConfirmResolution();
        ConfirmQuality();
        VisivilityOfMenu(false);
    }

    public void BackBTN()
    {
        VisivilityOfMenu(false);
    }

    ///------------------------ Quality ------------------------------------///
    [SerializeField] bool EnableQualityOp = true;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] GameObject Menu;
    int quality = 5;

    public void SetQuality()
    {
        if (!EnableQualityOp) return;
        quality = qualityDropdown.value;
        Debug.Log($"Set quality: {quality}");

    }

    void ConfirmQuality()
    {
        if (!EnableQualityOp) return;
        QualitySettings.SetQualityLevel(quality, true);
    }


    ///------------------------ Resolution ------------------------------------///

    [SerializeField] TMP_Dropdown ResDropDown;
    [SerializeField] Toggle FullScreenToggle;
    Resolution[] AllResolutions;
    bool IsFullScreen;
    int SelectedResolution;
    List<Resolution> SelectedResolutionList = new List<Resolution>();

    void SetResolutionDrowDown()
    {
        IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;

        int currectResIndex = 0;
        int index = 0;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();

            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }

            if(res.width == Screen.currentResolution.width  && res.height == Screen.currentResolution.height)
            {
                currectResIndex = index;
            }

            index++;
        }
        ResDropDown.AddOptions(resolutionStringList);
        ResDropDown.value = currectResIndex;
        ResDropDown.RefreshShownValue();
    }

    public void SetResolution()
    {
        SelectedResolution = ResDropDown.value;
        ResDropDown.RefreshShownValue();

    }

    public void SetFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;

    }

    void ConfirmResolution()
    {
        Screen.SetResolution(
            SelectedResolutionList[SelectedResolution].width,
            SelectedResolutionList[SelectedResolution].height,
            IsFullScreen
        );
    }
}
