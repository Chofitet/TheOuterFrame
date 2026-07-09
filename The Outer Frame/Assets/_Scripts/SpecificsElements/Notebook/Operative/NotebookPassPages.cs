using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookPassPages : MonoBehaviour
{
    [SerializeField] GameObject LeftPageBTN;
    [SerializeField] GameObject RightPageBTN;
    [SerializeField] GameEvent OnChangePage;

    [SerializeField] List<NotebookPage> Pages;

    int actualPage;

    private void Start()
    {
        LeftPageBTN.SetActive(false);
        RightPageBTN.SetActive(false);

        StartCoroutine(DelayTurnOfAllPagesExceptFor(0));
    }
    IEnumerator DelayTurnOfAllPagesExceptFor(int i)
    {
        yield return new WaitForSeconds(0.1f);
        TurnOfAllPagesExceptFor(i);
    }
    void TurnOfAllPagesExceptFor(int i)
    {
        foreach(NotebookPage page in Pages)
        {
            page.DisableEnable(false);
        }
        Pages[i].DisableEnable(true);
    }


    public void PassToASpecificPage(Component sender,object obj)
    {
        int requestPage = (int)obj;

        if (requestPage > actualPage) PassRightPage(null, null);

        if(requestPage < actualPage) PassLeftPage(null,null);

    }

    public void PassLeftPage(Component sender, object obj)
    {
        if (actualPage == 0) return;

        Pages[actualPage].DisableEnable(false);
        actualPage -= 1;
        Pages[actualPage].DisableEnable(true);

        RightPageBTN.SetActive(true);
        if(actualPage == 0) LeftPageBTN.SetActive(false);

        OnChangePage?.Invoke(this, actualPage);
    }

    public void PassRightPage(Component sender, object obj)
    {
        if (actualPage + 1 == Pages.Count) return;

        Pages[actualPage].DisableEnable(false);
        actualPage += 1;
        Pages[actualPage].DisableEnable(true);


        LeftPageBTN.SetActive(true);
        if (actualPage + 1 == Pages.Count) RightPageBTN.SetActive(false);
        OnChangePage?.Invoke(this, actualPage);
    }

}
