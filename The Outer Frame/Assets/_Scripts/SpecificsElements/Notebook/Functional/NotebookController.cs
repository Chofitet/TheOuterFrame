using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NotebookController : MonoBehaviour
{
    [SerializeField] List<NotebookPage> pages = new List<NotebookPage>();
    [SerializeField] WordData CabinWord;
    List<Transform> WordSpots = new List<Transform>();
    List<GameObject> WordsInstances = new List<GameObject>();
    List<WordData> InctiveWordsOnBoard = new List<WordData>();
    [SerializeField] NotebookProcessManager ProccessManager;
    [SerializeField] NotebookPassPages notebookPassPages;
    [SerializeField] GameEvent OnPendingWordsToPutInBoard;
    [SerializeField] float PassPageTime;
    float TimeLeftToEndPassingPage;
    int actualEmpyPage;
    int actualPage;
    int i = 0;
    bool once;
    bool isStarting = true;
    List<int> removedIndex = new List<int>();
    private void Start()
    {
        foreach(NotebookPage page in pages)
        {
            WordSpots.AddRange(page.GetWordSpots());
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
        if (i >= WordSpots.Count - 1) isOutOfRange = true;


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

        float delayTime = 0;
        if (actualView == ViewStates.TVView) delayTime = 0.3f;


        StartCoroutine(DelayInstanciate(delayTime, auxIndex, LastWordAdded));

        once = false;
    }

    IEnumerator DelayInstanciate(float delayTime, int auxIndex, WordData LastWordAdded)
    {
        
        yield return new WaitForSeconds(delayTime);
        float specialHeight = 0;
        if (WordSpots[auxIndex].GetComponent<CopyWordNotebookProperties>()) specialHeight = WordSpots[auxIndex].GetComponent<CopyWordNotebookProperties>().height;
        GameObject wordaux = pages[actualEmpyPage].InstanciateWord();
        wordaux.GetComponent<NotebookWordInstance>().GetButton().GetComponent<OnClickDownEvent>().onPointerDown.AddListener(ClearUnderLine);

        wordaux.GetComponent<NotebookWordInstance>().Initialization(LastWordAdded,actualEmpyPage, notebookPassPages, PassPageTime, actualView, isStarting, ProccessManager, specialHeight,TimeLeftToEndPassingPage);
        WordsInstances.Add(wordaux);
        if (pages[actualEmpyPage].GetIsFull()) actualEmpyPage++; 
    }


    int SearchIndexOfCrossWord(WordData newword)
    {
        List<GameObject> WordsToRemove = new List<GameObject>();
        int _index = 0;
        foreach (GameObject w in WordsInstances)
        {
            NotebookWordInstance script = w.GetComponent<NotebookWordInstance>();
            if (script.GetWord().GetInactiveStateSeen())
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
        WordData word = (WordData)obj;
       

       
        InctiveWordsOnBoard.Add((WordData)obj);
        InactiveWordInBoard((WordData)obj);

        checkWordsToPut();

    }

    void checkWordsToPut()
    {
        bool areWordsToPut = false;
         foreach (GameObject w in WordsInstances)
        {
            if (w.GetComponent<NotebookWordInstance>().GetButton().enabled) areWordsToPut = true;
        }

        if (areWordsToPut) OnPendingWordsToPutInBoard?.Invoke(this, true);
        else OnPendingWordsToPutInBoard?.Invoke(this, false);

    }

    ViewStates actualView;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
        List<WordData> listAllWord = new List<WordData>();
        foreach (GameObject instance in WordsInstances)
        {
            listAllWord.Add(instance.GetComponent<NotebookWordInstance>().GetWord());
        }


        if (actualView == ViewStates.BoardView)
        {
            DisableWordsOfList(InctiveWordsOnBoard,"Board",true,true);

            checkWordsToPut();
        }
        else if (actualView == ViewStates.TVView)
        {
            DisableWordsOfList(listAllWord);
        }
        else if(actualView == ViewStates.PinchofonoView)
        {
            DisableWordsOfList(listAllWord);
        }
        else if (actualView == ViewStates.OnTakeSomeInBoard)
        {
            DisableWordsOfList(InctiveWordsOnBoard, "Board", true, true);
        }
        else
        {
            List<WordData> Empylist = new List<WordData>();
            DisableWordsOfList(Empylist);
        }
    }


    void DisableWordsOfList(List<WordData> list, string material = "",bool changes = true, bool thickness = false)
    {
        List<NotebookWordInstance> activeWords = new List<NotebookWordInstance>();

        foreach (GameObject instanceBTN in WordsInstances) 
        {
            bool isActive = true;
            NotebookWordInstance Wordinstance = instanceBTN.GetComponent<NotebookWordInstance>();
            Wordinstance.TryActiveWord(true);
            Wordinstance.SetInactive(false);

            foreach (WordData word in list)
            {
                if(Wordinstance.GetWord() == word)
                {
                    isActive = false;
                    Wordinstance.TryActiveWord(false);
                    Wordinstance.SetInactive(true);
                }
            }
            if (isActive)
            {
                //palabra que sigue activa
                Wordinstance.ApplyMaterial(material);
                if(changes) Wordinstance.ApplyThicknessAnim(thickness);
            }
        }
    }

    void InactiveWordInBoard(WordData word)
    {
        foreach (GameObject instanceBTN in WordsInstances)
        {
            NotebookWordInstance script = instanceBTN.GetComponent<NotebookWordInstance>();

            if (script == null) return;

            if (script.GetWord() == word)
            {
                script.InactiveDirectly();
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
        foreach(Transform word in WordSpots) 
        {
            Destroy(word.GetChild(i).gameObject);
        }
    }

    public void SetActualPage(Component sender, object obj)
    {
        int newActualPage = (int)obj;

        if (actualPage != newActualPage)
        {
            isPassingPage = true;
            TimeLeftToEndPassingPage = PassPageTime;
        }
        actualPage = newActualPage;
    }
    bool isPassingPage;
    private void Update()
    {
        if(isPassingPage)
        {
            TimeLeftToEndPassingPage -= Time.deltaTime;

            if (TimeLeftToEndPassingPage <= 0) isPassingPage = false;
        }
    }
}
