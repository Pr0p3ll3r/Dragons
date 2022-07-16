using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon", menuName = "ScriptableObjects/Dragon")]
public class DragonInfo : ScriptableObject
{
    public int ID;
    public string dragonName = "";
    public GameObject prefabBaby;
    public GameObject prefabAdult;
    public GameObject prefabEgg;
    public Sprite look;
    public Sprite eggLook;
    public float hatchingTime;
    public float eatingTime;
    public bool isEgg;
    public bool toName;
    public bool isAdult;
    public int level = 1;
    public int remainPoints;
    public bool hatching;
    public bool shown;
    public bool onExpedition;
    public bool loot;
    public bool canEat;
    public int index = -1;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Stat[] stats = new Stat[5];

    public Equipment[] equipment = new Equipment[4];

    [Header("Rewards")]
    public int rewardGold;
    public int rewardSilver;

    [Header("Saved")]
    public float remainingHatchingTime;
    public float remainingEatingTime;
    public float remainingExpeditionTime;
    public int currentExpedition;

    public DragonInfo GetCopy()
    {
        return Instantiate(this);
    }

    public void Initialize()
    {
        stats[0] = new Stat(stats[0].GetValue(), StatType.Damage);
        stats[1] = new Stat(stats[1].GetValue(), StatType.Defense);
        stats[2] = new Stat(stats[2].GetValue(), StatType.Strength);
        stats[3] = new Stat(stats[3].GetValue(), StatType.Vitality);
        stats[4] = new Stat(stats[4].GetValue(), StatType.Luck);
    }

    public void LoadEquipment()
    {
        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null) continue;
            AddStats(equipment[i]);
        }
    }
        
    public void LvlUp()
    {
        level++;
        remainPoints++;
        if (level >= 15) isAdult = true;
    }

    public void AddStrength()
    {
        stats[2].AddPoint();
        remainPoints--;
        stats[0].AddPoint();
    }

    public void AddVitality()
    {
        stats[3].AddPoint();
        remainPoints--;
        maxHealth += 10;
    }

    public void AddLuck()
    {
        stats[4].AddPoint();
        remainPoints--;
    }

    public void AddStats(Equipment item)
    {
        foreach (ItemStat itemStat in item.stats)
        {
            foreach (Stat stat in stats)
            {
                if (itemStat.statType == stat.statType)
                {
                    stat.AddModifier(itemStat.value);
                    break;
                }                  
            }
        }
    }

    public void RemoveStats(Equipment item)
    {
        foreach (ItemStat itemStat in item.stats)
        {
            foreach (Stat stat in stats)
            {
                if (itemStat.statType == stat.statType)
                {
                    stat.RemoveModifier(itemStat.value);
                    break;
                }                  
            }
        }
    }

    public bool TakeDamage(int damage)
    {
        damage -= stats[1].GetValue();
        damage = Mathf.Clamp(damage, 1, int.MaxValue);
        currentHealth -= damage;

        //Debug.Log(dragonName + " takes " + damage + " damage.");
        if (currentHealth <= 0)
        {
            //Debug.Log(dragonName + " died");
            return true;
        }
        else
            return false;
    }

    public void Heal()
    {
        currentHealth = maxHealth;
    }
}
