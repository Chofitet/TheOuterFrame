using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FadeWordsEffect : MonoBehaviour
{
    private TextMeshProUGUI m_TextComponent;
    [SerializeField] private float FadeSpeed = 20.0f;
    private float auxfadespeed => FadeSpeed;
    [SerializeField] private int RolloverCharacterSpread = 10;
    [SerializeField] GameEvent OnEraseSound;
    public void StartEffect(bool IsFadeTransparent = true)
    {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        StopAllCoroutines();
        StartCoroutine(FadeInText(IsFadeTransparent));
    }

    public void OnStartEffect(Component sender, object obj)
    {
        /*if ((GameObject)obj != gameObject) return;
        m_TextComponent = GetComponent<TextMeshProUGUI>();
        StopAllCoroutines();
        StartCoroutine(FadeInText(true));*/
    }

    IEnumerator FadeInText(bool IsFadeTransparent)
    {
        float StartAlpha = IsFadeTransparent ? 0 : 255;
        float EndAlpha = IsFadeTransparent ? 255 : 0;

        if (!IsFadeTransparent)
        { 
            OnEraseSound?.Invoke(this, null);
        }

        Color originalColor = m_TextComponent.color;
        m_TextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, StartAlpha / 255f);

        // Forzar la actualización del texto para tener datos válidos desde el principio.
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        Color32[] newVertexColors;

        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool isRangeMax = false;

        //DefineFadeSpeedAccordingWordLength(textInfo.characterCount);

        while (!isRangeMax)
        {
            int characterCount = textInfo.characterCount;

            // Spread should not exceed the number of characters.
            byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);

            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                // Skip characters that are not visible (like white spaces)
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Calculate the new alpha value based on the fade direction.
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps * (IsFadeTransparent ? 1 : -1), 0, 255);

                // Set new alpha values.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;

                if (alpha == EndAlpha)
                {
                    startingCharacterRange += 1;

                    if (startingCharacterRange == characterCount)
                    {
                        // Update mesh vertex data one last time.
                        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        m_TextComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, EndAlpha / 255f);

                        //m_TextComponent.ForceMeshUpdate();

                        yield return new WaitForSeconds(1.0f);

                        // Reset our counters.
                        currentCharacter = 0;
                        startingCharacterRange = 0;
                        isRangeMax = true;
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacter + 1 < characterCount) currentCharacter += 1;

            yield return new WaitForSeconds(0.25f - auxfadespeed * 0.01f);
        }
    }

    /*void DefineFadeSpeedAccordingWordLength(float characterCount)
    {
        auxfadespeed = FadeSpeed;
        for (int i = 0; characterCount > i; i++)
        {
            if (i > 12) return;
            auxfadespeed += 0.3f;
        }
    }*/


}
