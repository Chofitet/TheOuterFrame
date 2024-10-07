using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Briefing2AppearAnimController : MonoBehaviour
{
    
    [SerializeField] Transform OnTvPos;
    [SerializeField] Transform RigthDossierPos;
    [SerializeField] Transform BriefingFinalPos;
    [SerializeField] GameEvent OnButtonElement;
    [SerializeField] GameEvent OnBriefing2Dossier;
    [SerializeField] GameEvent OnChangeFakeBiefing2;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();
    bool once;

    Sequence BriefingOnTVSequence;
    Sequence BriefingOnDossier;

    private void Start()
    {
        TimeManager.OnMinuteChange += CheckAppearConditions;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= CheckAppearConditions;
    }

    void CheckAppearConditions()
    {
        if (CheckForConditionals() && !once)
        {
            once = true;
            transform.GetChild(0).gameObject.SetActive(true);
            BriefingOnTV();
        }
    }

    public void BriefingOnTV()
    {
        BriefingOnTVSequence = DOTween.Sequence();

        BriefingOnTVSequence.Append(transform.DOMove(OnTvPos.position, 1f)).SetEase(Ease.OutCirc)
            .Join(transform.DORotate(OnTvPos.rotation.eulerAngles, 1f)).SetEase(Ease.OutQuad);
    }

    public void TakeBriefing(Component sender, object obj)
    {
        float StartAnimTime = 0;

       if(actualView != ViewStates.DossierView)
        {
            StartAnimTime = 0.5f;
            OnButtonElement?.Invoke(this, ViewStates.DossierView);
        }

        Invoke("TakeBriefingAnim", StartAnimTime);

        
    }

    void TakeBriefingAnim()
    {
        BriefingOnDossier = DOTween.Sequence();

        OnDisableInput?.Invoke(this, null);

        BriefingOnDossier.Append(transform.DOMove(RigthDossierPos.position, 0.3f).SetEase(Ease.InSine))
                .Join(transform.DORotate(RigthDossierPos.rotation.eulerAngles, 0.35f).SetEase(Ease.InSine))
                .AppendCallback(TriggerActionPlanDossier)
                .AppendInterval(0.5f)
                .Append(transform.DOMove(BriefingFinalPos.position,0.3f)).SetEase(Ease.InOutSine)
                .Join(transform.DORotate(BriefingFinalPos.rotation.eulerAngles,0.3f))
                .OnComplete(()=> {
                    OnChangeFakeBiefing2?.Invoke(this, null);
                    OnEnableInput?.Invoke(this, null);
                    Destroy(gameObject);
                });
    }

    void TriggerActionPlanDossier()
    {
        OnBriefing2Dossier?.Invoke(this, null);
    }

    ViewStates actualView;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }

    public bool CheckForConditionals()
    {

        if (Conditions == null) return true;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

        return true;
    }
}
