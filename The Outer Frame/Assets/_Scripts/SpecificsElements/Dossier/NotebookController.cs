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
    int i = 0;

    private void Start()
    {
        for(int i = 0; i < WordAnchors.childCount;i++)
        {
            WordSpots.Add(WordAnchors.GetChild(i));
            
        }
    }

    // Refresh When is added a new Word
    public void RefreshWords(Component component, object obj)
    {
        WordData LastWordAdded = (WordData)obj;

        if (WordReplaceOther(LastWordAdded)) return;

        if (LastWordAdded.GetIsAPhoneNumber()) return;
        GameObject wordaux = Instantiate(WordPrefab, WordSpots[i].position, WordSpots[i].rotation, WordContainer);
        wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<NotebookWordInstance>().Initialization(LastWordAdded);
        WordsInstances.Add(wordaux);

        i++;
    }

    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        foreach(GameObject w in WordsInstances)
        {
            if (!newword.GetWordThatReplaces()) continue;
            NotebookWordInstance script = w.GetComponent<NotebookWordInstance>();
            if (script.GetWord() == newword.GetWordThatReplaces())
            {
                script.ReplaceWord(newword);
                ClearUnderLine();
                aux = true;
            }
            
        }
        return aux;
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
