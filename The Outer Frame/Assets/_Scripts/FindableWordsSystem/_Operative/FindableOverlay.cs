using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FindableOverlay : MonoBehaviour
{
    TMP_Text original;
    TMP_Text overlay;

    public void CreateOverlay(TMP_Text _original)
    {
        original = _original;
        if (overlay != null) return;

        GameObject go = new GameObject("TMP_Overlay");
        go.transform.SetParent(original.transform.parent);

        overlay = go.AddComponent<TextMeshProUGUI>();

        CopyTMPValues(original, overlay);

        RectTransform oRect = original.rectTransform;
        RectTransform nRect = overlay.rectTransform;

        nRect.localRotation = Quaternion.identity;

        nRect.anchorMin = oRect.anchorMin;
        nRect.anchorMax = oRect.anchorMax;
        nRect.pivot = oRect.pivot;
        nRect.sizeDelta = oRect.sizeDelta;
        nRect.localScale = Vector3.one;

        nRect.localPosition = Vector3.zero;
        nRect.anchoredPosition = Vector2.zero;

        nRect.SetAsLastSibling();

        overlay.raycastTarget = false;
    }

    void CopyTMPValues(TMP_Text source, TMP_Text target)
    {
        string json = JsonUtility.ToJson(source);
        JsonUtility.FromJsonOverwrite(json, target);
    }

    public void RemoveOverlay()
    {
        if (overlay == null) return;

        Destroy(overlay.gameObject);
        overlay = null;
    }

    public void UpdateOverlay()
    {
        if (overlay == null) return;

        overlay.text = original.text;
    }

    Sequence glowSequence;

    Material GetMat()
    {
        return GetComponent<ShaderMaterialManager>().GetBoldMaterial(original.font.name);
    }

    public void GlowOn()
    {
        if (overlay == null) return;

        Material mat = GetMat();

        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                0.2f,
                0.15f
            ).SetEase(Ease.InOutSine)
        );
    }

    public void GlowOff()
    {
        if (overlay == null) return;

        Material mat = GetMat();

        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                0.5f,
                0.15f
            ).SetEase(Ease.InOutSine)
        );


    }
}