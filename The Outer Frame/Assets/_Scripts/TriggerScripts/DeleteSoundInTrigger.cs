using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSoundInTrigger : MonoBehaviour
{
    MultipleTrigger multipleTrigger;
    [SerializeField] GameObject[] toDelete;
    // Start is called before the first frame update
    void Start()
    {
        multipleTrigger = GetComponent<MultipleTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (multipleTrigger.onTrigger)
        {
            foreach( GameObject Object in toDelete)
            Destroy(Object) ;
        }
    }
}
