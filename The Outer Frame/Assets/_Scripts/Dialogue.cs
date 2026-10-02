using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] string TextoNull;
    [SerializeField] string Texto1;
    [SerializeField] string Texto2;
    [SerializeField] string Texto3;

    public void AsignText(TMP_Text Dialogue, int NC)
    {
        if (NC == 0) Dialogue.text = TextoNull;
        else if (NC == 1) Dialogue.text = Texto1;
        else if (NC == 2) Dialogue.text = Texto2;
        else if (NC == 3) Dialogue.text = Texto3;
    }
    private void Update()
    {

    }
}
