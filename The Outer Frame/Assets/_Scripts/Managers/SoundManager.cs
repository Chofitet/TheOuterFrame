using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    List<GameObject> LoopingSounds = new List<GameObject>();
    public void InstantiateAndPlaySound(Component sender, object obj)
    {
        SoundInfo soundInfo = (SoundInfo)obj;


        GameObject SoundInstance = new GameObject(sender.name);
        SoundInstance.transform.position = soundInfo.WordPosition.position;

        AudioSource audioSource = SoundInstance.AddComponent<AudioSource>();

        audioSource.clip = soundInfo.audioSource.clip;
        if (soundInfo.clips.Count != 0) PlayRandomClip(audioSource, soundInfo.clips);
        audioSource.outputAudioMixerGroup = soundInfo.audioSource.outputAudioMixerGroup;
        audioSource.volume = soundInfo.audioSource.volume;
        audioSource.pitch = soundInfo.audioSource.pitch;
        if (soundInfo.pitchVariation != Vector2.one) PlayRandomPitch(audioSource,soundInfo.pitchVariation);
        audioSource.loop = soundInfo.audioSource.loop;

        audioSource.spatialBlend = soundInfo.audioSource.spatialBlend;  
        audioSource.minDistance = soundInfo.audioSource.minDistance;  
        audioSource.maxDistance = soundInfo.audioSource.maxDistance;  
        audioSource.rolloffMode = soundInfo.audioSource.rolloffMode;


        audioSource.Play();

        if (audioSource.loop)
        {
            LoopingSounds.Add(SoundInstance);
            return;
        }

        Destroy(SoundInstance, audioSource.clip.length);
    }

    public void StopSoundEvent(Component sender, object obj)
    {
        SoundInfo soundInfo = (SoundInfo)obj;
        foreach(GameObject sound in LoopingSounds)
        {
            if(soundInfo.Name == sound.name)
            {
                Destroy(sound);
            }
        }
    }

    void PlayRandomClip(AudioSource audioSource, List<AudioClip> _clips)
    {
        int randomIndex = Random.Range(0, _clips.Count - 1);
        audioSource.clip = _clips[randomIndex];
    }

    void PlayRandomPitch(AudioSource audioSource, Vector2 variationPitch)
    {
        float randomPitch = Random.Range(variationPitch.x, variationPitch.y);
        audioSource.pitch = randomPitch;
    }
}


public struct SoundInfo
{
    public string Name;
    public AudioSource audioSource;
    //public float playDuration;
    public Vector2 pitchVariation;
    public List<AudioClip> clips;
    public Transform WordPosition;

    public SoundInfo(AudioSource _audioClip, float _playDuration, Vector2 _pitchVariation, List<AudioClip> _clips, Transform _WordPosition, string _name)
    {
        audioSource = _audioClip;
        //playDuration = _playDuration;
        pitchVariation = _pitchVariation;
        clips = _clips;
        WordPosition = _WordPosition;
        Name = _name;
    }

    
}