using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DossierUpForAddAPController : MonoBehaviour
{
    [SerializeField] GameObject Dossier;
    [SerializeField] GameObject Notebook;
    [SerializeField] Transform DossierPivot;
    [SerializeField] Transform NotebookPivot;
    [SerializeField] Transform position;
    [SerializeField] Transform initPos;
    Sequence AnimDossierUpSequence;
    Transform originalDossierParent;
    Transform originalNotebookParent;

    private void OnEnable()
    {
        originalDossierParent = Dossier.transform.parent;
        originalNotebookParent = Notebook.transform.parent;
    }


    public void MadeAnimUpDossier(Component sender,object obj)
    {
        if (AnimDossierUpSequence.IsActive() || AnimDossierUpSequence != null) AnimDossierUpSequence.Kill();

        Dossier.transform.SetParent(transform, true);
        Notebook.transform.SetParent(transform, true);

        transform.position = position.position;
        transform.rotation = position.rotation;
        Notebook.transform.position = NotebookPivot.position;
        Notebook.transform.rotation = NotebookPivot.rotation;
        Dossier.transform.position = DossierPivot.position;
        Dossier.transform.rotation = DossierPivot.rotation;
        transform.position = initPos.position;
        transform.rotation = initPos.rotation;

        AnimDossierUpSequence = DOTween.Sequence();
        
        
        


        AnimDossierUpSequence
            .Append(transform.DORotate(position.rotation.eulerAngles, 1.2f).SetEase(Ease.OutCubic))
            .OnComplete(() =>
            {
                Dossier.transform.SetParent(originalDossierParent);
                Notebook.transform.SetParent(originalNotebookParent);
            });

    }
}
