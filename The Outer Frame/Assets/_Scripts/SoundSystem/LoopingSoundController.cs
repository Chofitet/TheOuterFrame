using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSoundController : MonoBehaviour
{
    [SerializeField] private GameEvent InstanciateStartSound;
    [SerializeField] float timeToLoopNext;
    [SerializeField] GameEvent InstanciateSound;
    [SerializeField] float timeToStartCycle;

    bool isLooping;

    private void Start()
    {
        StartCoroutine(LoopSound());
    }

    IEnumerator LoopSound()
    {
        yield return new WaitForSeconds(0.5f);
        InstanciateStartSound?.Invoke(this, null);
        yield return new WaitForSeconds(timeToStartCycle);
        InstanciateSound?.Invoke(this,null);

        while (true)
        {
           yield return new WaitForSeconds(timeToLoopNext);
            InstanciateSound?.Invoke(this, null);

        }
    }

}
