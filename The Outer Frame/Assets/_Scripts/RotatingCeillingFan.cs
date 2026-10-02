using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotatingCeillingFan : MonoBehaviour
{
    
    MultipleTrigger multipleTrigger;
    Rigidbody RB;
    public float acelerate;
    public float maxVelocity;
    float Counter = 0;
    public float soundAcelerate;
    [SerializeField] AudioSource FanSound;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponentInChildren<Rigidbody>();
        multipleTrigger = GetComponent<MultipleTrigger>();
    }

    private void Update()
    {
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(Counter);
        if (multipleTrigger.onTrigger)
        {
            FanSound.enabled = true;
            if (RB.angularVelocity.magnitude <= maxVelocity)
            {
                if (Counter < 1)
                {
                    Counter += Time.smoothDeltaTime * soundAcelerate;
                }
                
                RB.isKinematic = false;
                RB.AddTorque(Vector3.up * acelerate);
            }
        }
        else
        {
            if (RB.angularVelocity.magnitude >= 0)
            {
                if(Counter > 0) Counter -= Time.smoothDeltaTime * soundAcelerate;

                RB.AddTorque(-Vector3.up * acelerate);
            }
            else FanSound.enabled = false;
        }
        Debug.Log(RB.angularVelocity.y);
        if (RB.angularVelocity.y < 0)
        {
            RB.isKinematic = true;

        }

        
        FanSound.volume = Counter;

    }
}
