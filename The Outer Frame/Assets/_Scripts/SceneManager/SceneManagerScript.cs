using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] GameEvent OnStartGame;
    [SerializeField] float OtherBeginDuration;
    [SerializeField] float OtherDuration;
    private void Start()
    {
        fadeObject = Instantiate(fadeScenePrefab, transform);
        if(OtherBeginDuration !=0) fadeObject.GetComponent<FadeSceneController>().fadeDuration(OtherBeginDuration);
        OnStartGame?.Invoke(this, null);
    }

    [SerializeField] GameObject fadeScenePrefab;
    GameObject fadeObject;
    public void OnChangeScene(Component sender, object obj)
    {
        string levelToChange = (string)obj;

        StartCoroutine(ChangeScene(levelToChange));

    }

    IEnumerator ChangeScene(string levelToChange)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(levelToChange);
    }

    public void QuitGame(Component sender, object obj)
    {
        Application.Quit();
    }

    public void ChangeFadeDuration(Component sender, object obj)
    {
        fadeObject.GetComponent<FadeSceneController>().fadeDuration(OtherDuration);
    }
}
