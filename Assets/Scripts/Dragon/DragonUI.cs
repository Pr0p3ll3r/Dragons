using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private Image look;
    [SerializeField] private Animation anim;
    [SerializeField] private Button button;

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

    public void SetUp(DragonInfo dragon)
    {
        if (dragon.shown)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }

        if (!dragon.hatching && dragon.isEgg)
        {
            look.gameObject.SetActive(true);
            timer.gameObject.SetActive(false);
        }
        else if (dragon.hatching && dragon.isEgg)
        {
            look.gameObject.SetActive(false);
            timer.gameObject.SetActive(true);
        }
        else if (dragon.onExpedition)
        {
            look.gameObject.SetActive(false);
            timer.gameObject.SetActive(true);
            button.interactable = false;
        }
        else if (dragon.loot)
        {
            look.gameObject.SetActive(false);
            timer.gameObject.SetActive(true);
            button.interactable = true;
            timer.gameObject.GetComponent<Animation>().Play("Flash");
        }
        else
        {
            look.gameObject.SetActive(true);
            timer.gameObject.SetActive(false);
            timer.gameObject.GetComponent<Animation>().Play("Idle");
        }

        if (dragon.isEgg)
            look.sprite = Database.database.dragons[dragon.ID].eggLook;
        else
            look.sprite = Database.database.dragons[dragon.ID].look;
    }
}
