using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] GameObject SlotPrefab;


    public void SetActionInCourse(string _word, string state)
    {
        if (WordsManager.WM.CheckIfStateWasDone(_word, WordsManager.WM.ConvertStringToState(state)))
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().ActionWasDone();
            slot.transform.SetParent(transform, false);
        }
        else
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().initParameters(_word, state);
            WordsManager.WM.RequestChangeState(_word, WordsManager.WM.ConvertStringToState(state));
            slot.transform.SetParent(transform, false);
            
        }



        //esta linea se debe concretar recien pasado el tiempo del progreso
       
    }

}
