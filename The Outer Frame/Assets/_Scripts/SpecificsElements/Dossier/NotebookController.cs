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
    bool once;
    List<int> removedIndex = new List<int>();
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

        int auxIndex = i;
        bool replaceBool = WordReplaceOther(LastWordAdded);
        if (removedIndex.Count != 0 && !once)
        {
            auxIndex = removedIndex[0];
            removedIndex.Remove(removedIndex[0]);
        }
        else if (removedIndex.Count == 0 && !replaceBool)
        {
            i++;
        }

        if (replaceBool) return;

        if (LastWordAdded.GetIsAPhoneNumber()) return;
        GameObject wordaux = Instantiate(WordPrefab, WordSpots[auxIndex].position, WordSpots[auxIndex].rotation, WordContainer);
        wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<NotebookWordInstance>().Initialization(LastWordAdded);
        WordsInstances.Add(wordaux);

        once = false;
    }

    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        once = false;
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

    //refresh when a word are erace
    public void RemoveEraceInstance(Component sender, object obj)
    {
        WordData wordToRemove = (WordData)obj;
        List<GameObject> WordsToRemove = new List<GameObject>();

        once = true;

        foreach (GameObject instanceWord in WordsInstances)
        {
            NotebookWordInstance script = instanceWord.GetComponent<NotebookWordInstance>();
            if(script.GetWord() == wordToRemove)
            {
                script.EraseAnim();
                WordsToRemove.Add(instanceWord);

                int index = WordsInstances.FindIndex(word => word == instanceWord);
                if (index != -1)
                {
                    removedIndex.Add(index);
                }
            }
        }

        StartCoroutine(delete(WordsToRemove));
    }

    IEnumerator delete(List<GameObject> list)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject w in list)
        {
            WordsInstances.Remove(w);
            Destroy(w);
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
