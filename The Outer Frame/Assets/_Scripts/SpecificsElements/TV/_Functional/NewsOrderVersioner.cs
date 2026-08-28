using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "oodas", menuName = "aaaaaaaa")]
public class NewsOrderVersioner : ScriptableObject
{
    [SerializeField] List<ScheduledNewsOrderList> ScheduledNews = new List<ScheduledNewsOrderList>();

    [SerializeField] List<ReactiveNewsOrderList> ReactiveNews = new List<ReactiveNewsOrderList>();
}


[Serializable]
public class ScheduledNewsOrderList
{
    public string name;
    [SerializeField] List<TVScheduledNewType> Versions = new List<TVScheduledNewType>();
}

[Serializable]
public class ReactiveNewsOrderList
{
    public string name;
    [SerializeField] List<TVNewType> Versions = new List<TVNewType>();
}