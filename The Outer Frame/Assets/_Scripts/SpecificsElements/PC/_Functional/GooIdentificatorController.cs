using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GooIdentificatorController : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject QuestionBlock;
    [SerializeField] GameObject QuestionBlockPivot;
    [SerializeField] List<IdentificatorGooQuestion> IdentificatorQuestions = new List<IdentificatorGooQuestion>();
    [SerializeField] GameObject ComponentAccessWindow;
    [SerializeField] GameObject BackButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameEvent OnDisableSearchBar;

    [SerializeField] GameObject RightAnswerePanel;
    [SerializeField] GameObject WrongAnswerePanel;
 
    int[] RightAnsweres = { -1, -1, -1, -1, -1 };

    List<GameObject> Pages = new List<GameObject>();

    int IdentficatorPage;

   

    int[] playerCode= {-1,-1,-1,-1,-1};

    private void Start()
    {
        int index = 0;
        foreach(IdentificatorGooQuestion id in IdentificatorQuestions)
        {
            id.SetPage(index);
            GameObject questionBlock = Instantiate(QuestionBlock, QuestionBlockPivot.transform);
            questionBlock.GetComponent<GooIdentificatorBlock>().Init(IdentificatorQuestions[index]);
            questionBlock.gameObject.SetActive(false);
            Pages.Add(questionBlock);

            RightAnsweres[index] = id.GetCorrectAnswer();

            index++;
        }

        Pages[0].gameObject.SetActive(true);

    }

    public void pressNext()
    {
        IdentficatorPage += 1;

        if (IdentficatorPage == IdentificatorQuestions.Count)
        {
            ComponentAccessWindow.GetComponent<ComponentEnterWindow>().ShowPanel(true);
        }

        IdentficatorPage = Math.Clamp(IdentficatorPage, 0, IdentificatorQuestions.Count - 1);

        setActualPage();
    }

    public void pressBack()
    {
        IdentficatorPage -= 1;
        IdentficatorPage = Math.Clamp(IdentficatorPage, 0, IdentificatorQuestions.Count - 1);


        if(ComponentAccessWindow.transform.GetChild(0).gameObject.activeSelf)
        {
            // está en la pagina del componente

            IdentficatorPage = IdentificatorQuestions.Count - 1; //lopone automaticamente en la ultima
            ComponentAccessWindow.GetComponent<ComponentEnterWindow>().ShowPanel(false);
        }
        setActualPage();
    }

    void setActualPage()
    {
        turnOffPages();
        if (Pages[IdentficatorPage] == null) return;
        Pages[IdentficatorPage].SetActive(true);
    }

    void turnOffPages()
    {
        foreach(GameObject page in Pages)
        {
            page.SetActive(false);
        }

        TurnOnOffNavegatingButtons();
    }


    void TurnOnOffNavegatingButtons()
    {
        NextButton.SetActive(false);
        BackButton.SetActive(false);

        if(IdentficatorPage != 0)
        {
            //back button se muestra en todas menos la primera
            BackButton.SetActive(true);
        }

        if (playerCode[IdentficatorPage] != -1 && !ComponentAccessWindow.transform.GetChild(0).gameObject.activeSelf)
        {
            //next button se muestra si ya se respondió la pregunta
            NextButton.SetActive(true);
        }

    }

    public int GetIdentificatorPage() { return IdentficatorPage; }

    public void SetIdentificatorCode(Component sender, object obj)
    {
        int[] coordinate = (int[])obj;

        playerCode[coordinate[0]]= coordinate[1];

        if(playerCode.Length == IdentificatorQuestions.Count)
        {
            string finalCode = string.Join(", ", playerCode);
            Debug.Log($"All Answers have been Answered. Final Code: {finalCode}");
        }
    }

    public void ActiveGooPanel(Component sender, object obj)
    {
        content.SetActive(true);
        OnDisableSearchBar?.Invoke(this, true);
    }

    public void DesactiveGooPanel()
    {
        content.SetActive(false);
        OnDisableSearchBar?.Invoke(this, false);
    }

    public void CheckFinalAnsweres(Component sender, object obj)
    {
        bool areEqual = RightAnsweres.SequenceEqual(playerCode);

        if(areEqual)
        {
            RightAnswerePanel.SetActive(true);
        }
        else
        {
            WrongAnswerePanel.SetActive(true);
        }
    }

    public void Reset()
    {
        playerCode = new int[]{ -1,-1,-1,-1,-1};
        IdentficatorPage = 0;

        WrongAnswerePanel.SetActive(false);

        foreach (GameObject page in Pages)
        {
            page.GetComponent<GooIdentificatorBlock>().ResetBlock();
        }

        setActualPage();
    }
}

[Serializable]
public class IdentificatorGooQuestion
{
    [SerializeField] string question;
    [SerializeField] List<string> answers;
    [SerializeField] int correctAnswer;
    int NumPage;

    public void SetPage(int page)
    {
        NumPage = page;
    }
    public int GetPage() { return NumPage; }

    public string GetQuestion() { return question;}
    public List<string> GetAnswers() { return answers; }
    public int GetCorrectAnswer() {  return correctAnswer;}
}