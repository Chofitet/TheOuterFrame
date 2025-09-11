using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class PinchofonoWaveController : MonoBehaviour
{
    [SerializeField] GameObject WaveLine;
    SineWave[] sineWaves;
    Sequence StartRecordingSequence;
    float amplitud1;
    float amplitud2;
    [SerializeField] float MaxAmplitud;
    float frequency1;
    float frequency2;
    [SerializeField] float MaxFrequency;

    [SerializeField] float FrequencyIdle;
    [SerializeField] float AmplitudIdle;

    private void Start()
    {
        sineWaves = WaveLine.GetComponents<SineWave>();
    }

    public void StartRecording(Component sender, object obj)
    {
        StartRecordingSequence = DOTween.Sequence();
        CatchCallSequence1.Kill();
        CatchCallSequence2.Kill();

        StartRecordingSequence.Append(DOTween.To(() => amplitud1, x => amplitud1 = x, AmplitudIdle, 1f))
                              .Join(DOTween.To(() => frequency1, x => frequency1 = x, FrequencyIdle, 1f))
                              .Join(DOTween.To(() => amplitud2, x => amplitud2 = x, AmplitudIdle, 1f))
                              .Join(DOTween.To(() => frequency2, x => frequency2 = x, FrequencyIdle, 1f));
                              //.OnComplete(() => StaticLoop());
    }

    Sequence CatchCallSequence1;
    Sequence CatchCallSequence2;

    public void StaticLoop()
    {
        // Secuencia para amplitud1 y frecuencia1 en estado idle
        if (CatchCallSequence1 != null && CatchCallSequence1.IsPlaying())
        {
            CatchCallSequence1.Kill();
        }

        CatchCallSequence1 = DOTween.Sequence();

        CatchCallSequence1.AppendCallback(() =>
        {
            float idleAmplitud1 = 0;  // Asume que este es el valor idle
            float idleFrequency1 = 0; // Asume que este es el valor idle

            CatchCallSequence1.Append(DOTween.To(() => amplitud1, x => amplitud1 = x, idleAmplitud1, 1f))
                               .Join(DOTween.To(() => frequency1, x => frequency1 = x, idleFrequency1, 1f));
        })
        .SetLoops(-1, LoopType.Yoyo);

        // Secuencia para amplitud2 y frecuencia2 en estado idle
        if (CatchCallSequence2 != null && CatchCallSequence2.IsPlaying())
        {
            CatchCallSequence2.Kill();
        }

        CatchCallSequence2 = DOTween.Sequence();

        CatchCallSequence2.AppendCallback(() =>
        {
            float idleAmplitud2 = 0;  // Asume que este es el valor idle
            float idleFrequency2 = 0; // Asume que este es el valor idle

            CatchCallSequence2.Append(DOTween.To(() => amplitud2, x => amplitud2 = x, idleAmplitud2, 1f))
                               .Join(DOTween.To(() => frequency2, x => frequency2 = x, idleFrequency2, 1f));
        })
        .SetLoops(-1, LoopType.Yoyo);
    }


    public void CatchCall(Component sender, object obj)
    {
        
        StartCoroutine(StopCatchAnim());
        // Secuencia para amplitud1 y frecuencia1
        if (CatchCallSequence1 != null && CatchCallSequence1.IsPlaying())
        {
            CatchCallSequence1.Kill();
        }

        CatchCallSequence1 = DOTween.Sequence();

        CatchCallSequence1.AppendCallback(() =>
        {
            float randomAmplitud1 = Random.Range(0, MaxAmplitud);
            float randomFrequency1 = Random.Range(0, MaxFrequency);

            CatchCallSequence1.Append(DOTween.To(() => amplitud1, x => amplitud1 = x, randomAmplitud1, 1f))
                               .Join(DOTween.To(() => frequency1, x => frequency1 = x, randomFrequency1, 1f));
        })
        .SetLoops(-1, LoopType.Yoyo);

        // Secuencia para amplitud2 y frecuencia2
        if (CatchCallSequence2 != null && CatchCallSequence2.IsPlaying())
        {
            CatchCallSequence2.Kill();
        }

        CatchCallSequence2 = DOTween.Sequence();

        CatchCallSequence2.AppendCallback(() =>
        {
            float randomAmplitud2 = Random.Range(0, MaxAmplitud);
            float randomFrequency2 = Random.Range(0, MaxFrequency);

            CatchCallSequence2.Append(DOTween.To(() => amplitud2, x => amplitud2 = x, randomAmplitud2, 1f))
                               .Join(DOTween.To(() => frequency2, x => frequency2 = x, randomFrequency2, 1f));
        })
        .SetLoops(-1, LoopType.Yoyo);
    }

    public void EndRecording(Component sender, object obj)
    {
        Sequence EndRecordingSequence = DOTween.Sequence();
        CatchCallSequence1.Kill();
        CatchCallSequence2.Kill();
        StopAllCoroutines();

        EndRecordingSequence.Append(DOTween.To(() => amplitud1, x => amplitud1 = x, 0, 1f))
                            .Join(DOTween.To(() => frequency1, x => frequency1 = x, 0, 1f))
                            .Join(DOTween.To(() => amplitud2, x => amplitud2 = x, 0, 1f))
                            .Join(DOTween.To(() => frequency2, x => frequency2 = x, 0, 1f));
    }

    private void Update()
    {
        sineWaves[0].Amplitude = amplitud1;
        sineWaves[0].Frequency = frequency1;
        sineWaves[1].Amplitude = amplitud2;
        sineWaves[1].Frequency = frequency2;
    }

    [ContextMenu("Start Recording")]
    private void StartRecordingTest()
    {
        StartRecording(null, null);
    }

    [ContextMenu("End Recording")]
    private void EndRecordingTest()
    {
        EndRecording(null, null);
    }

    [ContextMenu("Catch Recording")]
    private void CathRecordingTest()
    {
        CatchCall(null, null);
    }

    IEnumerator StopCatchAnim()
    {
        yield return new WaitForSeconds(15f);
        StartRecording(null, null);
    }
}

