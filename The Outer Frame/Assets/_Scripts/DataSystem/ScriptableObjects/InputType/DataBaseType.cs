using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "DB")]
public class DataBaseType : DataType,  IReseteableScriptableObject
{
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    public WordData AccessWord;
    [Header("Unlock Button")]
    public bool hasLockedBTN;
    public WordData wordToUnlock;
    public StateEnum UnlockState;
    [Header("Extra data")]
    public Sprite image;
    public string phoneNum;
    public string age;
    public string location;
    public string born;
    public string occupation;
    public string found;
    public string status;
    public string government;
    public string populatoin;
    public string area;
    public string zipcode;
    public string areacode;
    public string classification;
    public string serial;
    public string adress;
    public string ManufacturedBy;
    public string Email;
    public string ActiveLot;
    public string Consualties;
    public string DeclaredCasualties;
    public string Married;
    public string Size;
    public string Branch;
    public string MadeBy;
    public string Technology;
    public string Type;

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
    public void SetText(string _text) => text = _text;

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

        aux.Add(0, Type);
        aux.Add(1, status);
        aux.Add(2, MadeBy);
         aux.Add(3, Branch);
         aux.Add(4, Technology);
         aux.Add(5, serial);
         aux.Add(6, born);
        aux.Add(7,  age);
         aux.Add(8, Size); // Typo corregido
         aux.Add(9,occupation);
         aux.Add(10,phoneNum);
         aux.Add(11, Email);
         aux.Add(12, adress);
         aux.Add(13, location);
        aux.Add(14, found);
        aux.Add(15, Married);
        aux.Add(16, area);
        aux.Add(17, areacode);
        aux.Add(18, zipcode);
        aux.Add(19, populatoin);
        aux.Add(20, government);
        aux.Add(21, classification);
        aux.Add(22, ManufacturedBy);
        aux.Add(23, ActiveLot);
        aux.Add(24, Consualties);
        aux.Add(25, DeclaredCasualties);

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
