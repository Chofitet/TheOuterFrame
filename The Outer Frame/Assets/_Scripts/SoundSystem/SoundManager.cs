using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    /*public static SoundManager instance{ get; private set; }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

    }*/

    List<GameObject> LoopingSounds = new List<GameObject>();

    List<AudioSource> ClippingSounds = new List<AudioSource>();
    public void InstantiateAndPlaySound(Component sender, object obj)
    {
        SoundInfo soundInfo = (SoundInfo)obj;


        GameObject SoundInstance = new GameObject(sender.name);
        SoundInstance.transform.position = soundInfo.WordPosition.position;
        if (soundInfo.madeDontDestroyInLoad) DontDestroyOnLoad(SoundInstance);

        AudioSource audioSource = SoundInstance.AddComponent<AudioSource>();

        
        audioSource.clip = soundInfo.audioSource.clip;
        if (soundInfo.clips.Count != 0) PlayRandomClip(audioSource, soundInfo.clips);
        audioSource.outputAudioMixerGroup = soundInfo.audioSource.outputAudioMixerGroup;
        audioSource.volume = soundInfo.audioSource.volume / GetVolumeDecrementFactorClippingAudio(audioSource);

        audioSource.pitch = soundInfo.audioSource.pitch;
        if (soundInfo.pitchVariation != Vector2.one) PlayRandomPitch(audioSource,soundInfo.pitchVariation);
        audioSource.loop = soundInfo.audioSource.loop;

        audioSource.spatialBlend = soundInfo.audioSource.spatialBlend;  
        audioSource.minDistance = soundInfo.audioSource.minDistance;  
        audioSource.maxDistance = soundInfo.audioSource.maxDistance;  
        audioSource.rolloffMode = soundInfo.audioSource.rolloffMode;

        ClippingSounds.RemoveAll(x => !x);
        ClippingSounds.Add(audioSource);
        StartCoroutine(CheckForClippingSounds(audioSource));

        audioSource.Play();

        if (audioSource.loop || soundInfo.isDestroyable)
        {
            LoopingSounds.Add(SoundInstance);
            return;
        }

        Destroy(SoundInstance, audioSource.clip.length);
    }

    public void StopSoundEvent(Component sender, object obj)
    {
        SoundInfo soundInfo = (SoundInfo)obj;
        LoopingSounds.RemoveAll(s => s == null);
        foreach (GameObject sound in LoopingSounds)
        {
            if(soundInfo.fadeOutTime != 0)
            {
                sound.GetComponent<AudioSource>().DOFade(0, soundInfo.fadeOutTime);
            }
            else if(soundInfo.Name == sound.name)
            {
                Destroy(sound);
            }
        }
    }

    void PlayRandomClip(AudioSource audioSource, List<AudioClip> _clips)
    {
        int randomIndex = Random.Range(0, _clips.Count);
        audioSource.clip = _clips[randomIndex];
    }

    void PlayRandomPitch(AudioSource audioSource, Vector2 variationPitch)
    {
        float randomPitch = Random.Range(variationPitch.x, variationPitch.y);
        audioSource.pitch = randomPitch;
    }

    IEnumerator CheckForClippingSounds(AudioSource sound)
    {
        yield return new WaitForSeconds(0.3f);
        ClippingSounds.Remove(sound);
    }

    float GetVolumeDecrementFactorClippingAudio(AudioSource sound)
    {
        float decrementFactor = 1.0f;
       foreach(AudioSource clip in ClippingSounds)
        {
            if(clip.clip == sound.clip)
            {
                decrementFactor += 0.5f;
            }
        }

       return decrementFactor;
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
    public bool isDestroyable;
    public float fadeOutTime;
    public bool madeDontDestroyInLoad;

    public SoundInfo(AudioSource _audioClip, float _playDuration, Vector2 _pitchVariation, List<AudioClip> _clips, Transform _WordPosition, string _name, bool _isDestroyable, float _fadeOutTime, bool _madeDontDestroyInLoad)
    {
        audioSource = _audioClip;
        //playDuration = _playDuration;
        pitchVariation = _pitchVariation;
        clips = _clips;
        WordPosition = _WordPosition;
        Name = _name;
        isDestroyable = _isDestroyable;
        fadeOutTime = _fadeOutTime;
        madeDontDestroyInLoad = _madeDontDestroyInLoad;
    }

    
}