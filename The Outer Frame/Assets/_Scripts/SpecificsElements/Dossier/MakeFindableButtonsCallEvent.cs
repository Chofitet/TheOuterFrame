using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeFindableButtonsCallEvent : MonoBehaviour
{
    [SerializeField] GameEvent Event;

    public void SubscribeChildrensToEvent(Component component, object obj)
    {
        GameObject btn = (GameObject)obj;

        if (!CheckIfIsChild(btn)) return;

        btn.GetComponent<Button>().onClick.AddListener(OnButtonPress);
    }

    public void OnButtonPress()
    {
        Event?.Invoke(this, null);
    }

    bool CheckIfIsChild(GameObject btn)
    {
        FindableWordBTNController[] FindableBTNs = GetComponentsInChildren<FindableWordBTNController>();

        foreach(FindableWordBTNController b in FindableBTNs)
        {
            if(b.gameObject == btn)
            {
                return true;
            }
        }
        return false;
    }
}
