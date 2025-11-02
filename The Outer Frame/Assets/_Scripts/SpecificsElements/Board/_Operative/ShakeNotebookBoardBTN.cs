using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeNotebookBoardBTN : MonoBehaviour
{
    [SerializeField] GameEvent OnShakeNotebook;
    private ViewStates actualView;

    bool AreWordsToPut;

    bool blockInput;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;

        if(actualView == ViewStates.BoardView)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        else GetComponent<BoxCollider>().enabled = false;
    }
    
    private void OnMouseDown()
    {
        blockInput = false;
        MoreThanAClickCorutine = StartCoroutine(MoreThanAClick());
    }

    private void OnMouseUp()
    {
        StopCoroutine(MoreThanAClickCorutine);
        if (blockInput) return;
        TriggerShakeNotebook();
    }

    Coroutine MoreThanAClickCorutine;
    IEnumerator MoreThanAClick()
    {
        yield return new WaitForSeconds(0.15f);
        blockInput = true;
    }

    public void TriggerShakeNotebook()
    {
        // Al presionar el board

        if (!AreWordsToPut) return;

        OnShakeNotebook?.Invoke(this, 0.2f);
    }

    public void SetAreWordsToPut(Component sender, object obj)
    {
        AreWordsToPut = (bool)obj;
        Debug.Log($"Pendig words to put: {AreWordsToPut}");
    }
}
