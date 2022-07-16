using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public DragonInfo newDragon;
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI hatchingTimeText;
    [SerializeField] private GameObject fireButton;
    [SerializeField] private GameObject fireAnimation;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_InputField nameInput;   
    private float hatchingTimer;
    private string dragonName;
    private GameObject egg;
    private Animator animator;

    void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        namePanel.SetActive(false);
        timer.SetActive(false);

        if (newDragon.toName)
        {
            namePanel.SetActive(true);
            fireButton.SetActive(false);
        }
        else if(newDragon.hatching)
        {
            egg = Instantiate(newDragon.prefabEgg, transform);
            animator = egg.GetComponent<Animator>();
            timer.SetActive(true);
            fireButton.SetActive(false);
            hatchingTimer = newDragon.remainingHatchingTime;
            StartCoroutine(Hatching(hatchingTimer));
        }
        else
        {
            egg = Instantiate(newDragon.prefabEgg, transform);
            animator = egg.GetComponent<Animator>();
        }
    }

    public void StartHatching()
    {
        fireAnimation.SetActive(true);
        fireButton.SetActive(false);
        timer.SetActive(true);
        newDragon.hatching = true;
        hatchingTimer = newDragon.hatchingTime;
        newDragon.remainingHatchingTime = hatchingTimer;
        StartCoroutine(Hatching(hatchingTimer));
        GameController.Instance.StartHatching(hatchingTimer, newDragon);
    }

    private IEnumerator Hatching(float time)
    {
        SetTimer(time);

        yield return new WaitForSeconds(1f);

        time -= 1;

        SetTimer(time);
        if (time > 0)
        {
            StartCoroutine(Hatching(time));
        }
    }

    void SetTimer(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString = timeSpan.ToString(@"hh\:mm\:ss");
        hatchingTimeText.text = timeString;
    }

    public void EndHatching()
    {
        if (newDragon.toName)
        {
            timer.SetActive(false);
            namePanel.SetActive(true);
        }
        if(egg != null)
        {
            animator.SetTrigger("Cracking");
        }
    }

    public void SetName(string n)
    {
        dragonName = n;
    }

    public void NameDragon()
    {
        if (string.IsNullOrEmpty(dragonName)) return;

        newDragon.isEgg = false;
        newDragon.toName = false;
        newDragon.shown = false;
        newDragon.dragonName = nameInput.text;
        GameController.Instance.RefreshDragonList();
        Destroy(gameObject);
    }

    public void HideEgg()
    {
        newDragon.shown = false;
        GameController.Instance.RefreshDragonList();
        Destroy(gameObject);
    }
}
