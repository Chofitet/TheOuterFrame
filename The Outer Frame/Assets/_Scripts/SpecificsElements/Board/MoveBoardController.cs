using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveBoardController : MonoBehaviour
{
    [SerializeField] GameObject board;
    [SerializeField] GameObject initialPos;
    [SerializeField] AnimationCurve MyCurveAnim;

    private void Start()
    {
        board.SetActive(false);
        board.transform.position = initialPos.transform.position;
    }

    public void MoveBoard(Component sender, object obj)
    {
        board.transform.DOMove(transform.position, 3).SetEase(Ease.OutSine);
        board.SetActive(true);
    }
}
