using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPhonesController : MonoBehaviour
{
    [SerializeField] GameObject PhoneNumberPrefab;
    [SerializeField] Transform WordContainer;
    List<GameObject> WordsInstances = new List<GameObject>();

    public void RefreshPhones(Component component, object obj)
    {
        List<WordData> Words = WordSelectedInNotebook.Notebook.GetWordsList();
        DeleteWords();
        WordsInstances.Clear();

        int i = 0;

        foreach (WordData word in Words)
        {
            if (word.GetPhoneNumber() == "") continue;
            GameObject wordaux = Instantiate(PhoneNumberPrefab, WordContainer);
            wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
            wordaux.GetComponent<PhoneRowNotebookController>().Initialization(word);
            WordsInstances.Add(wordaux);

            i++;
        }
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
}
