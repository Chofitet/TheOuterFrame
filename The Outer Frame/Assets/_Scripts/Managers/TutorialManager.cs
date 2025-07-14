using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] GameEvent OnSetTutorial;
    [SerializeField] bool StartWithTutorial;
    bool isInTutorial = true;
    // Start is called before the first frame update
    void Start()
    {
        if(isInTutorial || StartWithTutorial) OnSetTutorial?.Invoke(this, true);
        else
        {
            OnSetTutorial?.Invoke(this, false);
        }
    }

    public void FinishTutorial(Component sender, object obj)
    {
        OnSetTutorial?.Invoke(this, false);
        isInTutorial = false;
        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.SaveGame();
    }

    public void LoadData(GameData data)
    {
        isInTutorial = data.TutorialComplete;
    }

    public void SaveData(GameData data)
    {
        data.TutorialComplete = isInTutorial;
    }
}
