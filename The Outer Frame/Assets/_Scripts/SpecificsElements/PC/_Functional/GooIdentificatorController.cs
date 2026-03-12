using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GooIdentificatorController : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject QuestionBlock;
    [SerializeField] GameObject QuestionBlockPivot;
    [SerializeField] List<IdentificatorGooQuestion> IdentificatorQuestions = new List<IdentificatorGooQuestion>();
    [SerializeField] List<CodeArray> RightAnswerers = new List<CodeArray>();
    [SerializeField] GameObject ComponentAccessWindow;
    [SerializeField] GameObject BackButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject QuitButton;
    [SerializeField] GameEvent OnDisableSearchBar;

    [SerializeField] GameObject RightAnswerePanel;
    [SerializeField] GameObject WrongAnswerePanel;

    [SerializeField] ProtocolIdeaBTNController ProtocolIdea;

    [SerializeField] List<ConditionalClass> InactiveGooIdea;

    

    bool isAccessComponentRight = false;
 
    int[] RightAnswere = { -1, -1, -1, -1 };
  

    List<GameObject> Pages = new List<GameObject>();

    int IdentficatorPage;

   

    int[] playerCode= {-1,-1,-1,-1};
    private bool isOrderMatters;

    private void Start()
    {
        int index = 0;
        foreach(IdentificatorGooQuestion id in IdentificatorQuestions)
        {
            id.SetPage(index);
            GameObject questionBlock = Instantiate(QuestionBlock, QuestionBlockPivot.transform);
            questionBlock.transform.SetSiblingIndex(0);
            questionBlock.GetComponent<GooIdentificatorBlock>().Init(IdentificatorQuestions[index]);
            questionBlock.gameObject.SetActive(false);
            Pages.Add(questionBlock);

            RightAnswere[index] = id.GetCorrectAnswer();
            
            index++;
        }
        RightAnswerers.Add(new CodeArray(RightAnswere));

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
        else
        {
            SetASlotAnswered(IdentficatorPage +1 , -1); //resetSlot
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

        /*if (playerCode[IdentficatorPage] != -1 && !ComponentAccessWindow.transform.GetChild(0).gameObject.activeSelf)
        {
            //next button se muestra si ya se respondió la pregunta
            NextButton.SetActive(true);
        }*/

    }

    public int GetIdentificatorPage() { return IdentficatorPage; }

    public void SetIdentificatorCode(Component sender, object obj)
    {

        int[] coordinate = (int[])obj;

        SetASlotAnswered(coordinate[0], coordinate[1]);

        if(playerCode.Length == IdentificatorQuestions.Count)
        {
            string finalCode = string.Join(", ", playerCode);
            Debug.Log($"All Answers have been Answered. Final Code: {finalCode}");
        }

        pressNext();
    }

    void SetASlotAnswered(int x, int y)
    {
        playerCode[x] = y;
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
        isAccessComponentRight = (bool)obj;
        bool isOneCodeCorrect = false;

        foreach(CodeArray code in RightAnswerers)
        {
            isOneCodeCorrect = code.values.SequenceEqual(playerCode);

            if (isOneCodeCorrect) break;
        }

        if(isOneCodeCorrect && isAccessComponentRight)
        {
            StartCoroutine(DelayShowAnswere(RightAnswerePanel));
        }
        else
        {
            StartCoroutine(DelayShowAnswere(WrongAnswerePanel));
        }
    }

    IEnumerator DelayShowAnswere(GameObject panel)
    {
        QuitButton.GetComponent<Button>().interactable = false;
        BackButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        QuitButton.GetComponent<Button>().interactable = true;
        panel.SetActive(true);
    }

    public void ReactiveProtocolIdea(Component sender, object obj)
    {
        ProtocolIdea.ReactiveIdea((StateEnum)obj);
    }

    public void RejectProtocolIdea(Component sender, object obj)
    {
        ProtocolIdea.RejectIdea((StateEnum)obj);
    }

    public void AddProtocolIdeaFromBoard(Component sender, object obj)
    {
        ProtocolIdea.DesactiveButton((StateEnum)obj);
    }

    public void OnCheckView(Component sender, object obj)
    {
        ViewStates actualView = (ViewStates)obj;

        if(CheckForConditionals(InactiveGooIdea))
        {
            ProtocolIdea.InactiveButton();
        }
    }
    public void ActionIsDoing(Component sender, object obj)
    {
        DataFromActionPlan data = (DataFromActionPlan)obj;
        StateEnum action = data.state;

        ProtocolIdea.DesactiveButton(action,true);
    }

    public void Reset()
    {
        playerCode = new int[]{ -1,-1,-1,-1};
        IdentficatorPage = 0;

        WrongAnswerePanel.SetActive(false);
        RightAnswerePanel.SetActive(false);

        foreach (GameObject page in Pages)
        {
            page.GetComponent<GooIdentificatorBlock>().ResetBlock();
        }

        setActualPage();
    }

    public bool CheckForConditionals(List<ConditionalClass> ListOfConditionals)
    {
        try
        {
            if (ListOfConditionals.Count == 0) return true;

            foreach (ConditionalClass conditional in ListOfConditionals)
            {
                try
                {
                    IConditionable auxInterface = conditional.condition as IConditionable;

                    if (auxInterface == null)
                        throw new Exception("La condición no implementa IConditionable.");

                    bool conditionState = auxInterface.GetStateCondition(2);

                    if (conditional.ifNot)
                    {
                        conditionState = !conditionState;
                    }

                    if (!conditionState)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error en el condicional: {conditional.condition.name}. Detalles: {ex.Message}", ex);
                }
            }

            if (isOrderMatters) return CheckIfConditionalAreInOrder(ListOfConditionals);
            else return true;
        }
        catch (Exception ex)
        {
            // Mensaje de error general con la excepción específica
            Debug.LogError($"Error al evaluar los condicionales. Detalles: {ex.Message}");
            return false;
        }
    }

    bool CheckIfConditionalAreInOrder(List<ConditionalClass> ListOfConditionals)
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in ListOfConditionals)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
            }
        }
        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
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

[Serializable]
public class CodeArray
{
    public int[] values = { -1, -1, -1, -1 };

    public CodeArray(int[] _values)
    {
        this.values = _values;
    }
}