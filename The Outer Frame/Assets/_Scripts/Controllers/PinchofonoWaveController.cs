using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class PinchofonoWaveController : MonoBehaviour
{
    [SerializeField] SineWave WaveLine;
    Sequence StartRecordingSequence;
    float amplitud;
    [SerializeField] float MaxAmplitud;
    float frequency;
    [SerializeField] float MaxFrequency;

    [SerializeField] float FrequencyIdle;
    [SerializeField] float AmplitudIdle;
    public void StartRecording(Component sender, object obj)
    {
        StartRecordingSequence = DOTween.Sequence();

        StartRecordingSequence.Append(DOTween.To(() => amplitud, x => amplitud = x, AmplitudIdle, 1f))
                              .Join(DOTween.To(() => frequency, x => frequency = x, FrequencyIdle, 1f));
    }

    public void CatchCall(Component sender, object obj)
    {
        Sequence CathRecordingSequence = DOTween.Sequence();

        CathRecordingSequence.Append(DOTween.To(() => amplitud, x => amplitud = x, MaxAmplitud, 1f))
                              .Join(DOTween.To(() => frequency, x => frequency = x, MaxFrequency, 1f));
    }

    public void EndRecording(Component sender, object obj)
    {
        Sequence EndRecordingSequence = DOTween.Sequence();

        EndRecordingSequence.Append(DOTween.To(() => amplitud, x => amplitud = x, 0, 1f))
                            .Join(DOTween.To(() => frequency, x => frequency = x, 0, 1f));
    }

    private void Update()
    {
        WaveLine.Amplitude = amplitud;
        WaveLine.Frequency = frequency;
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
}

