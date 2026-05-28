using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordToRememberInstantiator : MonoBehaviour
{
    [SerializeField] GameObject WordToRememberPrefab;
    [SerializeField] GameObject Anchors;

    private void Start()
    {
        
    } 

    public void ShowRememberWordsInVoid(Component sender,object obj)
    { 
        List<WordData> WordsToRemember = (List<WordData>)obj;

        Transform[] anchorTransforms = Anchors.GetComponentsInChildren<Transform>();

        for (int i = 0; i < WordsToRemember.Count; i++)
        {
            // +1 porque el elemento 0 es el propio objeto "anchors"
            Transform anchor = anchorTransforms[i + 1];

            GameObject wordToRemember = Instantiate(
                WordToRememberPrefab,
                anchor.position,
                anchor.rotation,
                anchor
            );

            wordToRemember.GetComponent<WordToRemember>()
                .Initialize(WordsToRemember[i]);
        }
    }
}
