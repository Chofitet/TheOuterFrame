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
    bool B1andB2Disanabled;
    [SerializeField] GameObject RunOutAPNote;
    [SerializeField] GameEvent OnWritingShakeDossier;
    [SerializeField] GameEvent OnShakeDossierSound;
    [SerializeField] GameEvent OnActionPlanDossier;
    [SerializeField] GameEvent OnIsInActionPlan;

    private void Start()
    {
        anim = GetComponent<Animator>();
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
    }

    public void ChangeToActionPlan(Component sender, object obj)
    {
        if (isInActionPlan) return;
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
        OnIsInActionPlan?.Invoke(sender, true);

    }

    public void ChangeToBrifing(Component sender, object obj)
    {
        if (B1andB2Disanabled) return;
        if (isInBrifing) return;

        //changetobrifing
        if (!isInDossierView) return;
        anim.SetTrigger("toBA");
        BrifingBtn.GetComponent<BoxCollider>().enabled = false;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;
        if(wasBrieffing2Taked) Brifing2BTN.GetComponent<BoxCollider>().enabled = true;
        OnIsInActionPlan?.Invoke(sender, false);

        isInBrifing = true;
        isInActionPlan = false;
        isInBrifing2 = false;
       

    }

    public void ChangeBrifing2(Component sender, object obj)
    {
        if (B1andB2Disanabled) return;
        if (isInBrifing2 || !wasBrieffing2Taked) return;

        //changetobrifing
        if (!isInDossierView) return;
        anim.SetTrigger("toBB");
        BrifingBtn.GetComponent<BoxCollider>().enabled = true;
        ActionPlanBtn.GetComponent<BoxCollider>().enabled = true;
        Brifing2BTN.GetComponent<BoxCollider>().enabled = false;
        OnIsInActionPlan?.Invoke(sender, false);

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

        if (currentState == ViewStates.DossierView || currentState == ViewStates.TutorialView) isInDossierView = true;
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

        foreach (GameObject go in InactiveInTutorial)
        {
            SetTutorialComponents(go, !isInTutorial);
        }

    }

    public void DesactiveElements(Component sender, object obj)
    {
        foreach (GameObject go in InactiveInTutorial)
        {
            go.SetActive(false);
        }
    }

    public void ActiveElements(Component sender, object obj)
    {
        foreach (GameObject go in InactiveInTutorial)
        {
            go.SetActive(true);
        }
    }

    public void ActiveFunctionalities(Component sender, object obj)
    {
        foreach (GameObject go in InactiveInTutorial)
        {
            SetTutorialComponents(go, true);
        }
    }

    private void SetTutorialComponents(GameObject go, bool active)
    {
        // Incluye el objeto de la lista y todos sus hijos
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        Collider[] colliders = go.GetComponentsInChildren<Collider>(true);
        Canvas[] canvases = go.GetComponentsInChildren<Canvas>(true);

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = active;
        }

        foreach (Collider collider in colliders)
        {
            collider.enabled = active;
        }

        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = active;
        }
    }

    ViewStates currentView;
    public void CheckVIew(Component sender, object obj)
    {
        currentView = (ViewStates)obj;

        if(isInActionPlan && currentView == ViewStates.DossierView) OnIsInActionPlan?.Invoke(sender, true);
        else OnIsInActionPlan?.Invoke(sender, false);
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
            Debug.Log("is in AP");
        }
    }
    
    public void OnSendLastReport(Component sender, object obj)
    {
        B1andB2Disanabled = true;
    }
}
