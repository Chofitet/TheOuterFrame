using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorInteractCollider : MonoBehaviour
{
    private void OnMouseEnter()
    {
        CursorManager.CM.SetInteractCursor();
    }

    private void OnMouseExit()
    {
        CursorManager.CM.SetDefaultCursor();
    }


}
