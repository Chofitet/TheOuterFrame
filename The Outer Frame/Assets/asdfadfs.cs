using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asdfadfs : MonoBehaviour
{
    [SerializeField] Material glitchMat;
    [SerializeField] float glitchTime = 1f;
    [SerializeField] float maxIntensity = 0.6f;

    void Start()
    {
        glitchMat.SetFloat("_Intensity", 0f);
    }

    [ContextMenu("glich")]
    public void PlayGlitch()
    {
        StartCoroutine(GlitchRoutine());
    }

    System.Collections.IEnumerator GlitchRoutine()
    {
        float t = 0;
        while (t < glitchTime)
        {
            t += Time.deltaTime;
            float value = Mathf.PingPong(t * 3f, maxIntensity);
            glitchMat.SetFloat("_Intensity", value);
            yield return null;
        }
        glitchMat.SetFloat("_Intensity", 0f);
    }
}
