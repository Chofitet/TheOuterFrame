using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

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
    [SerializeField] GameEvent OnAnimLayoutFinish;

    [SerializeField] TMP_Text NewContentTMPtxt;
    [SerializeField]TMP_Text HeadlineTMPtxt;
    [SerializeField] TMP_Text Headline2TMPtxt;
    [SerializeField] TMP_Text QuipTMPtxt;
    
    [SerializeField] float moveTimes = 1;
    float acceleratedFactor = 1;
    [SerializeField] float pauseTimes = 1;
    [SerializeField] float newsChangeTime = 1;
    Sequence newsInAnim;
    Sequence newsOutAnim;
    Sequence pictureInAnim;
    Sequence pictureOutAnim;
    Sequence quipAnim;
    bool first;
    private List<Tween> overlayTweens = new List<Tween>();
    private Transform currentTarget;
    private Transform currentMovingObject;


    float lerpTime;

    bool moveNewsTitle;
    bool moveNewsText;
    bool movePics;
    bool moveQuip;

    Transform targetNewsTitle;
    Transform targetNewsText;
    Transform targetPics;
    Transform targetQuip;


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        textStartingPosition = newsTextUI.transform.localPosition;
        titleStartingPosition = newsTitleUI.transform.localPosition;
        quipStartingPosition = newsQuipUI.transform.localPosition;
        picsStartingPosition = picsUI.transform.localPosition;

        first = true;
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (moveNewsTitle && targetNewsTitle != null)
        {
            newsTitleUI.transform.localPosition =
                Vector3.Lerp(
                    newsTitleUI.transform.localPosition,
                    targetNewsTitle.localPosition,
                    lerpTime);
        }

        if (moveNewsText && targetNewsText != null)
        {
            newsTextUI.transform.localPosition =
                Vector3.Lerp(
                    newsTextUI.transform.localPosition,
                    targetNewsText.localPosition,
                    lerpTime);
        }

        if (movePics && targetPics != null)
        {
            picsUI.transform.localPosition =
                Vector3.Lerp(
                    picsUI.transform.localPosition,
                    targetPics.localPosition,
                    lerpTime);
        }

        if (moveQuip && targetQuip != null)
        {
            newsQuipUI.transform.localPosition =
                Vector3.Lerp(
                    newsQuipUI.transform.localPosition,
                    targetQuip.localPosition,
                    lerpTime);
        }
    }


    // =====================================================
    // NEWS IN
    // =====================================================

    public void NewsIn()
    {
        if (newsInAnim != null && newsInAnim.IsActive())
            newsInAnim.Kill();

        newsTitleUI.transform.localPosition =
            titleOffscreenPositionLeft.localPosition;

        newsTextUI.transform.localPosition =
            textOffscreenPositionRight.localPosition;

        SetAlpha(NewContentTMPtxt, 0);
        SetAlpha(HeadlineTMPtxt, 0);

        newsInAnim = DOTween.Sequence();
        overlayTweens.Add(newsInAnim);

        newsInAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetNewsTitle = CreateLocalTarget(titleStartingPosition);
                targetNewsText = CreateLocalTarget(textStartingPosition);

                moveNewsTitle = true;
                moveNewsText = true;
                lerpTime = 0;
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .JoinCallback(() =>
            {
                fadeUI(NewContentTMPtxt, 1, moveTimes );
                fadeUI(HeadlineTMPtxt, 1, moveTimes );
                fadeUI(Headline2TMPtxt, 1, moveTimes );
                fadeUI(QuipTMPtxt, 1, moveTimes);
            })

            .OnComplete(() =>
            {
                moveNewsTitle = false;
                moveNewsText = false;
            });
    }


    // =====================================================
    // NEWS OUT
    // =====================================================

    public void NewsOut()
    {
        if (newsOutAnim != null && newsOutAnim.IsActive())
            newsOutAnim.Kill();

        newsTitleUI.transform.localPosition = titleStartingPosition;
        newsTextUI.transform.localPosition = textStartingPosition;

        newsOutAnim = DOTween.Sequence();
        overlayTweens.Add(newsOutAnim);

        newsOutAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetNewsTitle = titleOffscreenPositionRight;
                targetNewsText = textOffscreenPositionLeft;

                moveNewsTitle = true;
                moveNewsText = true;
                lerpTime = 0;
            })
            .JoinCallback(() =>
            {
                fadeUI(NewContentTMPtxt, 0, moveTimes *0.3f);
                fadeUI(HeadlineTMPtxt, 0, moveTimes * 0.3f);
                fadeUI(QuipTMPtxt, 0, moveTimes * 0.3f);
                fadeUI(Headline2TMPtxt, 0, moveTimes * 0.3f);
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .OnComplete(() =>
            {
                moveNewsTitle = false;
                moveNewsText = false;

                OnAnimLayoutFinish?.Invoke(this, null);
            });
    }


    // =====================================================
    // PICS IN
    // =====================================================

    public void PicsIn()
    {
        if (pictureInAnim != null && pictureInAnim.IsActive())
            pictureInAnim.Kill();

        picsUI.transform.localPosition =
            picsOffscreenPosition.localPosition;

        pictureInAnim = DOTween.Sequence();
        overlayTweens.Add(pictureInAnim);

        pictureInAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetPics = CreateLocalTarget(picsStartingPosition);
                movePics = true;
                lerpTime = 0;
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .OnComplete(() => movePics = false);
    }


    // =====================================================
    // PICS OUT
    // =====================================================

    public void PicsOut()
    {
        if (pictureOutAnim != null && pictureOutAnim.IsActive())
            pictureOutAnim.Kill();

        picsUI.transform.localPosition =
            picsStartingPosition;

        pictureOutAnim = DOTween.Sequence();
        overlayTweens.Add(pictureOutAnim);

        pictureOutAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetPics = picsOffscreenPosition;
                movePics = true;
                lerpTime = 0;
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .OnComplete(() => movePics = false);
    }


    // =====================================================
    // QUIP IN
    // =====================================================

    public void QuipIn()
    {
        if (quipAnim != null && quipAnim.IsActive())
            quipAnim.Kill();

        newsQuipUI.transform.localPosition =
            quipOffscreenPosition.localPosition;

        SetAlpha(QuipTMPtxt, 0);

        quipAnim = DOTween.Sequence();
        overlayTweens.Add(quipAnim);

        quipAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetQuip = CreateLocalTarget(quipStartingPosition);
                moveQuip = true;
                lerpTime = 0;
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .OnComplete(() => moveQuip = false);
    }


    // =====================================================
    // QUIP OUT
    // =====================================================

    public void QuipOut()
    {
        if (quipAnim != null && quipAnim.IsActive())
            quipAnim.Kill();

        newsQuipUI.transform.localPosition =
            quipStartingPosition;

        quipAnim = DOTween.Sequence();
        overlayTweens.Add(quipAnim);

        quipAnim
            .PrependInterval(pauseTimes)

            .AppendCallback(() =>
            {
                targetQuip = quipOffscreenPosition;
                moveQuip = true;
                lerpTime = 0;
            })

            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    moveTimes * acceleratedFactor
                ).SetEase(Ease.InOutBack)
            )

            .OnComplete(() => moveQuip = false);
    }


    // =====================================================
    // HELPERS
    // =====================================================

    Transform CreateLocalTarget(Vector3 localPos)
    {
        GameObject go = new GameObject("TempTarget");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    void SetAlpha(TMP_Text txt, float a)
    {
        Color c = txt.color;
        c.a = a;
        txt.color = c;
    }

    public void AcceleratedTime(Component sender, object obj)
    {
        float _speed = (float)obj;

        overlayTweens.RemoveAll(s => s == null);

        if (_speed <= 0)
        {
            acceleratedFactor = 0.0001f;

            foreach (var tween in overlayTweens)
            {
                tween.Pause();
            }
        }
        else
        {
            foreach (var tween in overlayTweens)
            {
                tween.Play();
            }

            if(_speed < 1 && _speed != 0)
            {
                acceleratedFactor = 1;
            }
            else
            {
                acceleratedFactor = 1 / _speed;
            }
            Debug.Log("accelerator factor: " + acceleratedFactor);
        }
    }

    void fadeUI( TMP_Text textToFade,float valueToFade, float fadeTime)
    {
        textToFade.DOFade(valueToFade, (fadeTime * acceleratedFactor)).SetEase(Ease.InQuad);

    }

    public float GetAnimTime() { return newsChangeTime * acceleratedFactor; }

    
}


