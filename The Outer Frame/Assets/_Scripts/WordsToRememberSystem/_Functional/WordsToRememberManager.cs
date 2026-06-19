using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class WordsToRememberManager : MonoBehaviour
{
    public static WordsToRememberManager instance;

    [SerializeField] bool DebugMode;
    [SerializeField] List<WordData> AllMemberWords = new List<WordData>();
    [SerializeField] int AvailableMemberWords;
    List<WordData> MemberWordsCandidates = new List<WordData>(); //Internal List of memeberWords filter by founded and they are candidates
    List<WordData> ChosenMemberWords = new List<WordData>(); // Internal List of Words Selected to remember
    [SerializeField] GameEvent OnAddWordsToRemember;
    [SerializeField] GameEvent OnShowRememberWordsInVoid;
    [SerializeField] GameEvent OnChangeScene;

    [SerializeField] DataDirectory directory;

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

        if (!DebugMode)
        {
            if (ChosenMemberWords.Count > 0)
            {
                OnAddWordsToRemember?.Invoke(this, ChosenMemberWords);
                foreach (WordData memberWord in ChosenMemberWords) memberWord.SetWordWasRemember();
            }
        }
        else
        {
            OnAddWordsToRemember?.Invoke(this, AllMemberWords);
        }

    }

    //Construct the Candidate list of memember word entering the void
    void CheckWordsOnRememberVoid()
    {
        MemberWordsCandidates.Clear();
        ChosenMemberWords.Clear();

        List<WordData> AuxMemberWordsCandidates = new List<WordData>();

        foreach (WordData memberWords in AllMemberWords)
        {
            if (memberWords.GetIsFound())
            {
                AuxMemberWordsCandidates.Add(memberWords);
            }
        }

        MemberWordsCandidates = AuxMemberWordsCandidates.OrderBy(_ => UnityEngine.Random.value).Take(AvailableMemberWords).ToList();

        DataPersistenceManager dataPersistenceManager = DataPersistenceManager.instance;

        if (dataPersistenceManager != null)
        {
            if (dataPersistenceManager.GetGameData().ContingencyContinue)
            {
                MemberWordsCandidates = GetMemberWordsByID(DataPersistenceManager.instance.GetGameData().LastMemberWordsID);
            }
        }

        foreach(WordData candidates in MemberWordsCandidates)
        {
            Debug.Log($"{candidates.name} was found");
        }

        OnShowRememberWordsInVoid?.Invoke(this, MemberWordsCandidates);
        if (DebugMode) OnShowRememberWordsInVoid?.Invoke(this, AllMemberWords);

        DataPersistenceManager.instance.ContingencyContinue(true, GetIDByMemberWords(MemberWordsCandidates));
    }

    List<WordData> GetMemberWordsByID(List<string> list)
    {
        List<WordData> MemberWordsByID = new List<WordData>();

        foreach(string id in list)
        {
            WordData memberWord = directory.GetById(id) as WordData;

            MemberWordsByID.Add(memberWord);
        }

        return MemberWordsByID;
    }

    List<string> GetIDByMemberWords(List<WordData> list)
    {
        List<string> idByMemberWords = new();

        foreach (WordData memberWord in list)
        {
            idByMemberWords.Add(memberWord.ID.ToString());
        }

        return idByMemberWords;
    }

    public void SetWordsToRemember(Component sender,object obj)
    {
        GameObject memberWord = (GameObject) obj;


        WordData word = memberWord.GetComponent<WordToRemember>().GetWord();

        if (!ChosenMemberWords.Contains(word))
        {
            ChosenMemberWords.Add(word);
        }
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
