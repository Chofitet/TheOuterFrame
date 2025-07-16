using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryWriteWithKeyBoard : MonoBehaviour
{
    [SerializeField] GameEvent OnShakeNotebook;
    ViewStates actualView;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!Input.GetKeyDown(KeyCode.Escape) && actualView == ViewStates.PCView)
            {
                OnShakeNotebook?.Invoke(this, 1.5f);
            }
        }
    }

    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }
}
