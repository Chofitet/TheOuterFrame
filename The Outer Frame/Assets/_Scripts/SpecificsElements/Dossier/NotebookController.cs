using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookController : MonoBehaviour
{
    [SerializeField] GameObject WordPrefab;
    [SerializeField] Transform WordContainer;
    [SerializeField] WordData CabinWord;
    List<Transform> WordSpots = new List<Transform>();
    List<GameObject> WordsInstances = new List<GameObject>();
    [SerializeField] Transform WordAnchors;
    List<WordData> InctiveWordsOnBoard = new List<WordData>();
    int i = 0;
    bool once;
    bool isStarting = true;

    List<int> removedIndex = new List<int>();
    private void Start()
    {
        for(int i = 0; i < WordAnchors.childCount;i++)
        {
            WordSpots.Add(WordAnchors.GetChild(i));
        }

        Invoke("SetisStartingFalse", 1f);

    }

    void SetisStartingFalse()
    {
        isStarting = false;
        InctiveWordsOnBoard = new List<WordData>(WordSelectedInNotebook.Notebook.GetWordsInBeggining());
    }

    // Refresh When is added a new Word
    public void RefreshWords(Component component, object obj)
    {
        WordData LastWordAdded = (WordData)obj;

        

        int auxIndex = i;

        bool isOutOfRange = false;
        if (i >= WordAnchors.childCount - 1) isOutOfRange = true;


        bool replaceBool = WordReplaceOther(LastWordAdded);

        if (removedIndex.Count == 0 && !replaceBool && !isOutOfRange)
        {
            i++;
        }
         else if (removedIndex.Count != 0 && !once)
        {
            auxIndex = removedIndex[0];
            removedIndex.Remove(removedIndex[0]);
        }
        else if(isOutOfRange && !replaceBool)
        {
            auxIndex = SearchIndexOfCrossWord(LastWordAdded);
            removedIndex.Remove(removedIndex[0]);
            return;
        }

        if (replaceBool) return;

        if (LastWordAdded.GetIsAPhoneNumber()) return;

        GameObject wordaux = Instantiate(WordPrefab, WordSpots[auxIndex].position, WordSpots[auxIndex].rotation, WordContainer);
        wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<NotebookWordInstance>().Initialization(LastWordAdded, isStarting);
        WordsInstances.Add(wordaux);

        once = false;
    }

    int SearchIndexOfCrossWord(WordData newword)
    {
        List<GameObject> WordsToRemove = new List<GameObject>();
        int _index = 0;
        foreach (GameObject w in WordsInstances)
        {
            NotebookWordInstance script = w.GetComponent<NotebookWordInstance>();
            if (script.GetWord().GetInactiveState())
            {
                script.ReplaceWord(newword);
                ClearUnderLine();

                _index = WordsInstances.FindIndex(word => word == w);
                if (_index != -1)
                {
                    removedIndex.Add(_index);
                }
                break;
            }
        }
        return _index;
    }


    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        once = false;
        foreach(GameObject w in WordsInstances)
        {
            if (!newword.GetWordThatReplaces()) continue;
            NotebookWordInstance script = w.GetComponent<NotebookWordInstance>();
            if (SearchForWordThatReplaceRetroactive(script.GetWord(), newword))
            {
                ClearUnderLine();
                script.ReplaceWord(newword);
                aux = true;
            }
            
        }
        return aux;
    }

    public void ReplaceAllWithCabin(Component sender, object obj)
    {
        foreach(GameObject w in WordsInstances)
        {
            NotebookWordInstance script = w.GetComponent<NotebookWordInstance>();

            script.ReplaceWordInstantly(CabinWord);
        }
    }

    bool SearchForWordThatReplaceRetroactive(WordData oldWord, WordData newWord)
    {
        WordData currentWord = newWord.GetWordThatReplaces();
        WordData startWord = oldWord;

        while (currentWord != null)
        {
            if (currentWord == startWord) 
                return true;

            currentWord.SetIsFound(); 
            currentWord = currentWord.GetWordThatReplaces();
        }

        return false; 
    }



    //refresh when a word are erace
    public void RemoveEraceInstance(Component sender, object obj)
    {
        WordData wordToRemove = (WordData)obj;
        List<GameObject> WordsToRemove = new List<GameObject>();

        string see = sender.name;

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

        delete(WordsToRemove);
    }

    public void DeleteAllWords(Component sender, object obj)
    {
        List<GameObject> auxList = new List<GameObject>(WordsInstances);

        delete(auxList);
        i = 0;
    }

    void delete(List<GameObject> list)
    {
        foreach (GameObject w in list)
        {
            WordsInstances.Remove(w);
            Destroy(w);
        }
    }

    public void PutingWordOnBoard(Component sender, object obj)
    {
        InctiveWordsOnBoard.Add((WordData)obj);
        DisableWordsOfList(InctiveWordsOnBoard);
    }

    ViewStates actualView;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;

        if (actualView == ViewStates.BoardView)
        {
            DisableWordsOfList(InctiveWordsOnBoard);
        }
        else if (actualView == ViewStates.TVView)
        {
            List<WordData> listAllWord = new List<WordData>();
            foreach(GameObject instance in WordsInstances)
            {
                listAllWord.Add(instance.GetComponent<NotebookWordInstance>().GetWord());
            }

            DisableWordsOfList(listAllWord);
        }
        else
        {
            List<WordData> Empylist = new List<WordData>();
            DisableWordsOfList(Empylist);
        }
    }

    void DisableWordsOfList(List<WordData> list)
    {
        foreach (GameObject instanceBTN in WordsInstances) 
        {
            instanceBTN.GetComponent<Button>().enabled = true;

            foreach (WordData word in list)
            {
                NotebookWordInstance Wordinstance = instanceBTN.GetComponent<NotebookWordInstance>();
                if(Wordinstance.GetWord() == word)
                {
                    instanceBTN.GetComponent<Button>().enabled = false;
                }
            }
        }
    }

    public void ClearUnderLine()
    {
        foreach (GameObject word in WordsInstances)
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
