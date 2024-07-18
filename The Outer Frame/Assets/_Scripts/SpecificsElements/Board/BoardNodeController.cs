using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BoardNodeController : MonoBehaviour
{
    [SerializeField] WordData word;
    [SerializeField] Sprite Photo;
    bool isFound;
    ViewStates actualView;

    [HideInInspector][SerializeField] GameObject PosItObject;
    [HideInInspector][SerializeField] GameObject PhotoObject;
    [HideInInspector][SerializeField] GameObject Content;
    [HideInInspector][SerializeField] TMP_Text textPhotoField;
    [HideInInspector][SerializeField] TMP_Text textPosItField;
    [HideInInspector][SerializeField] Image PhotoImg;
    [HideInInspector][SerializeField] GameEvent OnBoardRefreshData;

    public bool GetIsFound() { return isFound; }

    private void Start()
    {
        if (!Application.isPlaying) return;
        Content.SetActive(false);
    }

    private void OnValidate()
    {
        if(!word)
        {
            Content.SetActive(false);
            return;
        }
        Content.SetActive(true);
        textPosItField.text = word.GetName();
        textPhotoField.text = word.GetName();
        PosItObject.SetActive(true);
        PhotoObject.SetActive(false);

        if (Photo)
        {
            PhotoObject.SetActive(true);
            PhotoImg.sprite = Photo;
        }
    }

    public void CheckToAppear(Component sender, object obj)
    {
        WordData NotebookWord = (WordData)obj;

        if (actualView != ViewStates.BoardView) return;

        if (NotebookWord == word)
        {
            Content.SetActive(true);
            isFound = true;
            OnBoardRefreshData?.Invoke(this, null);
        }
    }

    public void GetActualState(Component aender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        actualView = view;
    }


}
