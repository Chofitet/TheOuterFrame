using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumenSettings : MonoBehaviour
{
    float MusicVolume = 1;
    float SFXVolume = 1;
    [SerializeField] GameEvent OnSetMusicVolume;
    [SerializeField] GameEvent OnSetSFXVolume;
    public static VolumenSettings instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;

        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSetMusicVolume?.Invoke(this,MusicVolume);
        OnSetSFXVolume?.Invoke(this,SFXVolume);
    }

    public void OnSaveMusicVolume(Component sender, object volume)
    {
        MusicVolume = MusicVolume + (float)volume;
        MusicVolume = Mathf.Clamp(MusicVolume, 0.001f, 1);
    }
    public void OnSaveSFXVolume(Component sender, object volume)
    {
        SFXVolume = SFXVolume + (float)volume;
        SFXVolume = Mathf.Clamp(SFXVolume, 0.001f, 1);
    }



}
