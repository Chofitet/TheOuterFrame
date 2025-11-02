using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using static Cinemachine.AxisState;

public class ZoomSystem : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Transform initPosition;
    [SerializeField] float followDelay;
    [SerializeField] GameEvent OnButtonElement;
    [SerializeField] bool startHolding;
    bool isHolding;
    float currentDelay;
    bool once = true;
    Sequence ZoomSequence;
    bool isInBoardView;

    [Header("Clamps locales")]
    [SerializeField] Vector2 xLimits = new Vector2(-2f, 2f); 
    [SerializeField] Vector2 yLimits = new Vector2(-1f, 1f);

    [SerializeField] Vector2 xTutorialLimits = new Vector2(-2f, 2f);
    [SerializeField] Vector2 yTutorialLimits = new Vector2(-1f, 1f);

    Vector2 XLimits;
    Vector2 YLimits;

    bool inZoom = false;

    public void SetInZoomIn(Component sender, object obj)
    {
        if (!isInBoardView) return;
        if (!startHolding)
        {
            isHolding = true;
            StartCoroutine(WaitToZoom());
        }
    }

    void SetZoom()
    {
        OnButtonElement?.Invoke(this, ViewStates.BoardZoomView);
        inZoom = true;
        if (ZoomSequence.IsActive() & ZoomSequence != null) ZoomSequence.Kill();
        currentDelay = 0f;
        once = false;
    }

    public void SetInZoomOut(Component sender, object obj)
    {
        if (once) return;
        inZoom = false;

        if(ZoomSequence.IsActive() & ZoomSequence != null) ZoomSequence.Kill();

        ZoomSequence = DOTween.Sequence();

        ZoomSequence.Append(target.transform.DOMove(initPosition.position, 0.5f).SetEase(Ease.InOutSine));

        OnButtonElement?.Invoke(null, ViewStates.BoardView);

        once = true;
    }

    private void Update()
    {
        
        if(Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            SetInZoomOut(null, null);
        }
        if (inZoom && isHolding) FollowMouse();

    }

    void FollowMouse()
    {
        float normX = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float normY = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        float targetX = Mathf.Lerp(XLimits.x, XLimits.y, (normX + 1f) / 2f);
        float targetY = Mathf.Lerp(YLimits.x, YLimits.y, (normY + 1f) / 2f);

        // 3. Si querés usar Z, podés ligarlo al scroll del mouse o a otra lógica
        float targetZ = 0;

        Vector3 desiredLocal = new Vector3(targetX, targetY, targetZ);

        target.transform.localPosition = Vector3.Lerp(
            target.transform.localPosition,
            desiredLocal,
            currentDelay * Time.deltaTime
        );

        currentDelay = followDelay;
    }

    IEnumerator WaitToZoom()
    {
        startHolding = true;
        yield return new WaitForSeconds(0.2f);
        startHolding = false;
        if (isHolding) SetZoom();
    }

    public void CkeckView(Component sender,object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.BoardView || view == ViewStates.BoardZoomView)
        {
            isInBoardView = true;
        }
        else
        {
            isInBoardView = false;
        }
    }

    public void SetTutorial(Component sender, object obj)
    {
        bool isInTutorial = (bool)obj;

        if (isInTutorial)
        {
            XLimits = xTutorialLimits;
            YLimits = yTutorialLimits;
        }
        else
        {
            XLimits = xLimits;
            YLimits = yLimits;
        }
    }
}
