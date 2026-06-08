using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordsToRememberManager : MonoBehaviour
{
    public static WordsToRememberManager instance;

    [SerializeField] bool DebugMode;
    [SerializeField] List<WordData> AllMemberWords = new List<WordData>();
    List<WordData> MemberWordsCandidates = new List<WordData>(); //Internal List of memeberWords filter by founded and they are candidates
    List<WordData> ChosenMemberWords = new List<WordData>(); // Internal List of Words Selected to remember
    [SerializeField] GameEvent OnAddWordsToRemember;
    [SerializeField] GameEvent OnShowRememberWordsInVoid;
    [SerializeField] GameEvent OnChangeScene;

    bool isInRememberVoid;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInRememberVoid = false;
        switch (scene.name)
        {
            case "Level1":
                CheckWordOnLevel1();
                break;
            case "RememberVoid":
                isInRememberVoid = true;
                CheckWordsOnRememberVoid();
                break;
            case "LoseMenu":
                CheckEnterTheVoid();
                break;
            case "MainMenu":
                MemberWordsCandidates.Clear();
                ChosenMemberWords.Clear();
                break;

        }
    }

    void CheckWordOnLevel1()
    {
        DataPersistenceManager.instance.ContingencyContinue(false, null);

        if(!DebugMode) OnAddWordsToRemember?.Invoke(this, ChosenMemberWords);
        else OnAddWordsToRemember?.Invoke(this, AllMemberWords);

    }

    //Construct the Candidate list of memember word entering the void
    void CheckWordsOnRememberVoid()
    {
        MemberWordsCandidates.Clear();
        ChosenMemberWords.Clear();

        foreach (WordData memberWords in AllMemberWords)
        {
            if(memberWords.GetIsFound()) MemberWordsCandidates.Add(memberWords);
        }

        DataPersistenceManager dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager != null)
        {
            if (dataPersistenceManager.GetGameData().ContingencyContinue)
            {
                MemberWordsCandidates = new List<WordData>(DataPersistenceManager.instance.GetGameData().LastMemberWords);
            }
        }

        OnShowRememberWordsInVoid?.Invoke(this, MemberWordsCandidates);
        if (DebugMode) OnShowRememberWordsInVoid?.Invoke(this, AllMemberWords);

        DataPersistenceManager.instance.ContingencyContinue(true,MemberWordsCandidates);

    }

    public void SetWordsToRemember(Component sender,object obj)
    {
        GameObject memberWord = (GameObject) obj;


        WordData word = memberWord.GetComponent<WordToRemember>().GetWord();

        if (!ChosenMemberWords.Contains(word))ChosenMemberWords.Add(word);
    }

    public void UnsetWordsToRemember(Component sender, object obj)
    {
        GameObject memberWord = (GameObject)obj;

        ChosenMemberWords.Remove(memberWord.GetComponent<WordToRemember>().GetWord());
    }

    void CheckEnterTheVoid()
    {
        foreach (WordData memberWords in AllMemberWords)
        {
            if (memberWords.GetIsFound())
            {
                OnChangeScene?.Invoke(this, "RememberVoid");
                return;
            }
        }
        OnChangeScene?.Invoke(this, "LoadingScreen");
    }

}
