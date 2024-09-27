using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateWindowController : MonoBehaviour
{
    [SerializeField] TMP_Text Datatxt;
    public void UpdatePC(Component sender, object obj)
    {
        WordData word = (WordData)obj;

        Datatxt.text = word.GetForm_DatabaseNameVersion();
    }
}
