using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName ="Word")]
public class WordData : ScriptableObject
{

    [Header("Action Plan Results")]

    [SerializeField] string dead;
    [SerializeField] string brainwashed;
    [SerializeField] string hacked;
    [SerializeField] string investigated;

    [Header("TV News")]

    [SerializeField] string TVdead;
    [SerializeField] string TVbrainwashed;
    [SerializeField] string TVhacked;
    [SerializeField] string TVinvestigated;

    [Header("BD Data")]

    [SerializeField] string BdData;

    public string GetActionPlanResult(Word.WordState _wordState) 
    {
        if(_wordState == Word.WordState.dead)
        {
            return dead;
        }
        else if(_wordState == Word.WordState.brainwashed)
        {
            return brainwashed;
        }
        else if (_wordState == Word.WordState.hacked)
        {
            return hacked;
        }
        else if (_wordState == Word.WordState.investigated)
        {
            return investigated;
        }
        return "";
    }

    public string GetTVNews(Word.WordState _wordState)
    {
        if (_wordState == Word.WordState.dead)
        {
            return TVdead;
        }
        else if (_wordState == Word.WordState.brainwashed)
        {
            return TVbrainwashed;
        }
        else if (_wordState == Word.WordState.hacked)
        {
            return TVhacked;
        }
        else if (_wordState == Word.WordState.investigated)
        {
            return TVinvestigated;
        }
        return "";
    }

    public string GetDataBD()
    {
        return BdData;
    }


}
