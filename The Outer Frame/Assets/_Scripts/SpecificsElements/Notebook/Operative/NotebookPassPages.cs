using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NotebookPassPages : MonoBehaviour
{
    [SerializeField] GameObject LeftPageBTN;
    [SerializeField] GameObject RightPageBTN;
    [SerializeField] GameObject LeftModel;
    [SerializeField] GameObject RightModel;
    [SerializeField] GameObject RightModelShadow;

    [SerializeField] GameEvent OnChangePage;
    [SerializeField] GameEvent OnChangePageDirection;

    [SerializeField] List<NotebookPage> Pages;
    [SerializeField] GameObject backPageCorner;
    [SerializeField] GameObject upperBlendCorner;

    [SerializeField] float timeToPasspage;
    [SerializeField] float AwaitPassPageToStartProcess;


    int actualPage;

    private void Start()
    {
        LeftPageBTN.SetActive(false);
        RightPageBTN.SetActive(false);
        LeftModel.SetActive(false);
        RightModel.SetActive(false);
        RightModelShadow.SetActive(false);

        StartCoroutine(DelayTurnOfAllPagesExceptFor(0));
    }
    IEnumerator DelayTurnOfAllPagesExceptFor(int i)
    {
        yield return new WaitForSeconds(0.1f);
        TurnOfAllPagesExceptFor(i);
    }
    void TurnOfAllPagesExceptFor(int i)
    {
        foreach (NotebookPage page in Pages)
        {
            page.DisableEnable(false);
        }
        Pages[i].DisableEnable(true);
    }


    public void PassToASpecificPage(Component sender, object obj)
    {
        int requestPage = (int)obj;

        if (requestPage > actualPage) PassRightPage(null, null);

        if (requestPage < actualPage) PassLeftPage(null, null);

    }

    public void PassLeftPage(Component sender, object obj)
    {
        if (actualPage == 0) return;

        Pages[actualPage].DisableEnable(false);
        actualPage -= 1;
        Pages[actualPage].DisableEnable(true);

        RightPageBTN.SetActive(true);
        RightModel.SetActive(true);
        RightModelShadow.SetActive(true);
        if (actualPage == 0)
        {
            LeftModel.SetActive(false);
            LeftPageBTN.SetActive(false);
        }

        OnChangePage?.Invoke(this, actualPage);
        OnChangePageDirection?.Invoke(this, 1);
    }


    public void PassRightPage(Component sender, object obj)
    {
        if (actualPage + 1 == Pages.Count) return;

        Pages[actualPage].DisableEnable(false);
        actualPage += 1;
        Pages[actualPage].DisableEnable(true);

        LeftPageBTN.SetActive(true);
        LeftModel.SetActive(true);
        if (actualPage + 1 == Pages.Count)
        {
            RightModel.SetActive(false);
            RightModelShadow.SetActive(false);
            RightPageBTN.SetActive(false);
        }

        OnChangePage?.Invoke(this, actualPage);
        OnChangePageDirection?.Invoke(this, -1);

    }

    float passPageCountDown;

    public async Task RequestPage(int targetPage, float timeToFinishAnim)
    {
        if (targetPage == actualPage)
        {
            passPageCountDown += timeToFinishAnim;
            return;
        }

        while (passPageCountDown > 0)
        {
            await Task.Yield();
        }

        passPageCountDown += timeToFinishAnim;

        if (targetPage > actualPage)
        {
            PassRightPage(null, null);
        }
        else if (targetPage < actualPage)
        {
            PassLeftPage(null, null);
        }


        passPageCountDown += AwaitPassPageToStartProcess;
        await Task.Delay(Mathf.RoundToInt(AwaitPassPageToStartProcess * 1000));

        return;
    }

    private void Update()
    {
        if (passPageCountDown <= 0)
        {
            passPageCountDown = 0;
            return;
        }

        passPageCountDown -= Time.deltaTime;

    }

    [ContextMenu("EnableSecondPage")]
    public void EnableSecondPage()
    {
        PassRightPage(null, null);
    }

}
