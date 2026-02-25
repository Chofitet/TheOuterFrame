using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HyperlinksManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonHyperLinkPrefab;
    [SerializeField] ButtonsPoolController pool;
    public static HyperlinksManager HLM { get; private set; }
    List<GameObject> HyperLinkBTNs = new List<GameObject>();

    [Header("Remove from Irrelevant")]
    [SerializeField] WordData Irrelevant;
    [SerializeField] List<DataRemoveIrrelevant> DataToUpdateIrrelevantDB = new List<DataRemoveIrrelevant>();

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += RemoveFindableAsToIrrelevant;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= RemoveFindableAsToIrrelevant;
    }

    private void Awake()
    {
        if (HLM != null && HLM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            HLM = this;
        }
    }
    public void InstanciateHyperLink(TMP_Text textField, FindableBtnType btnType, IReadOnlyList<FindableWordData> pre_proccess_PositioWords = null)
    {
        if (textField == null)
        {
            Debug.LogError("TextField is null");
            return;
        }

        List<GameObject> deactivatedParents = new List<GameObject>();
        Transform currentTransform = textField.transform;
        while (currentTransform != null)
        {
            if (!currentTransform.gameObject.activeSelf)
            {
                currentTransform.gameObject.SetActive(true);
                deactivatedParents.Add(currentTransform.gameObject);
            }
            currentTransform = currentTransform.parent;
        }

        RemoveFindableAsToIrrelevant();

        try
        {
            foreach (Transform child in textField.transform)
            {
                if (child.GetComponent<HyperlinksBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            IReadOnlyList<FindableWordData> PositionsWord = null;

            if (pre_proccess_PositioWords != null && pre_proccess_PositioWords.Count != 0)
            {
                Debug.LogWarning("Using PreProccess hyperlink Data");
                PositionsWord = pre_proccess_PositioWords;
            }
            else if(pre_proccess_PositioWords == null)
            {
                Debug.LogWarning("Using runtime hyperlink Data");
                PositionsWord = ProccessHyperLinks.SearchForHyperLinkWord(textField, Irrelevant);
            }
            else if(pre_proccess_PositioWords.Count == 0)
            {
                return;
            }   

            foreach (FindableWordData w in PositionsWord)
            {
               // Debug.Log("HyperLink found: " + w.GetWordData().GetName());

                GameObject auxObj = pool.GetFromPool(textField.transform);
                auxObj.transform.SetParent(textField.transform, false);
                auxObj.transform.localPosition = w.GetPosition();
                auxObj.transform.localRotation = Quaternion.identity;
                auxObj.GetComponent<HyperlinksBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GeisRepitedButton(),w.GetWordIndex());
                HyperLinkBTNs.Add(auxObj);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error instantiating findable words: " + ex.Message);
        }

        foreach (var obj in deactivatedParents)
        {
            obj.SetActive(false);
        }

    }

    void RemoveFindableAsToIrrelevant()
    {
        foreach (DataRemoveIrrelevant data in DataToUpdateIrrelevantDB)
        {
            if (CheckConditionals(data.Conditions))
            {
                Irrelevant.DeleteFoundAsWord(data.FindableAsToRemove);
            }
        }
    }

    public bool CheckConditionals(List<ConditionalClass> list)
    {
        //es el estado default
        if (list == null) return true;

        foreach (ConditionalClass conditional in list)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition(3);

            //Debug.Log("last compete conditional of " + auxInterface + " is " + auxInterface.GetLastCompletedConditional());

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
[Serializable]
public class DataRemoveIrrelevant
{
    public string FindableAsToRemove;
    public List<ConditionalClass> Conditions;
}
