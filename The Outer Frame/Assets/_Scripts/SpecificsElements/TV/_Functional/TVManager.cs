using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TVManager : MonoBehaviour
{
    public static Action OnNewGenericEmptySlotTV;
    [SerializeField] int InitChannel = 1;
    [SerializeField] List<ChannelController> Channels = new List<ChannelController>();
    [HideInInspector][SerializeField] TMP_Text ChannelNumTxt;
    [SerializeField] List<TVScheduledNewType> ScheduledNews = new List<TVScheduledNewType>();
    List<INewType> QueueOfNews = new List<INewType>();
    [SerializeField] List<TVNewType> ReactiveNews = new List<TVNewType>();
    [SerializeField] TVRandomNewType RunOutOfNews;
    [SerializeField] List<TVRandomNewType> RandomNews = new List<TVRandomNewType>();
    [SerializeField] Animator anim;
    private void Start()
    {
        AddVilifiedNewsToReactiveList(WordsManager.WM.GetVilifiedNews());
        FillEmptiesChannels();
        ChangeChannel(null, Channels[InitChannel].gameObject);
    }

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += NewsDirector;
       // TimeManager.OnMinuteChange += CountTimeToNewGenericEmptySlot;
    }
    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= NewsDirector;
       // TimeManager.OnMinuteChange -= CountTimeToNewGenericEmptySlot;
    }
    

    #region RandomNewLogic
    /*void Subscribe_UnsuscribeCountTimeToNewGenericEmptySlot(bool x)
    {
        if (x)
        {
            minuteCounter = 0;
            TimeManager.OnNewsChange += CountTimeToNewGenericEmptySlot;
        }
        else TimeManager.OnNewsChange -= CountTimeToNewGenericEmptySlot;
    }

    int minuteCounter = 0;
    void CountTimeToNewGenericEmptySlot()
    {
        minuteCounter++;

        if(minuteCounter >= DefaultMinuteToChangeNews)
        {
            SetRandomNew();
            minuteCounter = 0;
        }
    }*/

    
    #endregion

    void NewsDirector()
    { 
        //checkean si una noticia nueva entró y la coloca en la cola.
        CheckForScheduledNews();
        CheckForReactiveNews();
        CheckForQueue();

    }

    void CheckForScheduledNews()
    {
        if (ScheduledNews.Count == 0) return;

        List<TVScheduledNewType> newsToRemove = new List<TVScheduledNewType>();

        foreach (TVScheduledNewType _new in ScheduledNews)
        {
            if (_new.GetStateConditionalToAppear())
            {
                TVScheduledNewType auxNew = _new.GetNew();
                QueueOfNews.Add(auxNew);
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
        if (ReactiveNews.Count == 0) return;

        List<TVNewType> newsToRemove = new List<TVNewType>();

        foreach (TVNewType _new in ReactiveNews)
        {
            if (_new.GetStateConditionalToAppear())
            {
                QueueOfNews.Add(_new);
                newsToRemove.Add(_new);
            }
        }

        foreach (TVNewType _new in newsToRemove)
        {
            ReactiveNews.Remove(_new);
        }
    }

    void CheckForQueue()
    {
        //chequeos de prioridad (siempre salen las programadas primero, despues las reaccionarias en orden de aparicion)
        if (QueueOfNews.Count == 0)
        {
            Debug.Log("newsQueueFree");
            FillEmptiesChannels();
            return;
        }

        List<INewType> NewsToRemove = new List<INewType>();

        foreach(INewType _new in QueueOfNews)
        {
            if(_new.GetPriority() == 1)
            {
                if (SetNewInChannel(_new) != null) NewsToRemove.Add(_new);
            }
        }

        if (NewsToRemove.Count != 0) QueueOfNews.RemoveAll(_new => NewsToRemove.Contains(_new));

        foreach (INewType _new in QueueOfNews)
        {
            if (SetNewInChannel(_new) != null) NewsToRemove.Add(_new);
        }

        if (NewsToRemove.Count != 0) QueueOfNews.RemoveAll(_new => NewsToRemove.Contains(_new));
    }

    INewType SetNewInChannel(INewType _new)
    {
        int ChannelNum = _new.GetChannelNum();
        ChannelController channelToSet = null;

        // todos los canales ocupados
        if (!GetEmpyChannel())
        {
            return null;
        }

       // está lleno su canal correspondiente
        if(Channels[ChannelNum].GetIsMinTimePass())
        {
            channelToSet = GetEmpyChannel();
        }
        else
        {
            channelToSet = Channels[ChannelNum];
        }

        channelToSet.SetNew(_new);

        return _new;
    }

    ChannelController GetEmpyChannel()
    {
        foreach(ChannelController channel in Channels)
        {
            if (channel.GetisStaticChannel()) continue;
            if(channel.GetIsMinTimePass())
            {
                return channel;
            }
        }

        return null;
    }

    void FillEmptiesChannels()
    {
        foreach (ChannelController channel in Channels)
        {
            if (channel.GetisStaticChannel()) continue;
            if (channel.GetTimeToRestartRandoms())
            {
                channel.SetNew(SetRandomNew(Channels.IndexOf(channel)));
                channel.SetIsFull(true);
            }
        }
    }
    INewType SetRandomNew(int channel)
    {
        int a = RandomNews.Count;
        if (RandomNews.Count == 0)
        {
            return RunOutOfNews;
        }

        List<TVRandomNewType> AuxRandomNewList = new List<TVRandomNewType>();

        foreach(TVRandomNewType auxRandomNew in RandomNews)
        {
            if(auxRandomNew.GetChannelNum() == channel)
            {
                AuxRandomNewList.Add(auxRandomNew);
            }
        }
        TVRandomNewType randomTVNewRandom;
        if (AuxRandomNewList.Count != 0)
        {
            randomTVNewRandom = AuxRandomNewList[UnityEngine.Random.Range(0, RandomNews.Count - 1)];
            RandomNews.Remove(randomTVNewRandom);
            return randomTVNewRandom;
        }
        else
        {
            randomTVNewRandom = RandomNews[UnityEngine.Random.Range(0, RandomNews.Count - 1)];
           RandomNews.Remove(randomTVNewRandom);
            return randomTVNewRandom;
        }
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
        CanvasGroup canvasGroup = aux.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        ChannelController channel = aux.GetComponent<ChannelController>();
        anim.SetTrigger(channel.GetTriggerAnim());
        ChannelNumTxt.text = channel.GetNumOfChannel();

        foreach (ChannelController ch in Channels)
        {
            if(ch != channel)
            {
                CanvasGroup _canvasGroup = ch.GetComponent<CanvasGroup>();
                _canvasGroup.alpha = 0;
                _canvasGroup.interactable = false;
            }
        }
    }

    void AddVilifiedNewsToReactiveList(List<TVNewType> list)
    {
        foreach(TVNewType _new in list)
        {
            if (ReactiveNews.Contains(_new)) continue;

            ReactiveNews.Add(_new);
        }
    }
}