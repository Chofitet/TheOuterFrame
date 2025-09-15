using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAccelerator : MonoBehaviour
{
    public float speed = 1;
    ParticleSystem ps;
    void Start() { ps = GetComponent<ParticleSystem>() as ParticleSystem; }

    public void ChangeSpeed(Component sender, object obj)
    {

            ps.playbackSpeed = (float)obj;
    }

}
