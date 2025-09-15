using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoSplashController : MonoBehaviour
{
    VideoPlayer video;
    [SerializeField] GameEvent EventTotrigger;
    // Start is called before the first frame update
    void Start()
    {
        video = GetComponent<VideoPlayer>();
        StartCoroutine(CheckEnVideo());

    }

    IEnumerator CheckEnVideo()
    {
        yield return new WaitForSeconds ((float)video.length);

        EventTotrigger?.Invoke(this, "SapucaiLogo");

        
    }

}
