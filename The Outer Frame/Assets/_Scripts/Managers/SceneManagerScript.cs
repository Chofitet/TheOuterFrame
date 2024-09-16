using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    private void Start()
    {
        fadeObject = Instantiate(fadeScenePrefab, transform);
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
}
