using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupView : MonoBehaviour
{
    [SerializeField] GameObject ImageConteiner;

    public void init()
    {
        //AppearAnim();
    }
    void AppearAnim()
    {
        if (!ImageConteiner.GetComponent<BlinkMaterialEffect>()) return;
        ImageConteiner.GetComponent<BlinkMaterialEffect>().ActiveBlink(null, null);
    }

    public void CheckView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.PCView)
        {
            if (!ImageConteiner.GetComponent<BlinkMaterialEffect>()) return;
            ImageConteiner.GetComponent<BlinkMaterialEffect>().TurnOffLight(null, null);
        }
    }
}
