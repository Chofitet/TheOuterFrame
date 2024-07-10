using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TVNewRandom", menuName = "Input/News/TVNewRandom")]
public class TVRandomNewType : ScriptableObject, INewType
{
    [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;

    public string GetHeadline()
    {
        return headline;
    }
}
