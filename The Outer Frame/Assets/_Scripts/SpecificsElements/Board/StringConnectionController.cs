using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringConnectionController : MonoBehaviour
{
    [SerializeField] BoardNodeController Node1;
    [SerializeField] BoardNodeController Node2;

    [HideInInspector][SerializeField] GameObject Content;

    private void Start()
    {
        Content.SetActive(false);
    }

    public void CheckConnection(Component sender, object obj)
    {
        if (Node1.GetIsFound() && Node2.GetIsFound()) Content.SetActive(true);
    }

}
