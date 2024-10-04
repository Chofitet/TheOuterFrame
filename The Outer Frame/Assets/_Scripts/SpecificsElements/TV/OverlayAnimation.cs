using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine.Utility;
using System.Runtime.InteropServices;
using TMPro;

public class OverlayAnimation : MonoBehaviour
{
    [HideInInspector] [SerializeField] GameObject newsTextUI;
    Vector3 textStartingPosition;
    [HideInInspector] [SerializeField] Transform textOffscreenPositionLeft;
    [HideInInspector] [SerializeField] Transform textOffscreenPositionRight;
    [HideInInspector] [SerializeField] GameObject newsTitleUI;
    Vector3 titleStartingPosition;
    [HideInInspector] [SerializeField] Transform titleOffscreenPositionLeft;
    [HideInInspector] [SerializeField] Transform titleOffscreenPositionRight;
    [HideInInspector] [SerializeField] GameObject newsQuipUI;
    Vector3 quipStartingPosition;
    [HideInInspector] [SerializeField] Transform quipOffscreenPosition;
    [HideInInspector] [SerializeField] GameObject picsUI;
    Vector3 picsStartingPosition;
   [HideInInspector] [SerializeField] Transform picsOffscreenPosition;

    TMP_Text NewContentTMPtxt;
    TMP_Text HeadlineTMPtxt;
    TMP_Text QuipTMPtxt;
    
    [SerializeField] float moveTimes = 1;
    float acceleratedFactor = 1;
    [SerializeField] float pauseTimes = 1;
    [SerializeField] float newsChangeTime = 1;
    Sequence newsInAnim;
    Sequence newsOutAnim;
    Sequence pictureInAnim;
    Sequence pictureOutAnim;
    Sequence quipAnim;

    private void Start()
    {
        textStartingPosition = newsTextUI.transform.position;
        //newsTextUI.transform.position = textOffscreenPositionRight.position;

        titleStartingPosition = newsTitleUI.transform.position;
        //newsTitleUI.transform.position = titleOffscreenPositionLeft.position;

        quipStartingPosition = newsQuipUI.transform.position;
        //newsQuipUI.transform.position = quipOffscreenPosition.position;

        picsStartingPosition = picsUI.transform.position;
        //picsUI.transform.position = picsOffscreenPosition.position;

        NewContentTMPtxt = newsTextUI.transform.GetChild(0).GetComponent<TMP_Text>();
        HeadlineTMPtxt = newsTitleUI.transform.GetChild(0).GetComponent<TMP_Text>();
        QuipTMPtxt = newsQuipUI.transform.GetChild(0).GetComponent<TMP_Text>();

    }

    ////////////////////

    public void NewsIn()
    {
        if (newsInAnim != null && newsInAnim.IsActive()) newsInAnim.Kill();
        newsTitleUI.transform.position = titleOffscreenPositionLeft.transform.position;
        newsTextUI.transform.position = textOffscreenPositionRight.transform.position;
        NewContentTMPtxt.color = new Color(NewContentTMPtxt.color.r, NewContentTMPtxt.color.g, NewContentTMPtxt.color.b, 0);
        HeadlineTMPtxt.color = new Color(HeadlineTMPtxt.color.r, HeadlineTMPtxt.color.g, HeadlineTMPtxt.color.b, 0);

        newsInAnim = DOTween.Sequence();
        newsInAnim.PrependInterval(pauseTimes)
            .Append(newsTitleUI.transform.DOMove(titleStartingPosition, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack))
            .Join(newsTextUI.transform.DOMove(textStartingPosition, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack))
            .AppendCallback(() => fadeUI(NewContentTMPtxt,1))
            .JoinCallback(() => fadeUI(HeadlineTMPtxt, 1))
            .JoinCallback(() => fadeUI(QuipTMPtxt, 1));
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
        newsOutAnim.PrependInterval(pauseTimes)
            .AppendCallback(() => fadeUI(NewContentTMPtxt,0))
            .JoinCallback(() => fadeUI(HeadlineTMPtxt, 0))
            .JoinCallback(() => fadeUI(QuipTMPtxt, 0))
            .PrependInterval(pauseTimes)
            .Append(newsTitleUI.transform.DOMove(titleOffscreenPositionRight.position, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack))
            .Join(newsTextUI.transform.DOMove(textOffscreenPositionLeft.position, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack));

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
        pictureInAnim.PrependInterval(pauseTimes)
                       .Append(picsUI.transform.DOMove(picsStartingPosition, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack));
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
        pictureOutAnim.PrependInterval(pauseTimes)
            .Append(picsUI.transform.DOMove(picsOffscreenPosition.position, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack));
    }
    [ContextMenu("Pics Out")]
    private void PicsOutTest()
    {
        PicsOut();
    }

    /////////////////////////////

    public void QuipIn()
    {
        if (quipAnim != null && quipAnim.IsActive()) quipAnim.Kill();
        newsQuipUI.transform.position = quipOffscreenPosition.transform.position;
        QuipTMPtxt.color = new Color(QuipTMPtxt.color.r, QuipTMPtxt.color.g, QuipTMPtxt.color.b, 0);

        quipAnim = DOTween.Sequence();
        quipAnim.PrependInterval(pauseTimes)
            .Append(newsQuipUI.transform.DOMove(quipStartingPosition, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack));
    }
    [ContextMenu("Quip In")]
    private void QuipInTest()
    {
        QuipIn();
    }

    public void QuipOut()
    {
        if (quipAnim != null && quipAnim.IsActive()) quipAnim.Kill();
        newsQuipUI.transform.position = quipStartingPosition;
        

        quipAnim = DOTween.Sequence();
        quipAnim.PrependInterval(pauseTimes)
            .Append(newsQuipUI.transform.DOMove(quipOffscreenPosition.position, moveTimes * acceleratedFactor).SetEase(Ease.InOutBack));
    }
    [ContextMenu("Quip Out")]
    private void QuipOutTest()
    {
        QuipOut();
    }

    public void AcceleratedTime(Component sender, object obj)
    {
        float _speed = (float)obj;

        acceleratedFactor = 1/_speed;
    }

    void fadeUI( TMP_Text textToFade,float valueToFade)
    {
        textToFade.DOFade(valueToFade, moveTimes * acceleratedFactor);
    }

    public float GetAnimTime() { return newsChangeTime * acceleratedFactor; }
}
