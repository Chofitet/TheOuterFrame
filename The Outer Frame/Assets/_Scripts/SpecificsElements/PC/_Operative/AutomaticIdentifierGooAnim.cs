using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutomaticIdentifierGooAnim : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    [SerializeField] TMP_Text pointText;
    [SerializeField] GameObject lessAutomaticBTN;

    string originalText;
    private void Start()
    {
        originalText = textField.text;
        Reset();
    }

    public void OpenAutomaticIdentifierPanel(Component sender, object obj)
    {
        StartBootingAnim();
    }

    public void CloseAutomaticIdentifierPanel(Component sender, object obj)
    {
        CloseAutomaticIdentifierPanel();
        
    }

    public void CloseAutomaticIdentifierPanel()
    {
        Debug.Log("Closing panel");
        if (BootingCoroutine != null) StopCoroutine(BootingCoroutine);
        Reset();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void StartBootingAnim()
    {
        BootingCoroutine = StartCoroutine(BootingAnimation());
    }

    Coroutine BootingCoroutine;
    IEnumerator BootingAnimation()
    {
        Reset();

        float duration = 1.2f;
        float interval = 0.4f;
        float timer = 0f;
        int dotCount = 0;

        while (timer < duration)
        {
            dotCount = (dotCount + 1) % 4;
            pointText.text = "BOOTING" + new string('.', dotCount);

            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        pointText.text = "";
        textField.text = originalText;

        textField.text = "";

        string[] lines = originalText.Split('\n');

        int linesAmount = lines.Length;
        int lineIndex = 1;

        foreach (string line in lines)
        {
            
            textField.text += line + "\n";
            if(lineIndex != linesAmount) yield return new WaitForSeconds(0.15f);

            lineIndex++;
        }

        lessAutomaticBTN.SetActive(true);
    }

    private void Reset()
    {
        textField.text = "";
        pointText.text = "";
        lessAutomaticBTN.SetActive(false);
        
    }
}
