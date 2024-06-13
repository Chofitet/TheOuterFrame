using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    //Sistema encargado de solicitar requests a Words específicas para que devuelvan un input
    [SerializeField] string testRequest;
    [SerializeField] Word.WordState estado;
    public static WordsManager WM { get; private set; }
    public event Action<string> OnChangeStateOfWord;

    private void Awake()
    {

        if (WM != null && WM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            WM = this;
        }

    }

    private void Start()
    {
       // RequestInput();
    }

    private Dictionary<string, Word> wordsDic = new Dictionary<string, Word>();

    public void RegisterWord(string id, Word word)
    {
        if (!wordsDic.ContainsKey(id))
        {
            wordsDic.Add(id, word);
        }
    }

    public void RequestInput(string _word)
    {
        wordsDic[_word].GetActionPlanResult();
       
            // pedir input segun el ultimo estado que tenga el objeto (sacar que se tenga que pasar estado) necesita una forma de aclarar si es para TV, BASE DE DATOS, RESULTS...
    }

    public string RequestNew(string _word)
    {
        return wordsDic[_word].GetLastTVNew();
    }

    public void RequestChangeState(string _word, Word.WordState WordState)
    {
        wordsDic[_word].ChangeState(WordState);
        OnChangeStateOfWord?.Invoke(_word);
    }

    public bool CheckIfStateWasDone(string _word, Word.WordState WordState)
    {
        return wordsDic[_word].CheckIfStateWasDone(WordState);
    }
     

    public Word.WordState ConvertStringToState(string _state)
    {
        switch(_state)
        {
            case "brainwashed":
                return Word.WordState.brainwashed;
            case "dead":
                return Word.WordState.dead;
            case "hacked":
                return Word.WordState.hacked;
            case "investigated":
                return Word.WordState.investigated;
        }
        Debug.LogWarning("State not setted: The string value don't match with any state");
        return Word.WordState.none;
    }


}
