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
    public void StartEffect(bool isFadeTransparent = true)
    {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        string auxText = m_TextComponent.text;
        if (auxText.Contains("alpha"))
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
        if (auxText.Contains("alpha"))
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

        if (fadeIn)
        {
            int currentIndex = 0;

            while (currentIndex < length)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    if (i == currentIndex) // letra recién apareciendo
                        sb.Append($"<alpha=#55>{text[i]}");
                    else if (i == currentIndex - 1) // intermedia
                        sb.Append($"<alpha=#AA>{text[i]}");
                    else if (i <= currentIndex - 2) // ya consolidada
                        sb.Append($"<alpha=#FF>{text[i]}");
                    else // todavía invisible
                        sb.Append($"<alpha=#00>{text[i]}");
                }

                m_TextComponent.text = sb.ToString();
                currentIndex++;
                yield return new WaitForSeconds(stepDuration);
            }

            // al final aseguramos el texto completo visible
            m_TextComponent.text = text;
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
                    else if (i == currentIndex + 1) // la que ya está más apagada
                        sb.Append($"<alpha=#55>{text[i]}");
                    else if (i > currentIndex + 1) // las que ya se borraron
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
