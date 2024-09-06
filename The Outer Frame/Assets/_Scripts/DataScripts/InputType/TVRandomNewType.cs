using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TVNewRandom", menuName = "News/RandomNew")]
public class TVRandomNewType : ScriptableObject, INewType
{
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string headline;
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int channel;

    public string GetHeadline(){return headline;}

    public bool GetIfIsAEmergency() { return false; }

    public int GetIncreaseAlertLevel()
    {
        return 0;
    }

    public Sprite GetNewImag() { return image; }

    public string GetNewText()
    {
        return text;
    }
}
