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

    public void InstanciateRedactedBlocks(TMP_Text textField, IReadOnlyList<RedactedBlockData> pre_proccess_PositioBlock = null)
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
            foreach (Transform child in textField.transform)
            {
                if (child.GetComponent<RedactedBlock>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
            RedactedBlockList.Clear();

            IReadOnlyList<RedactedBlockData> PositionBlock;

            Debug.LogWarning("Using PreProccess redacted Data");
            PositionBlock = pre_proccess_PositioBlock;

            if (pre_proccess_PositioBlock == null || pre_proccess_PositioBlock.Count == 0) return;
             

            foreach (RedactedBlockData w in PositionBlock)
            {

                GameObject auxObj = pool.GetFromPool(textField.transform);
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