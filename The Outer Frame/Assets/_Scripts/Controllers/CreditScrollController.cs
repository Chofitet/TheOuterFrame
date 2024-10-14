using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScrollController : MonoBehaviour
{
    [SerializeField] GameObject ScrollPanel;
    [SerializeField] Transform EndPos;
    [SerializeField] float Speed;
    [SerializeField] GameEvent ChangeScene;

    private void Start()
    {
        ScrollCredits();
    }

    void ScrollCredits()
    {
        ScrollPanel.transform.DOMove(EndPos.position, Speed).SetEase(Ease.Linear).OnComplete(()=> { ChangeScene?.Invoke(this, "MainMenu"); });
    }
}
