using System;
using System.Collections.Generic;
using UnityEngine;

class NextStation : MonoBehaviour
{
    public Transform nextTarget;
    public Navigation navigation;

    public void nextStation()
    {
        navigation.switchArrival(nextTarget);
    }

    public void EndTravel()
    {
        navigation.EndGame();
    }
}

