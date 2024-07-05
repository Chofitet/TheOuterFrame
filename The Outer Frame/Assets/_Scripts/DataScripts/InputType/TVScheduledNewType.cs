using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScheduledNewType : ScriptableObject
{
    [SerializeField] string headline;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [SerializeField] int alertLevelIncrement;

    //Lista de condicionantes y chequeo de si son true todas para desactivar o reprogramar noticia
}
