using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameEvent OnSetTutorial;
    // Start is called before the first frame update
    void Start()
    {
        OnSetTutorial?.Invoke(this, true);
    }

    public void FinishTutorial(Component sender, object obj)
    {
        OnSetTutorial?.Invoke(this, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
