using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReporterAnimController : MonoBehaviour
{
    Animator anim;
    bool isAnimating;

    private void Start()
    {
        anim = GetComponent<Animator>();
        SetTriggerAnim(null, null);
    }

    public void SetTriggerAnim(Component sender, object obj)
    {
        anim.SetTrigger("newPaper");

        StartTalkLoop();
    }

    void StartTalkLoop()
    {
        StopAllCoroutines();
        anim.SetBool("isTalk", true);
        StartCoroutine(RandomizeAnimation());
        StartCoroutine(SetTalkTime(Random.Range(7, 15)));
    }

    IEnumerator RandomizeAnimation()
    {
        while(anim.GetBool("isTalk"))
        {
            yield return new WaitForSeconds(1.03f);
            anim.SetInteger("talkChoice", Random.Range(1, 12));
        }
    }

    IEnumerator SetTalkTime(float Time)
    {
        yield return new WaitForSeconds(Time);
        anim.SetBool("isTalk", false);

        anim.SetBool("isB", false);
        anim.SetBool("isC", false);
        anim.SetBool("isTalk", false);

        switch (Random.Range(1, 3))
        {
            case 1: anim.SetBool("isB", true); break;
            case 2: anim.SetBool("isC", true); break;
            case 3: anim.SetBool("isTalk", true); break;
        }

        Invoke("StartTalkLoop", 2);
    }



    public void AccelerateAnimator(Component sender, object obj)
    {
        float _speed = (float)obj;

        Debug.Log("speed: " + _speed);

        anim.speed = _speed;
    }
}
