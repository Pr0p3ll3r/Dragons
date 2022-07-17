using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "ScriptableObjects/Database")]
public class DatabaseSO : ScriptableObject, ISerializationCallbackReceiver
{
    public Item[] items;
    public DragonInfo[] dragons;
    public DragonInfo[] enemyDragons;
    public Expedition[] expeditions;

    public void OnBeforeSerialize()
    {     
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].ID = i;
        }
        for (int i = 0; i < dragons.Length; i++)
        {
            dragons[i].ID = i;
        }
        for (int i = 0; i < enemyDragons.Length; i++)
        {
            enemyDragons[i].ID = i;
        }
        for (int i = 0; i < expeditions.Length; i++)
        {
            expeditions[i].ID = i;
        }
    }

    public int GetIdByName(string _name)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == _name)
                return i;
        }
        return -1;
    }
}
