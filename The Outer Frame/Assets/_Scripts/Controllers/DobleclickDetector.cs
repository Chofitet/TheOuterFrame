using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobleclickDetector : MonoBehaviour
{
    int num;
    [SerializeField] GameEvent OnDobleClick;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            num++;
            CheckNumOfClicks();
        }
    }

    void CheckNumOfClicks()
    {
        if (num >= 2)
        {
            OnDobleClick?.Invoke(this, null);
        }
    }
}
