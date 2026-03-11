using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnPCSearchWord;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] TMP_Text WikiTitleSearchedWord;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] GameEvent OnWordAccessScreen;
    [SerializeField] GameEvent OnKeyBoardSound;
    [SerializeField] List<GameObject> PanelsAppearsOnSearch = new List<GameObject>();
    [SerializeField] WordData IrrelevantDB;
    [SerializeField] GameObject BtnBackToLastEntry;
    [SerializeField] GameEvent OnLogFilterType;
    [SerializeField] GameEvent OnSearchEmptyDB;
    
    GameEvent LastWindow;
    bool isWaitingAWord;
    bool inWordAccessWindow;


    [SerializeField] WordData Vessel;
    [SerializeField] WordData A15;

    WordData _LastSearchedWord;
    WordData word;
    bool isInPCView;
    TypingAnimText textAnim;

    private void Start()
    {
        isWaitingAWord = true;
        textAnim = SearchBar.GetComponent<TypingAnimText>();
        textAnim.SetCharacterPerSecond(2);
        foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(false);
        StartCoroutine(IdleSearchBarAnim());
        LastWindow = OnWikiWindow;
    }

    //OnSelectedWordInNotebook
    public void CompleteSeachBar(Component sender, object obj)
    {
        if (!isInPCView) return;
        if (inWordAccessWindow) return;
        WordData _word = (WordData)obj;
        
        word = _word;
        SearchBar.text = " " + word.GetName().Replace("?", "").Replace("!", "");
        StopCoroutine(IdleSearchBarAnim());
        textAnim.SetCharacterPerSecond();
        isWaitingAWord = false;
        SearchBar.GetComponent<TypingAnimText>().AnimateTyping();
        OnKeyBoardSound?.Invoke(this, null);

    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
    }

    //OnChangeView
    public void GetActualView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.PCView)
        {
            isInPCView = true;
        }
        else isInPCView = false;

        if(word)
        {
            word.GetDB().SetWasSearched();
        }
    }

    public void SearchHyperLink(Component sender, object obj)
    {
        if (!isInPCView) return;
        WordData _word = (WordData)obj;
        word = _word;
        SearchWordInWiki(_LastSearchedWord);
    }

    public void ForceUpdateWindow(Component sender, object obj)
    {
        UpdateWK(_LastSearchedWord, true);
    }

    public void UpdateDataBase(Component sender, object obj)
    {
        word = (WordData)obj;
        UpdateWK(word, false);
    }

    void UpdateWK(WordData word, bool forceUpdate)
    {
        if (OnWikiWindow != LastWindow && !forceUpdate) return;

        if (word != _LastSearchedWord) return;

        SearchWordInWiki();
        OnWikiWindow?.Invoke(this, null);
    }

    public void BackLastEntry(Component sender, object obj)
    {
        if (!isInPCView) return;
        WordData _word = (WordData)obj;
        word = _word;
        SearchWordInWiki();
    }

    public void SearchBTN()
    {
        SearchWordInWiki();
    }

    public void SearchBarBTN()
    {
        //triggers solo de la barra, tiembla si no hay palabra
        if (!word)
        {
            SearchBar.text = " |";
            if (isInPCView) OnShakeNotebook?.Invoke(this, null);
        }
    }
    public void OnSearWordInWiki(Component sender, object obj)
    {
        if (sender.GetComponent<ReportsInDetailController>() != null || sender.GetComponent<TranscriptsInDetailController>()) LastWindow = OnWikiWindow;
            word = (WordData)obj;
            SearchWordInWiki((WordData)obj);
    }

    public void SearchWordInWiki(WordData LastSearchedWord = null)
    {
        if(LastWindow != OnWikiWindow)
        {
            //OnLogWindow

            if (!word)
            {
                SearchBar.text = " |";
                if (isInPCView) OnShakeNotebook?.Invoke(this, null);
                OnSearchEmptyDB?.Invoke(this, null);
                return;
            }

            FilterAll(word);
            isWaitingAWord = true;
            StopAllCoroutines();
            StartCoroutine(IdleSearchBarAnim());
            word = null;
            return;
        }

        OnWikiWindow?.Invoke(this, null);
        //BtnBackToLastEntry.SetActive(false);
        if (!word)
        {
            SearchBar.text = " |";
            if(isInPCView) OnShakeNotebook?.Invoke(this, null);
            WikiTitleSearchedWord.text = "";
            foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(false);
            OnPCSearchWord?.Invoke(this, word);
            return;
        }

        if (word == Vessel) word = A15;
        if(word == IrrelevantDB) foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(false);
        else foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(true);
        DataBaseType db = WordsManager.WM.RequestBDWikiData(word);

        StopAllCoroutines();

        /*if (LastSearchedWord != null)
        {
            StartCoroutine(DelayBTNBackLatEntryAppear());
            BtnBackToLastEntry.GetComponent<BackToLastEntryBTNController>().SetWordToBack(LastSearchedWord);
        }*/

        if (db == null) return;
        if (db.GetAccessWord() && !db.GetisWordAccessFound())
        {
            OnWordAccessScreen?.Invoke(this, word);
            inWordAccessWindow = true;
            return;
        }

        if (db.GetAccessWord() && !db.GetisWordAccessFound())
        {
            OnWordAccessScreen?.Invoke(this, word);
            inWordAccessWindow = true;
            return;
        }

        string TitleName = WordsManager.WM.FindWordWithPhoneNum(word).GetDatabaseNameVersion();

        WikiTitleSearchedWord.text = TitleName;
        
        if (word.GetDB() != null && !word.GetIsFound() && !db.GetNoSetFindableInDBTitle())
        {
            WikiTitleSearchedWord.text = "<link>" + TitleName + "</link>";
            WikiTitleSearchedWord.ForceMeshUpdate();
            
        }
        FindableWordsManager.FWM.InstanciateFindableWord(WikiTitleSearchedWord, FindableBtnType.FindableBTN,null, true);
        isWaitingAWord = true;
       
        StartCoroutine(IdleSearchBarAnim());
        
        OnPCSearchWord?.Invoke(this, word);

        _LastSearchedWord = word;
        word = null;
    }

    public void SetWikiTitleSearchedWord(Component sender, object obj)
    {
        if (obj == null) return;

        WordData searchedWord = (WordData)obj;
        string newWord = WordsManager.WM.FindWordWithPhoneNum(searchedWord).GetDatabaseNameVersion();
        
        string existingText = WikiTitleSearchedWord.text;

        // Verificar si el texto existente contiene etiquetas <material>
        if (existingText.Contains("<material=") && existingText.Contains("</material>"))
        {
            // Si tiene etiquetas <material>, no sobrescribe el texto
            return;
        }

        // Si no tiene etiquetas <material>, actualiza el texto
        WikiTitleSearchedWord.text = newWord;
        
    }

    public void CloseWordAccessWindow(Component sender, object obj)
    {
        inWordAccessWindow = false;
    }

    public void ChangeWindow(GameEvent gameEvent)
    {
        gameEvent?.Invoke(this, null);
        LastWindow = gameEvent;
    }
    public void SetLastWindow(Component sender,object obj)
    {
        LastWindow = (GameEvent)obj;
    }
    public void FilterAll(WordData specificTag = null)
    {
        WordData WordToFilter = _LastSearchedWord;

        if (specificTag) WordToFilter = specificTag;
        if (specificTag == IrrelevantDB) WordToFilter = _LastSearchedWord;
        OnLogFilterType?.Invoke(this, new SearchLogData(WordToFilter, LogFilterType.report));
    }

    public void SetInWordAccessWindow(Component sender, object obj)
    {
        inWordAccessWindow = (bool)obj;
    }

    IEnumerator IdleSearchBarAnim()
    {
        SearchBar.text = " |";
        textAnim.SetCharacterPerSecond(2);

        while (isWaitingAWord)
        {
            textAnim.AnimateTyping();
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DelayBTNBackLatEntryAppear()
    {
        yield return new WaitForSeconds(0.4f);
        BtnBackToLastEntry.SetActive(true);
    }

   

}
