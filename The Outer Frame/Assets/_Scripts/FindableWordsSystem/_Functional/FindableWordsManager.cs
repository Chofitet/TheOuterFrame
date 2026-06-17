using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindableWordsManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonFindableWordPrefab;
    [SerializeField] ButtonsPoolController pool;
    List<GameObject> FindableWordsBTNs = new List<GameObject>();
    [SerializeField] GameEvent OnFindableWordInstance;
    [SerializeField] WordData Irrelevant;
    [SerializeField] WordData LastWord;
    bool isInLastWord;
    public static FindableWordsManager FWM { get; private set; }

    private void Awake()
    {
        if (FWM != null && FWM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            FWM = this;
        }
    }

    public void InstanciateFindableWord(TMP_Text textField, FindableBtnType btnType, IReadOnlyList<FindableWordData> pre_proccess_PositioWords = null, bool _comesFromDBTitle = false, bool _comesFromNewEmergency =false, bool _comesFromReport = false)
    {
        // btnType para cuando quiera refactorizar este script para que funcione con links tambien

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

        try
        {
            foreach (Transform child in textField.transform)
            {
                if (child.GetComponent<FindableWordBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            IReadOnlyList<FindableWordData> PositionsWord = null;

            if (pre_proccess_PositioWords != null && pre_proccess_PositioWords.Count > 0)
            {
                Debug.LogWarning("Using PreProccess FINDABLEWORD Data");
                PositionsWord = pre_proccess_PositioWords;
            }
            else
            {
                Debug.LogWarning("Using runtime FINDABLEWORD Data");
                PositionsWord = ProccessFindableWord.SearchForFindableWord(textField, Irrelevant);
            }

            foreach (FindableWordData w in PositionsWord)
            {
                if (isInLastWord && w.GetWordData() != LastWord) continue;
                if(_comesFromReport) w.GetWordData().SetAppearOnReport();
                if (w.GetWordData().GetIsFound()) continue;
                if (w.GetWordData().GetInactiveState()) continue;
                GameObject auxObj = pool.GetFromPool(textField.transform);
                auxObj.transform.SetParent(textField.transform, false);
                auxObj.transform.localPosition = w.GetPosition();
                auxObj.transform.localRotation = Quaternion.identity;
                auxObj.name = "FindableBTN_" + w.GetWordData().GetName();
                auxObj.GetComponent<FindableWordBTNController>().enabled = true;
                auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GeisRepitedButton(), _comesFromDBTitle, _comesFromNewEmergency);
                FindableWordsBTNs.Add(auxObj);
                OnFindableWordInstance?.Invoke(this, auxObj);
               // auxObj.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(auxObj));
            }
        } catch (Exception ex)
        {
            Debug.LogError("Error instantiating findable words: " + ex.Message);
        }

        foreach (var obj in deactivatedParents)
        {
            obj.SetActive(false);
        }

    }
    

   /* void OnButtonClick(GameObject obj)
    {
        DeleteBtnAlreadyFound(obj.GetComponent<FindableWordBTNController>().Getword());
    }

    void DeleteBtnAlreadyFound(WordData newWord)
    {
        foreach(GameObject btn in FindableWordsBTNs)
        {
            if (btn.GetComponent<FindableWordBTNController>().Getword() == newWord)
            {
                Destroy(btn);
            }
        }
    }*/


    int index;
    private void Update()
    {
        FindableWordsBTNs.RemoveAll(s => s == null);

        foreach (GameObject fw in FindableWordsBTNs)
        {
            WordData word = fw.GetComponent<FindableWordBTNController>().Getword();
            if (word.GetIsFound())
            {
                fw.GetComponent<FindableWordBTNController>().ApplyShaderMaterial("Grey");
                Destroy(fw);
            }
            else if(word.GetIsPhoneNumberFound() && word.GetIsAPhoneNumber())
            {
                fw.GetComponent<FindableWordBTNController>().ApplyShaderMaterial("Grey");
                Destroy(fw);
            }
            else if(word.GetInactiveState())
            {
                fw.GetComponent<FindableWordBTNController>().ApplyShaderMaterial("Grey");
                Destroy(fw);
            }
        }
   }
   
    public void SetDisableGrabWord(Component sender, object obj)
    {
        isInLastWord = true;
    }

    public void RemoveAllFindableWords(Component sender, object obj)
    {
        foreach (GameObject fw in FindableWordsBTNs) Destroy(fw);

    }
}
public enum FindableBtnType
{
    FindableBTN,
    HyperLink
}

