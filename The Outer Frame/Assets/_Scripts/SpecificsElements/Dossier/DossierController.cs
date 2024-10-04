using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DossierController : MonoBehaviour
{
    Animator anim;
    bool isOpen;
    [SerializeField] GameObject BrifingBtn;
    [SerializeField] GameObject ActionPlanBtn;
    [SerializeField] GameObject Brifing2BTN;
    bool isInBrifing = true;
    bool IsTakingIdea;
    private bool isInDossierView;
    bool isInBrifing2;
    bool isInActionPlan;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeToActionPlan(Component sender, object obj)
    {
        if (isInActionPlan) return;

        //changetoActionPlan
        if (!isInDossierView && !IsTakingIdea) return;
        anim.SetTrigger("toAP");
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = false;
        BrifingBtn.GetComponent<BoxCollider>().enabled = true;
        Brifing2BTN.GetComponent<BoxCollider>().enabled = true;
        isInActionPlan = true;
        isInBrifing = false;
        isInBrifing2 = false;
        IsTakingIdea = false;
    }

    public void ChangeToBrifing(Component sender, object obj)
    {
        if (isInBrifing) return;

        //changetobrifing
        if (!isInDossierView) return;
        anim.SetTrigger("toBA");
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;
        Brifing2BTN.GetComponent<BoxCollider>().enabled = true;

        isInBrifing = true;
        isInActionPlan = false;
        isInBrifing2 = false;

    }

    public void ChangeBrifing2(Component sender, object obj)
    {
        if (isInBrifing2) return;

        //changetobrifing
        if (!isInDossierView) return;
        anim.SetTrigger("toBB");
        BrifingBtn.GetComponent<BoxCollider>().enabled = true;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;
        Brifing2BTN.GetComponent<BoxCollider>().enabled = false;

        isInBrifing2 = true;
        isInActionPlan = false;
        isInBrifing = false;
    }

    public void OpenActionPlan(Component sender, object obj)
    {
        isOpen = true;
    }
    public void StateCheck(Component sender, object obj)
    {
        if ((ViewStates)obj == ViewStates.DossierView) isInDossierView = true;
        else isInDossierView = false;
    }

    public void CloseActionPlan(Component sender, object obj)
    {
        if(isOpen)
        {
            anim.SetTrigger("close");
        }

        isOpen = false;
    }

    public void OnTakeIdea(Component sender, object obj)
    {
        IsTakingIdea = true;
    }

}
