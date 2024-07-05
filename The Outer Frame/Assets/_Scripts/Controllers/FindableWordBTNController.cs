using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindableWordBTNController : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] GameEvent OnFindableWordButtonPress;
    [SerializeField] Texture2D[] CursorTextures;
    Color NormalColor;
    TMP_Text textField;
    WordData word;
    string OriginalText;
    int wordIndex;
    bool isHover;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField, int WordIndex)
    {
        rectTransform.sizeDelta = new Vector2(Width, Heigth);
        textField = TextField;
        NormalColor = textField.color;
        word = Word;
        OriginalText = textField.text;
        wordIndex = WordIndex;
    }

    public void ChangeToColorToHighligth()
    {
        isHover = true;
        BoldWord();
        //ChangeToColor(Color.red);
    }

    public void ChangeCusorIcon(int i)
    {
        Cursor.SetCursor(CursorTextures[i], new Vector2(95,95), CursorMode.Auto);

    }

    public void ChangeToColorToNormal()
    {
        isHover = false;
        UnBoldWord();
        //ChangeToColor(NormalColor);
    }


        void ChangeToColor(Color color)
    {
        TMP_WordInfo info = textField.textInfo.wordInfo[wordIndex];
        for (int i = 0; i < info.characterCount; ++i)
        {
            int charIndex = info.firstCharacterIndex + i;
            int meshIndex = textField.textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textField.textInfo.characterInfo[charIndex].vertexIndex;

            Color32[] vertexColors = textField.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;
        }

        textField.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void BoldWord()
    {
        string[] words = textField.text.Split(' ');
        string auxText = "";

        foreach (string w in words)
        {
            if (w.Contains(word.GetName()))
            {
                StringBuilder strBuilder = new StringBuilder(w);
                strBuilder = strBuilder.Replace(w, "<material=\"LiberationSans Findable Word Effect\">" + w + "</material>");
                auxText += strBuilder + " ";
            }
            else
            {
                auxText += w + " ";
            }

        }

        textField.text = auxText;

    }

    void UnBoldWord()
    {
        textField.text = OriginalText;
    }

    public void RegisterWord()
    {
        OnFindableWordButtonPress?.Invoke(this, word);
        ChangeToColor(NormalColor);
        Cursor.SetCursor(CursorTextures[0], Vector2.zero, CursorMode.Auto);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isHover) return;
        Vector3 mousePos = Input.mousePosition;
        float distance = Vector3.Distance(mousePos, Camera.main.WorldToScreenPoint(transform.position));
        if(distance < 70)
        {
            ChangeCusorIcon(3);
        }
        else if (distance < 120)
        {
            ChangeCusorIcon(2);
        }
        else if (distance < 200 )
        {
            ChangeCusorIcon(1);
        }
        else ChangeCusorIcon(0);

    }

}
