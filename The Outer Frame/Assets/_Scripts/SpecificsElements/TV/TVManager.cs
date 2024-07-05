using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TVManager : MonoBehaviour
{

    [SerializeField] List<TVScheduledNewType> ScheduledNews = new List<TVScheduledNewType>();

    List<TVNewType> ReactiveNews = new List<TVNewType>();

    [SerializeField] List<TVRandomNewType> RandomNews = new List<TVRandomNewType>();
   void NewsDirector()
    {
        //Chequear Noticias programadas

        //Chequear Noticias Reactivas

        //Chequear Noticias Random
    }

    void AddNewToReactiveNewList(Component sender, object obj)
     {
        //agregar un delay con corrutina para agregarlo a la pull (que el dalay dependa de qué acción es la realizada)

        TVNewType _new = (TVNewType)obj;

        ReactiveNews.Add(_new);
     }

    void OrderListDate(List<TVNewType> list)
    {
        


    }
}
