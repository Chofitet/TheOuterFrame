using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

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
    public event Action<float> OnEraseProgress;
    [SerializeField] float DilateMultiplier = 1;
    bool isVisible;
    [SerializeField] Material[] matLevels;
    private string lastCleanText = "";
    public void StartEffect(bool isFadeTransparent = true)
    {
        SetBlank(1);
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        string auxText = m_TextComponent.text;
        if (auxText.Length > 100)
        {
            auxText = lastCleanText;
        }
        else lastCleanText = auxText;

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
        SetBlank(1);
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        string auxText = m_TextComponent.text;

        if (auxText.Length > 100)
        {
            auxText = lastCleanText;
        }
        else lastCleanText = auxText;

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

        if (fadeIn)
        {
            int currentIndex = 0;
            isVisible = true;
            float elapsed = 0f;

            m_TextComponent.text = $"<alpha=#00>{text}";
            m_TextComponent.ForceMeshUpdate();

            while (currentIndex < length)
            {
                elapsed += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsed / totalDuration);
                int newIndex = Mathf.FloorToInt(progress * length);

                if (newIndex != currentIndex)
                {
                    currentIndex = newIndex;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    for (int i = 0; i < length; i++)
                    {
                        int diff = currentIndex - i;

                        if (diff == 0)
                            sb.Append($"<material=\"{matLevels[1].name}\">{text[i]}</material>");
                        else if (diff == 1)
                            sb.Append($"<material=\"{matLevels[2].name}\">{text[i]}</material>");
                        else if (diff >= 2 && diff <= 3)
                            sb.Append($"<material=\"{matLevels[3].name}\">{text[i]}</material>");
                        else if (diff < 0)
                            sb.Append($"<material=\"{matLevels[0].name}\">{text[i]}</material>");
                        else
                            sb.Append(text[i]);
                    }

                    m_TextComponent.text = sb.ToString();
                    m_TextComponent.ForceMeshUpdate();
                }

                yield return null; 
            }

            m_TextComponent.text = text;
        }
        else
        {
            isVisible = false;

            int currentIndex = 0;
            float elapsed = 0f;

            OnEraseSound?.Invoke(this, null);

            while (currentIndex < length)
            {
                elapsed += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsed / totalDuration);
                int newIndex = Mathf.FloorToInt(progress * length);

                if (newIndex != currentIndex)
                {
                    currentIndex = newIndex;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    for (int i = 0; i < length; i++)
                    {
                        if (i == currentIndex)
                            sb.Append($"<alpha=#AA>{text[i]}");
                        else if (i == currentIndex - 1)
                            sb.Append($"<alpha=#55>{text[i]}");
                        else if (i < currentIndex - 1)
                            sb.Append($"<alpha=#00>{text[i]}");
                        else
                            sb.Append($"<alpha=#FF>{text[i]}");
                    }

                    m_TextComponent.text = sb.ToString();

                    float eraseProgress = (float)currentIndex / length;
                    OnEraseProgress?.Invoke(eraseProgress);
                }

                yield return null; 
            }

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

    public void SetBlank(float i)
    {
        m_TextComponent = GetComponent<TextMeshProUGUI>();

        Color color = m_TextComponent.color;
        color.a = i;

        m_TextComponent.color = color;
    }

    public bool GetisVisible()
    {
        return isVisible;
    }

    public void SetFadeSpeed(float speed) => FadeSpeed = speed;
}
