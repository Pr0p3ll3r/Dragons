using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon", menuName = "ScriptableObjects/Dragon")]
public class DragonInfo : ScriptableObject
{
    public int ID;
    public string dragonName = "";
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
    public int specialSkill = 0;

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
        for (int i = 0; i < equipment.Length; i++)
        {
            if (equipment[i] == null) continue;
            for (int j = 0; j < equipment[i].stats.Length; j++)
            {
                for (int k = 0; k < stats.Length; k++)
                {
                    if (equipment[i].stats[j].statType == stats[k].statType)
                        stats[k].AddModifier(equipment[i].stats[j].value);
                }            
            }
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

    public void AddEquipment(Equipment item, int numSlot)
    {
        equipment[numSlot] = item;
        foreach (ItemStat itemStat in item.stats)
        {
            foreach (Stat stat in stats)
            {
                if (itemStat.statType == stat.statType)
                    stat.AddModifier(itemStat.value);
            }
        }
    }

    public void RemoveEquipment(Equipment item, int numSlot)
    {
        equipment[numSlot] = null;
        foreach (ItemStat itemStat in item.stats)
        {
            foreach (Stat stat in stats)
            {
                if (itemStat.statType == stat.statType)
                    stat.RemoveModifier(itemStat.value);
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
