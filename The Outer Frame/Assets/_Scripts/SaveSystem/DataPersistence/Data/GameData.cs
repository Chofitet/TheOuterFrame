using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public bool isInTutorial;
    public bool ContingencyContinue;
    public List<string> LastMemberWordsID;

    public float MusicVolume;
    public float SoundVolume;
    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        isInTutorial = true;
        LastMemberWordsID = new List<string>();
        ContingencyContinue = false;
        MusicVolume = 1;
        SoundVolume = 0;
    }

}
