using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INewType 
{
    string GetHeadline();

    string GetNewText();

    Sprite GetNewImag();

    bool GetIfIsAEmergency();

    int GetIncreaseAlertLevel();
}
