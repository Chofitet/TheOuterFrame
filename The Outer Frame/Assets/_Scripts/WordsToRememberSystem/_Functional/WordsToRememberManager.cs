using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordsToRememberManager : MonoBehaviour
{
    public static WordsToRememberManager Instance;

    [SerializeField] List<WordData> AllMemberWords = new List<WordData>();
    List<WordData> MemberWordsCandidates = new List<WordData>(); //Internal List of memeberWords filter by founded and they are candidates
    List<WordData> ChosenMemberWords = new List<WordData>(); // Internal List of Words Selected to remember
    [SerializeField] GameEvent OnAddWordsToRemember;
    [SerializeField] GameEvent OnShowRememberWordsInVoid;
    [SerializeField] GameEvent OnChangeScene;


    private void Awake()
    {
        // Evita duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Level1":
                CheckWordOnLevel1();
                break;

            case "RememberVoid":
                CheckWordsOnRememberVoid();
                break;
            case "LoseMenu":
                CheckEnterTheVoid();
                break;
        }
    }

    void CheckWordOnLevel1()
    {
        OnAddWordsToRemember?.Invoke(this, ChosenMemberWords);
    }

    //Construct the Candidate list of memember word entering the void
    void CheckWordsOnRememberVoid()
    {
        MemberWordsCandidates.Clear();

        foreach (WordData memberWords in AllMemberWords)
        {
            if(memberWords.GetIsFound()) MemberWordsCandidates.Add(memberWords);
        }

        OnShowRememberWordsInVoid?.Invoke(this, MemberWordsCandidates);
    }

    public void SetWordsToRemember(Component sender,object obj)
    {
        WordData memberWord = (WordData) obj;

        ChosenMemberWords.Add(memberWord);
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
