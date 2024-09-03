using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DossierController : MonoBehaviour
{
    Animator anim;
    bool isOpen;
    [SerializeField] GameObject BrifingBtn;
    [SerializeField] GameObject ActionPlanBtn;
    bool isInBrifing = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeToActionPlan(Component sender, object obj)
    {
        //changetobrifing
        if (isInBrifing) return;
        anim.SetTrigger("showBriefing");
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;

        anim.SetBool("isBriefingHidden", false);
        isInBrifing = true;

    }

    public void ChangeToBrifing(Component sender, object obj)
    {
        //changetoActionPlan
        if (!isInBrifing) return;
        anim.SetTrigger("hideBriefing");
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = false;
        BrifingBtn.GetComponent<BoxCollider>().enabled = true;

        anim.SetBool("isBriefingHidden", true);
        isInBrifing = false;
    }

    public void OpenActionPlan(Component sender, object obj)
    {
        isOpen = true;
    }

    public void CloseActionPlan(Component sender, object obj)
    {
        if(isOpen)
        {
            anim.SetTrigger("close");
        }

        isOpen = false;
    }

}
