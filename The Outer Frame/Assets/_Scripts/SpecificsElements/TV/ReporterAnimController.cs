using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReporterAnimController : MonoBehaviour
{
    Animator anim;
    bool isAnimating;
    float speed;

    [SerializeField] int _minClips = 2;
    [SerializeField] int _maxClips = 11;
    List<string> EmotionsClips = new List<string>() { "isNormal", "isSerious", "isQuestion", "isSurprise" };
    List<string> availableEmotions;
    string LastEmotion;


    private void Start()
    {
        SetTriggerAnim(null,null);
    }

    public void SetTriggerAnim(Component sender, object obj)
    {
        anim = GetComponent<Animator>();
        availableEmotions = new List<string>(EmotionsClips);
        LastEmotion = "isSurprise";
        anim.SetTrigger("newPaper");

        StartTalkLoop();
    }

    void StartTalkLoop()
    {
        StopAllCoroutines();
        anim.SetBool("isTalk", true);

        ChooseRandomEmotion();

        StartCoroutine(RandomizeAnimation());
        StartCoroutine(SetTalkTime(Random.Range(_minClips, _maxClips+1)));
    }

    IEnumerator RandomizeAnimation()
    {        
        while(anim.GetBool("isTalk"))
        {
            yield return new WaitForSeconds(5.05f );
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

    void ChooseRandomEmotion()
    {
        foreach (string emotion in EmotionsClips)
        {
            anim.SetBool(emotion, false);
        }

        if (availableEmotions.Count == 0)
        {
            availableEmotions = new List<string>(EmotionsClips);
        }

        int randomIndex = Random.Range(0, availableEmotions.Count);
        string selectedEmotion = availableEmotions[randomIndex];

        while(selectedEmotion == LastEmotion)
        {
            randomIndex = Random.Range(0, availableEmotions.Count);
            selectedEmotion = availableEmotions[randomIndex];
        }

        anim.SetBool(selectedEmotion, true);

        LastEmotion = selectedEmotion;
        availableEmotions.RemoveAt(randomIndex);
    }

    public void AccelerateAnimator(Component sender, object obj)
    {
        float _speed = (float)obj;

        anim.speed = _speed;
        speed = 1/_speed;
        //Debug.Log("speed: " + _speed);
    }
}
