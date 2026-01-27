using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SoundEventListener : MonoBehaviour
{
    [HideInInspector][SerializeField] GameEvent OnPlaySound;
    [SerializeField] GameEvent StopSoundEvent;
    [SerializeField] float SoundDuration;
    [SerializeField] float SoundFadeOut;
    [SerializeField][Range(-3,3)] float MinPitchVariation = 1;
    [SerializeField] [Range(-3, 3)] float MaxPitchVariation = 1;
    [SerializeField] List<AudioClip> Clips = new List<AudioClip>();
    [SerializeField] bool isDestroyable;
    [SerializeField] bool madeDontDestroyInLoad;
    AudioSource audioSource;
    Vector2 PitchVariation;
    SoundInfo _soundInfo;
    [SerializeField] bool isInactive;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(SoundDuration == 0 && audioSource.clip)
        {
            SoundDuration = 0;
        }
        PitchVariation = new Vector2(MinPitchVariation, MaxPitchVariation);
      
    }

    public void PlaySound(Component sender, object obj)
    {
        if (isInactive) return;
        if (!audioSource.clip && Clips.Count == 0) return;
        //Debug.Log("play " + audioSource.clip.name);
        _soundInfo = new SoundInfo(audioSource, SoundDuration, PitchVariation, Clips, transform, name, isDestroyable, SoundFadeOut, madeDontDestroyInLoad);
       OnPlaySound?.Invoke(this, _soundInfo);
    }

    [ContextMenu("PlaySound")]
    void TestPlaySound()
    {
        PlaySound(null,null);
    }

    public void StopSound(Component sender, object obj)
    {
        StopSoundEvent?.Invoke(null, _soundInfo);
    }

    public void ChangeIsDestroyable(Component sender, object obj)
    {
        isDestroyable = !isDestroyable;
    }

    public void InactiveSound(Component sender, object obj)
    {
        isInactive = true;
    }

}




