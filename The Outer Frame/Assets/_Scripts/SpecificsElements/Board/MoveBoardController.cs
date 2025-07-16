using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveBoardController : MonoBehaviour
{
    [SerializeField] GameObject board;
    [SerializeField] GameObject initialPos;
    [SerializeField] GameObject initialTutorialPos;
    [SerializeField] GameObject TutorialPos;
    [SerializeField] AnimationCurve MyCurveAnim;
    [SerializeField] GameEvent OnMoveBoardFinish;
    bool isInTutorial;

    private void Start()
    {
        board.SetActive(false);
        //board.transform.position = initialPos.transform.position;
    }

    public void MoveBoard(Component sender, object obj)
    {
        board.transform.DOMove(transform.position, 1.8f).SetEase(Ease.OutSine).OnComplete(() => { OnMoveBoardFinish?.Invoke(this, null); });
        board.SetActive(true);
    }

    public void MoveToTutorialPos(Component sender, object obj)
    {
        board.transform.DOMove(TutorialPos.transform.position, 1.8f).SetEase(Ease.OutSine).OnComplete(() => { OnMoveBoardFinish?.Invoke(this, null); });

        board.SetActive(true);
    }

    public void SetInitialPos(Component sender, object obj)
    {
        isInTutorial = (bool)obj;

        Invoke("SetPos", 0.2f);
    }

    void SetPos()
    {
        if (isInTutorial)
        {
            board.transform.position = initialPos.transform.position;
            board.transform.rotation = initialPos.transform.rotation;
        }
        else
        {
            board.SetActive(true);
        }
    }

    [ContextMenu("MoveBoard")]
    private void MoveBoardTest()
    {
        MoveBoard(null, null);
    }
}
