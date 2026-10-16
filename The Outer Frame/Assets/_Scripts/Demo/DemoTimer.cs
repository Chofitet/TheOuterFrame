using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DemoTimer : MonoBehaviour
{

    [SerializeField] float timeToWait = 10f;
    [SerializeField] GameEvent OnDemoCut;
    [SerializeField] GameObject Label;
    [SerializeField] TMP_Text Labeltxt;

    bool applyCutDemoWithTime = false;

    private float startTime;
    private bool isRunning;
    bool once;
    bool isInLevel1;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckLevel(scene);
    }

    private void CheckLevel(Scene scene)
    {
        isInLevel1 = scene.name == "Level1";

        isRunning = false;
        once = false;
    }



    public void StartTimer(Component sender, object obj)
    {
        if (!isInLevel1) return;
        startTime = Time.realtimeSinceStartup;
        isRunning = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Equals) ||
                Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                applyCutDemoWithTime = true;
                startTime = Time.realtimeSinceStartup;
                //StartCoroutine(ShowLabel("Timer Enabled"));
            }

            if (Input.GetKeyDown(KeyCode.Minus) ||
                Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                applyCutDemoWithTime = false;
                //StartCoroutine(ShowLabel("Timer Disabled"));
            }
        }

        if (once) return;

        if (!isRunning) return;

        if (Time.realtimeSinceStartup - startTime >= timeToWait * 60f)
        {
            if (applyCutDemoWithTime) OnDemoCut?.Invoke(this, null);
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
