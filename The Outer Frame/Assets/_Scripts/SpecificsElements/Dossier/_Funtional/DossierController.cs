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
    [SerializeField] GameObject[] InactiveInTutorial;
    bool isInBrifing = true;
    bool IsTakingIdea;
    private bool isInDossierView;
    bool isInBrifing2;
    bool isInActionPlan;
    bool wasBrieffing2Taked;
    [SerializeField] GameObject RunOutAPNote;
    [SerializeField] GameEvent OnWritingShakeDossier;
    [SerializeField] GameEvent OnShakeDossierSound;
    [SerializeField] GameEvent OnActionPlanDossier;

    private void Start()
    {
        anim = GetComponent<Animator>();
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
    }

    public void ChangeToActionPlan(Component sender, object obj)
    {
        if (isInActionPlan) return;
        Debug.Log("sender AP: " + sender.gameObject.name);
        if (postItSeenInTutorial) RunOutAPNote.SetActive(true);
        //changetoActionPlan
        if (!isInDossierView && !IsTakingIdea) return;
        anim.SetTrigger("toAP");
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = false;
        BrifingBtn.GetComponent<BoxCollider>().enabled = true;
        if (wasBrieffing2Taked) Brifing2BTN.GetComponent<BoxCollider>().enabled = true;
        isInActionPlan = true;
        isInBrifing = false;
        isInBrifing2 = false;
        IsTakingIdea = false;
        onceInAP = false;


    }

    public void ChangeToBrifing(Component sender, object obj)
    {
        if (isInBrifing) return;

        //changetobrifing
        if (!isInDossierView) return;
        anim.SetTrigger("toBA");
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;
        if(wasBrieffing2Taked) Brifing2BTN.GetComponent<BoxCollider>().enabled = true;

        isInBrifing = true;
        isInActionPlan = false;
        isInBrifing2 = false;
       

    }

    public void ChangeBrifing2(Component sender, object obj)
    {
        
        if (isInBrifing2 || !wasBrieffing2Taked) return;

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

    bool postItSeenInTutorial;
    ViewStates currentState;
    public void StateCheck(Component sender, object obj)
    {
        if (currentState == ViewStates.TutorialView)
        {
            postItSeenInTutorial = true;
        }
        

        currentState = (ViewStates)obj;

        if (currentState == ViewStates.DossierView) isInDossierView = true;
        else isInDossierView = false;

        if (currentState == ViewStates.BoardView)
        {
            postItSeenInTutorial = false;
        }


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

    public void OnAppearBrieffing2(Component sender, object obj)
    {
        wasBrieffing2Taked = true;
    }


    bool isInTutorial;
    public void SetActionPlanInTutorial(Component sender, object obj)
    {
        
        if (obj == null) isInTutorial = true;
        else isInTutorial = (bool)obj;

        foreach(GameObject go in InactiveInTutorial)
        {
                go.SetActive(!isInTutorial);
        }
    }

    public void ActiveFunctionalities(Component sender, object obj)
    {
        foreach (GameObject go in InactiveInTutorial)
        {
            go.SetActive(true);
        }
    }

    ViewStates currentView;
    public void CheckVIew(Component sender, object obj)
    {
        currentView = (ViewStates)obj;
    }

    bool onceInAP; 
    public void ShakeDossier(Component sender, object obj)
    {
        if (isInTutorial && isInActionPlan && onceInAP && currentView == ViewStates.DossierView)
        {
            OnShakeDossierSound?.Invoke(this, null);
            OnWritingShakeDossier?.Invoke(this, 0.5f);
        }
        onceInAP = true;
    }

    public void TriggerOnActionPlanDossier(Component sender, object obj)
    {
        if (isInActionPlan) return;
        if (currentView == ViewStates.DossierView)
        {
            OnActionPlanDossier?.Invoke(this, null);
        }
    }
}
