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
        // string triggerName = (string)obj;

        StopCoroutine(RandomizeAnimation());
        isAnimating = false;

        anim.SetTrigger("newPaper");
        if (!isAnimating)
        {
            StartCoroutine(RandomizeAnimation());
        }
    }

    IEnumerator RandomizeAnimation()
    {
        isAnimating = true;
        anim.SetBool("isB", true);
        yield return new WaitForSeconds(6f);

        while (true) 
        {
            float waitTime = Random.Range(8, 10);
            yield return new WaitForSeconds(waitTime);

            anim.SetTrigger("random");

            anim.SetBool("isB", false);
            anim.SetBool("isC", false);
            anim.SetBool("isTalk", false);

            switch(Random.Range(1,3))
            {
                case 1: anim.SetBool("isB", true); break;
                case 2: anim.SetBool("isC", true); break;
                case 3: anim.SetBool("isTalk", true); break;
            }
        }
    }

    public void AccelerateAnimator(Component sender, object obj)
    {
        float _speed = (float)obj;

        Debug.Log("speed: " + _speed);

        anim.speed = _speed;
    }
}
