using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScheduledNew", menuName = "Input/News/ScheduledNew")]
public class TVScheduledNewType : ScriptableObject, INewType
{
    [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] int Day;
    [SerializeField] int Hour;
    [SerializeField] int Minute;

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia

    public TimeData GetTimeToShow(){ return new TimeData(Day, Hour, Minute); }
    public string GetHeadline() { return headline; }
}
