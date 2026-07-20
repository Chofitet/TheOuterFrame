using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    bool inside = false;
    bool isEnable = true;

    private void OnMouseOver()
    {
        if (!isEnable)
        {
            CursorManager.CM.ExitInteractive();
            return;
        }
        if (!inside)
        {
            inside = true;
            CursorManager.CM.EnterInteractive();
        }
    }

    private void OnMouseExit()
    {
        if (!isEnable) return;
        if (inside)
        {
            inside = false;
            CursorManager.CM.ExitInteractive();
        }
    }

    private void OnMouseDown()
    {
        if (inside)
            CursorManager.CM.ClickInteractive();
    }

    public void DisableInteractCursor(Component sender, object obj)
    {
        isEnable = false;
    }
    public void EnableInteractCursor(Component sender, object obj)
    {
        isEnable = true;
    }
}
