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
    float currentDelay;

    Sequence ZoomSequence;

    [Header("Clamps locales")]
    [SerializeField] Vector2 xLimits = new Vector2(-2f, 2f); 
    [SerializeField] Vector2 yLimits = new Vector2(-1f, 1f);

    bool inZoom = false;

    public void SetInZoomIn(Component sender, object obj)
    {
        inZoom = true;
        if (ZoomSequence.IsActive() & ZoomSequence != null) ZoomSequence.Kill();
        currentDelay = 0f;
    }

    public void SetInZoomOut(Component sender, object obj)
    {
        inZoom = false;

        if(ZoomSequence.IsActive() & ZoomSequence != null) ZoomSequence.Kill();

        ZoomSequence = DOTween.Sequence();

        ZoomSequence.Append(target.transform.DOMove(initPosition.position, 0.5f).SetEase(Ease.InOutSine));
    }

    private void Update()
    {
        if (inZoom) FollowMouse();
    }

    void FollowMouse()
    {
        float normX = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float normY = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        float targetX = Mathf.Lerp(xLimits.x, xLimits.y, (normX + 1f) / 2f);
        float targetY = Mathf.Lerp(yLimits.x, yLimits.y, (normY + 1f) / 2f);

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

}
