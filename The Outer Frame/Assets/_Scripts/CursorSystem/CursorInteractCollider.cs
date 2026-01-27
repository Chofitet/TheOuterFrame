using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    bool inside = false;

    private void OnMouseOver()
    {
        if (!inside)
        {
            inside = true;
            CursorManager.CM.EnterInteractive();
        }
    }

    private void OnMouseExit()
    {
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
}
