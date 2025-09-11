using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScrollController : MonoBehaviour
{
    [SerializeField] GameObject ScrollPanel;
    [SerializeField] Transform EndPos;
    [SerializeField] float Speed;
    [SerializeField] float timeToStart;
    [SerializeField] GameEvent ChangeScene;

    private void Start()
    {
        StartCoroutine(delay());
    }

    void ScrollCredits()
    {
        ScrollPanel.transform.DOMove(EndPos.position, Speed).SetEase(Ease.Linear).OnComplete(()=> { ChangeScene?.Invoke(this, "MainMenu"); });
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(timeToStart);
        ScrollCredits();
    }
}
