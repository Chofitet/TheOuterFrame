using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPhonesController : MonoBehaviour
{
    [SerializeField] GameObject PhoneNumberPrefab;
    [SerializeField] Transform WordContainer;
    List<GameObject> WordsInstances = new List<GameObject>();
    int i = 0;

    // Refresh When is added a new Phone
    public void RefreshPhones(Component component, object obj)
    {
       WordData LastPhoneAdded = (WordData)obj;

        if (WordReplaceOther(LastPhoneAdded)) return;

        if (LastPhoneAdded.GetIsAPhoneNumber() && WordsInstances.Count !=0)
        {
            foreach (GameObject phone in WordsInstances)
            {
                PhoneRowNotebookController PhoneScript = phone.GetComponent<PhoneRowNotebookController>();
                if (PhoneScript.GetWord().GetName() == LastPhoneAdded.GetName())
                {
                    if(FindWordToReplaceNum(LastPhoneAdded))
                    {
                        PhoneScript.ReplaceNumberWithWord(FindWordToReplaceNum(LastPhoneAdded));
                        return;
                    }
                }
            }
        }

        if (LastPhoneAdded.GetIsPhoneNumberFound())
        {
            foreach(GameObject phone in WordsInstances)
            {
                PhoneRowNotebookController PhoneScript = phone.GetComponent<PhoneRowNotebookController>();
               if ( PhoneScript.GetWord().GetPhoneNumber() == LastPhoneAdded.GetPhoneNumber())
                {
                    PhoneScript.UpdateNumber();
                    return;
                }
            }
        }

       GameObject wordaux = Instantiate(PhoneNumberPrefab, WordContainer);
       wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
       wordaux.GetComponent<PhoneRowNotebookController>().Initialization(LastPhoneAdded);
       WordsInstances.Add(wordaux);

       i++;
    }

    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        foreach (GameObject w in WordsInstances)
        {
            if (!newword.GetWordThatReplaces()) continue;
            PhoneRowNotebookController script = w.GetComponent<PhoneRowNotebookController>();
            if (script.GetWord() == newword.GetWordThatReplaces())
            {
                script.ReplaceNumber(newword);
                ClearUnderLine();
                aux = true;
            }

        }
        return aux;
    }

    WordData FindWordToReplaceNum(WordData Num)
    {
        List<WordData> words = WordSelectedInNotebook.Notebook.GetWordsList();
        foreach (WordData word in words)
        {
            if(Num.GetName() == word.GetPhoneNumber())
            {
                return word;
            }
        }

        return null;
    }

    public void ClearUnderLine()
    {
        foreach (GameObject word in WordsInstances)
        {
            word.GetComponent<PhoneRowNotebookController>().ClearUnderline();
        }
    }

    void DeleteWords()
    {
        for (int i = 0; i < WordContainer.childCount; i++)
        {
            Destroy(WordContainer.GetChild(i).gameObject);
        }
    }

    public void DisablePhoneBTN(Component sender, object obj)
    {
        /*if (obj is not bool) return;

        if (WordContainer.childCount == 0) return;
        GameObject[] btns = WordContainer.gameObject.GetComponentsInChildren<GameObject>();
        
        foreach (GameObject btn in btns)
        {
            btn.SetActive(!(bool)obj);
        }*/
    }
}
