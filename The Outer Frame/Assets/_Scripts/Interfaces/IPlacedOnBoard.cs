using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlacedOnBoard 
{
    public bool GetConditionalState();

    public bool ActiveInBegining();

    public bool GetIsTaken();
}
