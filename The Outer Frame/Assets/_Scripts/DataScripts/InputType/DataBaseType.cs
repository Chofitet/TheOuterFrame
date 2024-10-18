using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "DB")]
public class DataBaseType : ScriptableObject,  IReseteableScriptableObject
{
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] WordData AccessWord;
    [Header("Unlock Button")]
    [SerializeField] bool hasLockedBTN;
    [SerializeField] WordData wordToUnlock;
    [SerializeField] StateEnum UnlockState;
    [Header("Extra data")]
    [SerializeField] Sprite image;
    [SerializeField] string phoneNum;
    [SerializeField] string age;
    [SerializeField] string location;
    [SerializeField] string born;
    [SerializeField] string occupation;
    [SerializeField] string found;
    [SerializeField] string status;
    [SerializeField] string government;
    [SerializeField] string populatoin;
    [SerializeField] string area;
    [SerializeField] string zipcode;
    [SerializeField] string areacode;
    [SerializeField] string classification;
    [SerializeField] string serial;

    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    [NonSerialized] bool isWordAccessFound;
    [NonSerialized] bool wasSearched;
    [NonSerialized] bool WasSetted;

    private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }

    public void ResetScriptableObject()
    {
        isWordAccessFound = false;
        wasSearched = false;
        WasSetted = false;
    }


    public WordData GetAccessWord() { return AccessWord; }
    public string GetText() { return text; }

    public Sprite GetImage() { return image; }

    public string GetPhoneNum() { return phoneNum; }

    public void SetisWordAccessFound() => isWordAccessFound = true;

    public bool GetisWordAccessFound() { return isWordAccessFound; }

    public bool GetIsLocked() { return hasLockedBTN; }

    public WordData GetwordToUnlock() { return wordToUnlock; }

    public StateEnum GetUnlockState() { return UnlockState; }

    public void SetWasSearched() => wasSearched = true;
    public bool GetWasSearched() { return wasSearched; }

    public void SetWasSetted() => WasSetted = true;

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }

    public Dictionary<int,string> GetDataFieldsInfo()
    {
        Dictionary<int,string> aux = new Dictionary<int,string>();

        aux.Add(0,phoneNum);
        aux.Add(1,age);
        aux.Add(2,location);
         aux.Add(3,born);
         aux.Add(4,occupation);
         aux.Add(5,found);
         aux.Add(6,status);
        aux.Add(7,government);
         aux.Add(8,populatoin); // Typo corregido
         aux.Add(9,area);
         aux.Add(10,zipcode);
         aux.Add(11,areacode);
         aux.Add(12,classification);
         aux.Add(13,serial);

        Debug.Log(aux);

        return aux;
    }

    public bool CheckConditionals()
    {
        if (Conditions == null) return true;
        if (WasSetted) return false;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition(3);


            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

            return true;
      
    }

    
}
