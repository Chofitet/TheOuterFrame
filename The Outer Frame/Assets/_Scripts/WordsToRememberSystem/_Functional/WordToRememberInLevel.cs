using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class WordToRememberInLevel : MonoBehaviour
{
    WordData word;
    [SerializeField] TMP_Text textField;
    [SerializeField] List<GameObject> models = new List<GameObject>();
    public void Initialize(WordData _word)
    {
        word = _word;
        textField.text = _word.GetName();

        foreach (GameObject model in models) { model.SetActive(false); }
        models[word.GetMemberWordNumPaperModel()].SetActive(true);
    }

    public void GetOut(Component sender, object obj)
    {
        if ((WordData)obj != word) return;
       

        Vector3 finalPos = transform.position + new Vector3(0,-0.05f,0);
        transform.DOMove(finalPos, 0.5f).OnComplete(()=> Destroy(gameObject));
    }
}
