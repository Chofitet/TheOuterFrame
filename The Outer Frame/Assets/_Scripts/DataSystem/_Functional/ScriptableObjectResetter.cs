using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptableObjectResetter : MonoBehaviour
{
    public static ScriptableObjectResetter instance { get; private set; }
    HashSet<IReseteableScriptableObject> SOlist = new HashSet<IReseteableScriptableObject>();
    [SerializeField] GameEvent OnChangeScene;
    [SerializeField] Loading loadingUI;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            
        }


        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetSOs(Component sender, object obj)
    {
       StartCoroutine(ResetAllScriptableObject());
    }

    public void RegisterScriptableObject(ScriptableObject _SO)
    {
        IReseteableScriptableObject SO = _SO as IReseteableScriptableObject;
        SOlist.Add(SO);
    }

    public IEnumerator ResetAllScriptableObject()
    {
        int total = SOlist.Count;
        int count = 0;

        foreach (var so in SOlist)
        {
            so.ResetScriptableObject();
            count++;

            loadingUI.UpdateProgress((float)count / total * 0.3f);
            Debug.Log((float)count / total);

            if (count % 80 == 0)
            {
                yield return new WaitForSeconds(0.01f);
            }

        }
        AsyncOperation op = SceneManager.LoadSceneAsync("Level1");
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            // op.progress va de 0 a 0.9
            float loadProgress = Mathf.Clamp01(op.progress / 0.9f);

            // El resto del 30% al 100% lo ocupa la carga
            float totalProgress = 0.3f + loadProgress * 0.7f;

            loadingUI.UpdateProgress(totalProgress);

            // Cuando llega al 90% (op.progress == 0.9), Unity está esperando que la actives
            if (op.progress >= 0.9f)
            {
                // Podés esperar un fade
                yield return new WaitForSeconds(0.2f);

                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
