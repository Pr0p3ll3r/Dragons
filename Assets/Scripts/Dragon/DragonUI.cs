using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragonUI : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public Animation anim;

    public void RefreshTimerExpedition(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString = timeSpan.ToString(@"hh\:mm\:ss");
        timer.text = $"Back in " + timeString;

        if (time <= 0)
        {
            timer.text = "LOOT!";
            anim.Play("Flash");
        }          
    }

    public void RefreshTimerHatching(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString = timeSpan.ToString(@"hh\:mm\:ss");
        timer.text = timeString;
    }
}
