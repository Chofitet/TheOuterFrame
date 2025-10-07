using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FadeWordsEffect : MonoBehaviour
{
    private TextMeshProUGUI m_TextComponent;
    [SerializeField] private float FadeSpeed = 0.1f;
    private float auxfadespeed;
    [SerializeField] private int RolloverCharacterSpread = 10;
    [SerializeField] GameEvent OnEraseSound;
    Coroutine fadeCoroutine;
    public Action OnComplete;
    public void StartEffect(bool isFadeTransparent = true)
    {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        string auxText = m_TextComponent.text;
        if (auxText.Contains("material"))
        {
            m_TextComponent.text = "";
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInText(isFadeTransparent, auxText));
    }

    public void OnStartEffect(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) return;
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        string auxText = m_TextComponent.text;
        if (auxText.Contains("material"))
        {
            m_TextComponent.text = "";
            return;
        }
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInText(true, auxText));
    }

    IEnumerator FadeInText(bool fadeIn, string text)
    {
        int length = text.Length;
        float totalDuration = FadeSpeed;
        float stepDuration = totalDuration / Mathf.Max(1, length);

        // Materiales de más claro a más oscuro
        string[] matLevels = {
        "NipCensHandwritingLightSDFWriting1", // más claro
        "NipCensHandwritingLightSDFWriting2",
        "NipCensHandwritingLightSDFWriting3",
        "NipCensHandwritingLightSDFWriting4"  // más oscuro
    };

        if (fadeIn)
        {
            int currentIndex = 0;

            while (currentIndex < length)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    int diff = currentIndex - i;

                    if (diff == 0)
                        sb.Append($"<material=\"{matLevels[1]}\">{text[i]}</material>"); // letra actual -> mat 2
                    else if (diff == 1)
                        sb.Append($"<material=\"{matLevels[2]}\">{text[i]}</material>"); // inmediata izquierda -> mat 3
                    else if (diff >= 2 && diff <= 3)
                        sb.Append($"<material=\"{matLevels[3]}\">{text[i]}</material>"); // dos siguientes a la izquierda -> mat 4
                    else if (diff < 0)
                        sb.Append($"<material=\"{matLevels[0]}\">{text[i]}</material>"); // aún no dibujadas
                    else
                        sb.Append(text[i]); // resto de letras ya dibujadas -> normal
                }

                m_TextComponent.text = sb.ToString();
                m_TextComponent.ForceMeshUpdate();

                currentIndex++;
                yield return new WaitForSeconds(stepDuration);
            }

            m_TextComponent.text = text; // al final todo normal
        }
        else
        {
            int currentIndex = length - 1;
            OnEraseSound?.Invoke(this, null);

            while (currentIndex >= 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    if (i == currentIndex) // letra empezando a borrarse
                        sb.Append($"<alpha=#AA>{text[i]}");
                    else if (i == currentIndex + 1) // más apagada
                        sb.Append($"<alpha=#55>{text[i]}");
                    else if (i > currentIndex + 1) // ya borradas
                        sb.Append($"<alpha=#00>{text[i]}");
                    else // todavía visibles
                        sb.Append($"<alpha=#FF>{text[i]}");
                }

                m_TextComponent.text = sb.ToString();
                currentIndex--;
                yield return new WaitForSeconds(stepDuration);
            }

            // al final dejamos todo borrado
            m_TextComponent.text = "";
        }

        OnComplete?.Invoke();
    }






    void DefineFadeSpeedAccordingWordLength(float characterCount)
    {
        auxfadespeed = FadeSpeed;
        for (int i = 0; characterCount > i; i++)
        {
            if (i > 12) return;
            auxfadespeed += 0.3f;
        }
    }


}
