using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    private void OnMouseOver()
    {
        CursorManager.CM.SetInteractCursor();
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        CursorManager.CM.SetDefaultCursor();
    }


}
