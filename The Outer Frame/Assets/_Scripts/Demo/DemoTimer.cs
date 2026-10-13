using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DemoTimer : MonoBehaviour
{
    [SerializeField] float timeToWait = 10f;
    [SerializeField] GameEvent OnDemoCut;
    [SerializeField] GameObject Label;
    [SerializeField] TMP_Text Labeltxt;

    bool applyCutDemoWithTime = true;

    private float startTime;
    private bool isRunning;
    bool once;

    public void StartTimer(Component sender, object obj)
    {
        startTime = Time.realtimeSinceStartup;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;
        if (once) return;

        if (Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Equals) ||
                Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                applyCutDemoWithTime = true;
                startTime = Time.realtimeSinceStartup;
                StartCoroutine(ShowLabel("Timer Enabled"));
            }

            if (Input.GetKeyDown(KeyCode.Minus) ||
                Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                applyCutDemoWithTime = false;
                StartCoroutine(ShowLabel("Timer Disabled"));
            }
        }

        if (!applyCutDemoWithTime || !isRunning)
            return;

        if (Time.realtimeSinceStartup - startTime >= timeToWait * 60f)
        {
            OnDemoCut?.Invoke(this, null);
            once = true;
        }
    }

    IEnumerator ShowLabel(string text)
    {
        Label.SetActive(true);
        Labeltxt.text = text;
        yield return new WaitForSeconds(2);
        Label.SetActive(false);
    }

}
