using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    bool isEnable = true;
    bool isInside;

    private void OnMouseOver()
    {
        if (!isEnable) return;

        if (!isInside)
        {
            isInside = true;
            CursorManager.CM.EnterInteractive();
        }
    }

    private void OnMouseExit()
    {
        ExitIfNeeded();
    }

    private void OnMouseDown()
    {
        if (!isEnable) return;

        CursorManager.CM.ClickInteractive();
    }

    public void DisableInteractCursor(Component sender, object obj)
    {
        isEnable = false;
        ExitIfNeeded();
    }

    public void EnableInteractCursor(Component sender, object obj)
    {
        isEnable = true;
    }

    private void OnDisable()
    {
        ExitIfNeeded();
    }

    private void ExitIfNeeded()
    {
        if (!isInside) return;

        isInside = false;
        CursorManager.CM.ExitInteractive();
    }

}
