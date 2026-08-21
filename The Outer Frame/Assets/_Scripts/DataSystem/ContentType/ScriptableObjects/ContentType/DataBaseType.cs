using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "DB")]
public class DataBaseType : ContentType,  IReseteableScriptableObject
{
    List<FindableWordData> hyperLinks = new();
    public IReadOnlyList<FindableWordData> HyperLinks => hyperLinks;
    public void SetHyperLinks(List<FindableWordData> _hyperlinks) { hyperLinks = _hyperlinks; }

    public List<RedactedBlockData> redactedBlocks = new();
    public IReadOnlyList<RedactedBlockData> RedactedBlocks => redactedBlocks;
    public void SetRedactedBlocks(List<RedactedBlockData> _RedactedBlocks) { redactedBlocks = _RedactedBlocks; }
    

    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] WordData AccessWord;
    [Header("Unlock Button")]
    [SerializeField] bool hasLockedBTN;
    [SerializeField] bool hasGooIdentificatorBTN;
    [SerializeField] WordData wordToUnlock;
    [SerializeField] StateEnum UnlockState;
    [SerializeField] bool NoSetFindableInDBTitle;
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
    [SerializeField] string adress;
    [SerializeField] string ManufacturedBy;
    [SerializeField] string Email;
    [SerializeField] string ActiveLot;
    [SerializeField] string Consualties;
    [SerializeField] string DeclaredCasualties;
    [SerializeField] string Married;
    [SerializeField] string Size;
    [SerializeField] string Branch;
    [SerializeField] string MadeBy;
    [SerializeField] string Technology;
    [SerializeField] string Type;


    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    [NonSerialized] bool isWordAccessFound;
    [NonSerialized] bool wasSearched;
    [NonSerialized] bool WasSetted;
    [NonSerialized] bool WasSeen;


    public override void ResetScriptableObject()
    {
        isWordAccessFound = false;
        wasSearched = false;
        WasSetted = false;
        WasSeen = false;
    }


    public WordData GetAccessWord() { return AccessWord; }
    override public string GetText() { return text; }
    public override string GetTextSecundary(){ return text; }
    public Sprite GetImage() { return image; }

    public string GetPhoneNum() { return phoneNum; }

    public void SetisWordAccessFound()
    {
        isWordAccessFound = true;
        MarkDirty();
    }

    public bool GetisWordAccessFound() { return isWordAccessFound; }

    public bool GetIsLocked() { return hasLockedBTN; }

    public bool GetIsGooIdentificator() { return hasGooIdentificatorBTN; }

    public WordData GetwordToUnlock() { return wordToUnlock; }

    public StateEnum GetUnlockState() { return UnlockState; }

    public void SetWasSearched()
    {
        wasSearched = true;
        MarkDirty();
    }
    public bool GetWasSearched() { return wasSearched; }

    public void SetWasSetted()
    {
        WasSetted = true;
        MarkDirty();
    }

    public void SetWasSeen()
    {
        WasSeen = true;
        MarkDirty();
    }
    public bool GetWasSeen() {  return WasSeen; }
    public bool GetNoSetFindableInDBTitle() { return NoSetFindableInDBTitle; }

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
