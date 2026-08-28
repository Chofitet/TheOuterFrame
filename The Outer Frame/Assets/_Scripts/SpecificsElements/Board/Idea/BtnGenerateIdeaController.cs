using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnGenerateIdeaController : MonoBehaviour
{

    [SerializeField] GameEvent OnAddActionInPlanAction;
    [SerializeField] TMP_Text txtfield;
    StateEnum state;
    bool isInactive;

    public void Inicialization(StateEnum _State)
    {
        state = _State;
        //si ya hay algo escrito, queda eso
        if (txtfield.text == "") txtfield.text = state.GetInfinitiveVerb();
    }

    public void OnAddAction()
    {
        if (isInactive) return;
        OnAddActionInPlanAction?.Invoke(this, state);
        GetComponent<Button>().enabled = false;
        Invoke("ActiveBTN", 3f);
    }

    void ActiveBTN()
    {
        if (isInactive) return;
        GetComponent<Button>().enabled = true;
    }

    public void InactiveIdea()
    {
        txtfield.text = "<s>" + txtfield.text + "</s>";
        GetComponent<Button>().enabled = false;
        isInactive = true;
    }

    public void ActivedDesactiveIdeaBTN(bool x)
    {
        if (isInactive) return;
        GetComponent<Button>().enabled = x;
    }

    public StateEnum GetState() { return state; }


    /// Glow Material 

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

        string auxText = txtfield.text;

        if (auxText.StartsWith("<material")) auxText = RemoveMaterialTags(auxText);

        string materialName = string.Empty;

        if (MaterialName != "")
        {
            try
            {
                materialName = "\"" + txtfield.font.name.Replace(" ", "") + MaterialName;
                Debug.Log(materialName);
                auxText = auxText.Replace(auxText, "<material=" + materialName + ">" + auxText + "</material>");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error al obtener el path del material: " + ex.Message);
                auxText = auxText.Replace(auxText, auxText);
            }
        }

        txtfield.text = auxText;
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


    Sequence glowSequence;
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
                0.285f,
                0.15f
            ).SetEase(Ease.InOutSine)
        );
    }

    public void GlowOff()
    {
        Material mat = GetMat();

        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        float enValue = 0.165f; //

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
        return GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(txtfield.font.name.Replace(" ", ""));
    }


    
}
