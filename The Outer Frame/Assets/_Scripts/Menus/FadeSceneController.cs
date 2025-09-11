using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSceneController : MonoBehaviour
{
    [SerializeField] Animator anim;
    public void fadeDuration(float _speed = 1)
    {
        anim.speed = _speed;
    }
}
