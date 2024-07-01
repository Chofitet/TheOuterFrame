using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookController : MonoBehaviour
{
    [SerializeField] GameObject WordPrefab;
    [SerializeField] Transform WordContainer;
    List<Transform> WordSpots = new List<Transform>();
    List<GameObject> WordsInstances = new List<GameObject>();
    [SerializeField] Transform WordAnchors;

    private void Start()
    {
        for(int i = 0; i < WordAnchors.childCount;i++)
        {
            WordSpots.Add(WordAnchors.GetChild(i));
            
        }
    }

    public void RefreshWords(Component component, object obj)
    {
        List<WordData> Words = WordSelectedInNotebook.Notebook.GetWordsList();
        DeleteWords();
        WordsInstances.Clear();

        int i = 0;

        foreach(WordData word in Words)

        {
            GameObject wordaux = Instantiate(WordPrefab, WordSpots[i].position, WordSpots[i].rotation, WordContainer);
            wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
            wordaux.GetComponent<NotebookWordInstance>().Initialization(word);
            WordsInstances.Add(wordaux);
           

            i++;
        }
    }

    public void ClearUnderLine()
    {
        foreach(GameObject word in WordsInstances)
        {
            word.GetComponent<NotebookWordInstance>().ClearUnderline();
        }
    }

    void DeleteWords()
    {
        for(int i = 0; i < WordContainer.childCount; i++)
        {
            Destroy(WordContainer.GetChild(i).gameObject);
        }
    }
    
}
