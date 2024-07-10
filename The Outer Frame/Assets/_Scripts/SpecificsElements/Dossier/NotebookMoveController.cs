using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookMoveController : MonoBehaviour
{
    Animator anim;
    bool isAllreadyOpen;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenNotebook(Component sender, object obj)
    {
        isAllreadyOpen = true;
        anim.SetTrigger("open");
    }

    public void CloseNotebook(Component sender, object obj)
    {
        if (!isAllreadyOpen) return;
        anim.SetTrigger("close");
        isAllreadyOpen = false;
    }

}
