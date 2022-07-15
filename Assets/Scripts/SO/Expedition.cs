using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Expedition", menuName = "ScriptableObjects/Expedition")]
public class Expedition : ScriptableObject
{
    public string placeName;
    public Sprite thumbnail;
    public int index = 0;
    public int level = 1;
    public int exp = 0;
    public ExpeditionData[] data;
    [HideInInspector] public ExpeditionData currentData;

    public void Initialize()
    {
        level = 1;
        exp = 0;
        currentData = data[0];
    }

    public void AddExp()
    {
        if (level == data.Length && exp == level * 2)
            return;

        exp++;
        if(exp >= level*2)
        {
            level++;
            currentData = data[level - 1];
            exp = 0;
        }
    }
}

[System.Serializable]
public class ExpeditionData
{
    public string name;
    public LootResource[] resources;
    public float expeditionTime;
    public LootItem[] loot;
}

[System.Serializable]
public class LootResource
{
    public Resource resource;
    public float dropChance;
}