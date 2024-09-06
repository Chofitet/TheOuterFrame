using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TVManager : MonoBehaviour
{
    [SerializeField] List<ChannelController> Channels = new List<ChannelController>();
    [SerializeField] Animator anim;
    // Está suscripto al evento de chequeo de noticias
    // Tiene referenciado los canales por tipo
    // Tiene las listas de las noticias programadas y random
    // Manejará el embudo de noticias. Sacará como conclusión en qué canal y por cuanto tiempo irá una noticia
    // Manejo de cronograma de programación.

    private void Start()
    {
        NewsDirector();
    }
    private void OnEnable()
    {
        TimeManager.OnNewsChange += NewsDirector;
    }
    private void OnDisable()
    {
        TimeManager.OnNewsChange -= NewsDirector;
    }

    [SerializeField] List<TVScheduledNewType> ScheduledNews = new List<TVScheduledNewType>();

    List<TVNewType> ReactiveNews = new List<TVNewType>();

    [SerializeField] List<TVRandomNewType> RandomNews = new List<TVRandomNewType>();
    void NewsDirector()
    {
        ResetChannels();
        Debug.Log("checkingNew");
        CheckForScheduledNews();

        CheckForReactiveNews();


        SetRandomNew();
    }

    public void AddNewToReactiveNewList(Component sender, object obj)
    {
        //agregar un delay con corrutina para agregarlo a la pull (que el dalay dependa de qué acción es la realizada)

        WordData word = (WordData)obj;

        List<StateEnum> history = WordsManager.WM.GetHistory(word);
        StateEnum LastState = history[history.Count - 1];
        TVNewType _new = WordsManager.WM.RequestNew(word, LastState);

        StartCoroutine("DelayToAddReactiveNew", _new);
        
    }

    void CheckForScheduledNews()
    {
        if (ScheduledNews.Count == 0) return;

        List<TVScheduledNewType> newsToRemove = new List<TVScheduledNewType>();

        if (ScheduledNews.Count == 0) return;
        foreach (TVScheduledNewType _new in ScheduledNews)
        {
            TimeData timeNew = _new.GetTimeToShow();
            TimeData ActualTime = TimeManager.timeManager.GetTime();

            if (timeNew.Day == ActualTime.Day && timeNew.Hour == ActualTime.Hour && timeNew.Minute == ActualTime.Minute)
            {
                TVScheduledNewType auxNew = _new.GetNew();
                SetNewInChannel(auxNew);
                newsToRemove.Add(_new);
            }
        }

        foreach (TVScheduledNewType _new in newsToRemove)
        {
            ScheduledNews.Remove(_new);
        }
    }

    void CheckForReactiveNews()
    {
        List<TVNewType> newsToRemove = new List<TVNewType>();
        if (ReactiveNews.Count == 0) return;
        foreach(TVNewType _new in ReactiveNews)
        {
            SetNewInChannel(_new);
            newsToRemove.Add(_new);
        }

        foreach(TVNewType n in newsToRemove)
        {
            ReactiveNews.Remove(n);
        }
    }

    void SetRandomNew()
    {
        if (RandomNews.Count == 0) return;
        TVRandomNewType randomTVNewRandom = RandomNews[Random.Range(0, RandomNews.Count - 1)];
        SetNewInChannel(randomTVNewRandom);
        RandomNews.Remove(randomTVNewRandom);
    }


    void SetNewInChannel(INewType _new)
    {
        if (!GetEmpyChannel()) return;
        ChannelController channelToSet = GetEmpyChannel();
        channelToSet.SetNew(_new);
        channelToSet.SetIsFull(true);
    }

    ChannelController GetEmpyChannel()
    {
        foreach(ChannelController channel in Channels)
        {
            if(!channel.GetIsFull())
            {
                return channel;
            }
        }

        return null;
    }

    IEnumerator DelayToAddReactiveNew(TVNewType _new)
    {
        // A esto le falta depender del Time Manager
        yield return new WaitForSeconds(0);
        ReactiveNews.Add(_new);
    }

    void ResetChannels()
    {
        foreach(ChannelController ch in Channels)
        {
            ch.SetIsFull(false);
            ch.resetChannel();
        }
    }

    public void ChangeChannel(Component sender, object obj)
    {
        GameObject aux = (GameObject)obj;
        aux.SetActive(true);
        ChannelController channel = aux.GetComponent<ChannelController>();
        anim.SetTrigger(channel.GetTriggerAnim());

        foreach(ChannelController ch in Channels)
        {
            if(ch != channel)
            {
                ch.gameObject.SetActive(false);
            }
        }
    }


}
