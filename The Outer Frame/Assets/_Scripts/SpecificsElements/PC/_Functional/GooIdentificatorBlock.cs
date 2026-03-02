using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GooIdentificatorBlock : MonoBehaviour
{

    [SerializeField] GameObject GooAnswerBlock;
    [SerializeField] GameObject AnswersPivot;
    [SerializeField] GameEvent OnChangeGooIdentificatorAnswere;

    [SerializeField] TMP_Text question;

    IdentificatorGooQuestion data;

    List<GameObject> AnswereBlocks = new List<GameObject>();
    int[] Coordinate = {0,0};

    int toggleNum;

    ToggleGroup toggleGroup;
    public void Init(IdentificatorGooQuestion _data)
    {
        data = _data;

        question.text = data.GetQuestion();

        AnswerBlockGeneration(data.GetAnswers());

    }

    void AnswerBlockGeneration(List<string> Answers)
    {
        int index = 0;
        foreach(string Answer in Answers)
        {
            GameObject AnswerBlock = Instantiate(GooAnswerBlock, AnswersPivot.transform);
            AnswerBlock.transform.GetChild(1).GetComponent<TMP_Text>().text = Answer;

            GameObject capturedBlock = AnswerBlock;
            AnswerBlock.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => PressButton(capturedBlock));
            AnswerBlock.GetComponent<GooIdentificatorAnswere>().init(index);

            AnswereBlocks.Add(AnswerBlock);
            index++;
        }
    }


    public void PressButton(GameObject pressedAnswere)
    {
        //Debug.Log(pressedAnswere.transform.GetChild(1).GetComponent<TMP_Text>().text);

        Button btn = pressedAnswere.transform.GetChild(0).GetComponent<Button>();

        //btn.interactable = false;

        foreach(GameObject A in AnswereBlocks)
        {
            if(A != pressedAnswere)
            {
                //A.transform.GetChild(0).GetComponent<Button>().interactable = true;
            }
            else
            {
                int page = data.GetPage();
                Coordinate[0] = page;
                Coordinate[1] = AnswereBlocks.IndexOf(A);
                OnChangeGooIdentificatorAnswere?.Invoke(this, Coordinate);
            }
        }
    }

    public void ResetBlock()
    {
        foreach (GameObject A in AnswereBlocks)
        {
            //A.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
    }


}


