using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    bool isStillOverInteractive;
    private void OnMouseOver()
    {
        CursorManager.CM.SetInteractCursor();
        isStillOverInteractive = true;
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        CursorManager.CM.SetDefaultCursor();
        isStillOverInteractive = false;
    }

    void OnMouseDown()
    {
        if(isStillOverInteractive) CursorManager.CM.SetInteractCursor();
    }
}
