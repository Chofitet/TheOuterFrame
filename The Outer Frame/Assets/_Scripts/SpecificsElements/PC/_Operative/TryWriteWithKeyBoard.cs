using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryWriteWithKeyBoard : MonoBehaviour
{
    [SerializeField] GameEvent OnShakeNotebook;
    ViewStates actualView;
    bool isSearchBarFull;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!Input.GetKeyDown(KeyCode.Escape) && actualView == ViewStates.PCView && !string.IsNullOrEmpty(Input.inputString) && !isSearchBarFull)
            {
                OnShakeNotebook?.Invoke(this, 1.5f);
            }
        }
    }

    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }

    public void SetIsSearchBarFull(Component sender,object obj)
    {
        isSearchBarFull = (bool)obj;
    }
}
