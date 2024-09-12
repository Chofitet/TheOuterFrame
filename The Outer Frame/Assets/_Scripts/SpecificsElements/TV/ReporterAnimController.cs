using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReporterAnimController : MonoBehaviour
{
    Animator anim;
    bool isAnimating;

    [SerializeField] int _minClips = 2;
    [SerializeField] int _maxClips = 11;
    
    /// <summary>
    /// organizar para que no repitan emociones
    /// </summary>

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

        anim.SetBool("isNormal", false);
        anim.SetBool("isSerious", false);
        anim.SetBool("isQuestion", false);
        anim.SetBool("isSurprise", false);

        switch (Random.Range(1, 5))
        {
            case 1: anim.SetBool("isNormal", true); break;
            case 2: anim.SetBool("isSerious", true); break;
            case 3: anim.SetBool("isQuestion", true); break;
            case 4: anim.SetBool("isSurprise", true); break;
        }

        StartCoroutine(RandomizeAnimation());
        StartCoroutine(SetTalkTime(Random.Range(_minClips, _maxClips+1)));
    }

    IEnumerator RandomizeAnimation()
    {        
        while(anim.GetBool("isTalk"))
        {
            yield return new WaitForSeconds(5.05f);
            anim.SetInteger("talkChoice", Random.Range(1, 11));
        }
    }

    IEnumerator SetTalkTime(int Repetitions)
    {
        while(Repetitions > 0)
        {
            //Debug.Log("Repetitions: " + Repetitions);
            yield return new WaitForSeconds(5.05f);
            Repetitions--;
        }

        anim.SetBool("isTalk", false);

        anim.SetBool("isA", false);
        anim.SetBool("isB", false);
        anim.SetBool("isC", false);
        anim.SetBool("isD", false);

        switch (Random.Range(1, 5))
        {
            case 1: anim.SetBool("isA", true); break;
            case 2: anim.SetBool("isB", true); break;
            case 3: anim.SetBool("isC", true); break;
            case 4: anim.SetBool("isD", true); break;
        }

        Invoke("StartTalkLoop", 2.1f);
    }



    public void AccelerateAnimator(Component sender, object obj)
    {
        float _speed = (float)obj;

        Debug.Log("speed: " + _speed);

        anim.speed = _speed;
    }
}
