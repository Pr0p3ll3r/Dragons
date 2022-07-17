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
    [SerializeField] private Button buttonRelease;
    private DragonInfo dragon;

    public void RefreshTimerExpedition(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString = timeSpan.ToString(@"hh\:mm\:ss");
        timer.text = $"Back in\n" + timeString;

        if (time <= 0)
        {
            timer.text = "LOOT!";
            anim.Play("Flash");
        }
    }

    public void RefreshTimer(string text, float time)
    {
        if (dragon == null)
            return;

        if (dragon.onExpedition || dragon.loot)
            return;

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString = timeSpan.ToString(@"hh\:mm\:ss");
        timer.text = text + "\n" + timeString;
    }

    public void SetUp(DragonInfo _dragon)
    {
        dragon = _dragon;
        look.gameObject.SetActive(false);
        timer.gameObject.SetActive(true);

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
        else if (dragon.hatching && dragon.isEgg || !dragon.canEat)
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
            timer.text = "LOOT!";
            timer.gameObject.GetComponent<Animation>().Play("Flash");
        }
        else
        {
            look.gameObject.SetActive(true);
            timer.gameObject.SetActive(false);
            timer.gameObject.GetComponent<Animation>().Play("Idle");
        }

        //set look
        if (dragon.toName)
            look.sprite = Database.database.dragons[dragon.ID].lookBaby;
        else if (dragon.isEgg)
            look.sprite = Database.database.dragons[dragon.ID].eggLook;
        else if (!dragon.isAdult)
            look.sprite = Database.database.dragons[dragon.ID].lookBaby;
        else
            look.sprite = Database.database.dragons[dragon.ID].lookAdult;

        //check if can release
        if (dragon.hatching || dragon.onExpedition || dragon.loot || !dragon.canEat)
            buttonRelease.interactable = false;
        else
            buttonRelease.interactable = true;
    }
}
