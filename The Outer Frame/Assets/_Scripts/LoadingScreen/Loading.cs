using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
        public Image progressBar;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScreen")
        {
            progressBar.gameObject.SetActive(true);
            progressBar.fillAmount = 0;

        }
        else progressBar.gameObject.SetActive(false);
    }

    public void UpdateProgress(float v)
        {
            v = Mathf.Clamp(v,0,0.99f);
            progressBar.fillAmount = v;
        }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
