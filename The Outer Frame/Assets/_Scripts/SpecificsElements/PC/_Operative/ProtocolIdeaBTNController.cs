using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProtocolIdeaBTNController : MonoBehaviour
{
    [SerializeField] GameEvent OnAddActionInPlanAction;
    [SerializeField] StateEnum idea;
    [SerializeField] GameObject FakePosit;
    [SerializeField] GameEvent OnSetTakenPosit;
    [SerializeField] GooProtocolCompleteConditional gooConditional;
    [SerializeField] GameEvent OnCompleteGooProtocol;
    [SerializeField] GameEvent OnReactiveGooIdea;
    [SerializeField] GameObject OutOfBoardPosition;
    private bool isInactive = true;
    Sequence glowSequence;
    TMP_Text textField;

    private void OnEnable()
    {
        textField = transform.GetChild(0).GetComponent<TMP_Text>();
        isInactive = false;
        ApplyShader("bold");
    }

    private void OnDisable()
    {
        isInactive = true;
        ApplyShader("");
    }

    public void AddIdeaToAP()
    {
        OnSetTakenPosit?.Invoke(this, FakePosit);
        OnAddActionInPlanAction?.Invoke(this, idea);
        ApplyShader("");
        isInactive = true;
        GetComponent<Button>().interactable = false;
        gooConditional.SetConditionalState();
        OnCompleteGooProtocol?.Invoke(this, OutOfBoardPosition.transform);
    }

    public void ReactiveIdea(StateEnum actualIdea)
    {
        if(actualIdea != idea)
        {
            isInactive = false;
            ChangeToColorToNormal();
            GetComponent<Button>().interactable = true;
            OnReactiveGooIdea?.Invoke(this, null);
        }
    }

    public void RejectIdea(StateEnum actualIdea)
    {
        if (actualIdea == idea)
        {
            isInactive = false;
            ChangeToColorToNormal();
            GetComponent<Button>().interactable = true;
            OnReactiveGooIdea?.Invoke(this, null);
        }
    }

    public void DesactiveButton(StateEnum actualIdea)
    {
        if (actualIdea == idea)
        {
            ApplyShader("");
            isInactive = true;
            GetComponent<Button>().interactable = false;
        }
    }

    public void ChangeToColorToHighligth()
    {

        if (isInactive || !isActiveAndEnabled) return;
        ApplyShader("Red");
        GlowOn();
    }

    public void ChangeToColorToNormal()
    {
        if (isInactive || !isActiveAndEnabled) return;
        GlowOff();
    }

    public void ApplyShaderMaterial(string x)
    {
        ApplyShader(x);
    }

    void ApplyShader(string MaterialName, bool eraceSpace = true)
    {
        if (isInactive) return;

        string auxText = textField.text;

        if (auxText.StartsWith("<material")) auxText = RemoveMaterialTags(auxText);

        string materialName = string.Empty;

        if (MaterialName != "")
        {
            try
            {
                materialName = "\"" + textField.font.name + "" + MaterialName;
                auxText = auxText.Replace(auxText, "<material=" + materialName + ">" + auxText + "</material>");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error al obtener el path del material: " + ex.Message);
                auxText = auxText.Replace(auxText, auxText);
            }
        }

        textField.text = auxText;
    }

     string RemoveMaterialTags(string input)
    {
        StringBuilder sb = new StringBuilder(input.Length);
        bool insideMaterialTag = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Detectar "<material"
            if (!insideMaterialTag &&
                c == '<' &&
                i + 8 < input.Length &&
                input.Substring(i, 8).StartsWith("<materi")) // robusto ante <material...  y <material>
            {
                insideMaterialTag = true;
                continue;
            }

            // Detectar "</material"
            if (!insideMaterialTag &&
                c == '<' &&
                i + 9 < input.Length &&
                input.Substring(i, 9).StartsWith("</materi"))
            {
                insideMaterialTag = true;
                continue;
            }

            // Si estamos dentro de un tag <material ...>
            if (insideMaterialTag)
            {
                if (c == '>')
                {
                    // se cierra el tag
                    insideMaterialTag = false;
                }
                continue; // saltar todo lo que está dentro
            }

            // Copiar caracteres normales
            sb.Append(c);
        }

        return sb.ToString();
    }

public void GlowOn()
    {
        Material mat = GetMat();

        // Cancelamos cualquier animación previa
        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                0.25f,
                0.15f
            ).SetEase(Ease.InOutSine)
        );
    }

    public void GlowOff()
    {
        Material mat = GetMat();

        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        float enValue = 0.2f; //

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                enValue,
                0.15f
            ).SetEase(Ease.InOutSine)
        );

        glowSequence.OnComplete(() => ApplyShader("Bold"));
    }

    private Material GetMat()
    {
        return GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(textField.font.name);
    }
}
