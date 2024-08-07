using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScheduledNew", menuName = "News/ScheduledNew")]
public class TVScheduledNewType : ScriptableObject, INewType
{
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] int Day;
    [SerializeField] int Hour;
    [SerializeField] int Minute;

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia

    public TimeData GetTimeToShow(){ return new TimeData(Day, Hour, Minute); }
    public string GetHeadline() { return headline; }

    Sprite INewType.GetNewImag(){ return image;}
}
