using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine.Utility;
using System.Runtime.InteropServices;

public class OverlayAnimation : MonoBehaviour
{
    [SerializeField] GameObject newsTextUI;
    Vector3 textStartingPosition;
    [SerializeField] Transform textOffscreenPositionLeft;
    [SerializeField] Transform textOffscreenPositionRight;
    [SerializeField] GameObject newsTitleUI;
    Vector3 titleStartingPosition;
    [SerializeField] Transform titleOffscreenPositionLeft;
    [SerializeField] Transform titleOffscreenPositionRight;
    [SerializeField] GameObject newsQuipUI;
    Vector3 quipStartingPosition;
    [SerializeField] Transform quipOffscreenPosition;
    [SerializeField] GameObject picsUI;
    Vector3 picsStartingPosition;
    [SerializeField] Transform picsOffscreenPosition;

    [SerializeField] float moveTimes = 1;
    [SerializeField] float pauseTimes = 1;
    Sequence newsInAnim;
    Sequence newsOutAnim;
    Sequence pictureInAnim;
    Sequence pictureOutAnim;

    private void Start()
    {
        textStartingPosition = newsTextUI.transform.position;
        newsTextUI.transform.position = textOffscreenPositionRight.position;

        titleStartingPosition = newsTitleUI.transform.position;
        newsTitleUI.transform.position = titleOffscreenPositionLeft.position;

        quipStartingPosition = newsQuipUI.transform.position;
        newsQuipUI.transform.position = quipOffscreenPosition.position;

        picsStartingPosition = picsUI.transform.position;
        picsUI.transform.position = picsOffscreenPosition.position;

    }

    ////////////////////

    public void NewsIn()
    {
        if (newsInAnim != null && newsInAnim.IsActive()) newsInAnim.Kill();
        newsTitleUI.transform.position = titleOffscreenPositionLeft.transform.position;
        newsTextUI.transform.position = textOffscreenPositionRight.transform.position;

        newsInAnim= DOTween.Sequence();
        newsInAnim.Append(newsTitleUI.transform.DOMove(titleStartingPosition, moveTimes).SetEase(Ease.InOutBack))
            .PrependInterval(pauseTimes)
            .Append(newsTextUI.transform.DOMove(textStartingPosition, moveTimes).SetEase(Ease.InOutBack));
    }
    [ContextMenu("News In")]
    private void NewsInTest()
    {
        NewsIn();
    }

    public void NewsOut()
    {
        if (newsOutAnim != null && newsOutAnim.IsActive()) newsOutAnim.Kill();
        newsTitleUI.transform.position = titleStartingPosition;
        newsTextUI.transform.position = textStartingPosition;

        newsOutAnim = DOTween.Sequence();
        newsOutAnim.Append(newsTitleUI.transform.DOMove(titleOffscreenPositionRight.position, moveTimes).SetEase(Ease.InOutBack))
            .PrependInterval(pauseTimes)
            .Append(newsTextUI.transform.DOMove(textOffscreenPositionLeft.position, moveTimes).SetEase(Ease.InOutBack));

    }
    [ContextMenu("News Out")]
    private void NewsOutTest()
    {
        NewsOut();
    }

    //////////////////////////////

    public void PicsIn()
    {
        if (pictureInAnim != null && pictureInAnim.IsActive()) pictureInAnim.Kill();
        picsUI.transform.position = picsOffscreenPosition.transform.position;

        pictureInAnim = DOTween.Sequence();
        pictureInAnim.Append(picsUI.transform.DOMove(picsStartingPosition, moveTimes).SetEase(Ease.InOutBack));
    }
    [ContextMenu("Pics In")]
    private void PicsInTest()
    {
        PicsIn();
    }

    public void PicsOut()
    {
        if (pictureOutAnim != null && pictureOutAnim.IsActive()) pictureOutAnim.Kill();
        picsUI.transform.position = picsStartingPosition;

        pictureOutAnim = DOTween.Sequence();
        pictureOutAnim.Append(picsUI.transform.DOMove(picsOffscreenPosition.position, moveTimes).SetEase(Ease.InOutBack));
    }
    [ContextMenu("Pics Out")]
    private void PicsOutTest()
    {
        PicsOut();
    }

    /////////////////////////////

    public void QuipIn()
    {
        newsQuipUI.transform.position = quipOffscreenPosition.transform.position;

        newsQuipUI.transform.DOMove(quipStartingPosition, moveTimes).SetEase(Ease.InOutBack);
    }
    [ContextMenu("Quip In")]
    private void QuipInTest()
    {
        QuipIn();
    }

    public void QuipOut()
    {
        newsQuipUI.transform.position = quipStartingPosition;

        newsQuipUI.transform.DOMove(quipOffscreenPosition.position, moveTimes).SetEase(Ease.InOutBack);
    }
    [ContextMenu("Quip Out")]
    private void QuipOutTest()
    {
        QuipOut();
    }
}
