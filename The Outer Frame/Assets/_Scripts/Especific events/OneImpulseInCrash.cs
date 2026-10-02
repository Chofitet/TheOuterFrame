using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OneImpulseInCrash : MonoBehaviour
{
    [SerializeField] MultipleTrigger trigger;
    [SerializeField] Transform target;
    Rigidbody RB;
    public float force; 

    bool x;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger.onTrigger && x == false)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir = dir.normalized;
            RB.AddForce(dir * force);
            x = true;
            StartCoroutine(Kinetic());
        }
    }

    IEnumerator Kinetic ()
    {
        yield return new WaitForSeconds(2);
        RB.isKinematic = true;
    }
}
