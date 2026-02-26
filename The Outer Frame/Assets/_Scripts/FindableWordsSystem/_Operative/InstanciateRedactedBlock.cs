using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InstanciateRedactedBlock : MonoBehaviour
{
    [SerializeField] GameObject redactedBlockPrefab;
    List<GameObject> RedactedBlockList = new List<GameObject>();
    [SerializeField] ButtonsPoolController pool;
    [SerializeField] GameObject RedactedComnteiner;

    public static InstanciateRedactedBlock IRM { get; private set; }

    private void Awake()
    {
        if (IRM != null && IRM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            IRM = this;
        }
    }

    public void InstanciateRedactedBlocks(TMP_Text textField, IReadOnlyList<RedactedBlockData> pre_proccess_PositioBlock = null, bool CleanPool = false)
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

        try
        {
            if (CleanPool)
            {
                foreach (GameObject child in RedactedBlockList)
                {
                    pool.ReturnToPool(child);
                }
                RedactedBlockList.Clear();
            }

            IReadOnlyList<RedactedBlockData> PositionBlock;

            if (pre_proccess_PositioBlock != null && pre_proccess_PositioBlock.Count > 0)
            {
                Debug.LogWarning("Using PreProccess findable Data");
                PositionBlock = pre_proccess_PositioBlock;
            }
            else
            {
                Debug.LogWarning("Using runtime findable Data");
                PositionBlock = ProcessRedactedBlock.SearchForRedactedBlocks(textField, false);
            }
             
            foreach (RedactedBlockData w in PositionBlock)
            {
                // pool.GetFromPool(textField.transform.GetComponentInParent<Transform>().GetChild(1)); // toma el RedactedConteiner
                GameObject auxObj = pool.GetFromPool(RedactedComnteiner.transform); // toma el RedactedConteiner
                auxObj.transform.localPosition = w.position;
                auxObj.transform.localRotation = Quaternion.identity;
                auxObj.GetComponent<RedactedBlock>().Initialization(w.redactedText);
                RedactedBlockList.Add(auxObj);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instantiating findable words: " + ex.Message);
        }

        foreach (var obj in deactivatedParents)
        {
            obj.SetActive(false);
        }

    }


}