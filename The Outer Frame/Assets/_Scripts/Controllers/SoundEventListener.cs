using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundEventListener : MonoBehaviour
{
    [HideInInspector][SerializeField] GameEvent OnPlaySound;
    [SerializeField] float SoundDuration;
    [SerializeField][Range(-3,3)] float MinPitchVariation = 1;
    [SerializeField] [Range(-3, 3)] float MaxPitchVariation = 1;
    [SerializeField] List<AudioClip> Clips = new List<AudioClip>();
    AudioSource audioSource;
    Vector2 PitchVariation;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(SoundDuration == 0)
        {
            SoundDuration = audioSource.clip.length;
        }
        PitchVariation = new Vector2(MinPitchVariation, MaxPitchVariation);
    }

    public void PlaySound(Component sender, object obj)
    {
        OnPlaySound?.Invoke(this, new SoundInfo(audioSource, SoundDuration, PitchVariation, Clips));
    }


}




