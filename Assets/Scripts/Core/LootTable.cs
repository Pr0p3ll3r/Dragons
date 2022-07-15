using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootTable 
{
    public static List<Item> Drop(LootItem[] possibleLoot)
    {
        List<Item> droppedItems = new List<Item>();
        foreach(LootItem loot in possibleLoot)
        {
            int roll = Random.Range(0, 100);

            if(roll <= loot.dropChance)
            {
                var item = loot.item.GetCopy();
                if(item.itemType == ItemType.Equipment)
                {
                    Equipment equipment = (Equipment)item;
                    foreach (ItemStat stat in equipment.stats)
                    {
                        stat.GenerateValue();
                    }
                }
                droppedItems.Add(item);
            }
        }
        return droppedItems;
    }

    public static List<Resource> Drop(LootResource[] possibleLoot)
    {
        List<Resource> droppedItems = new List<Resource>();
        foreach (LootResource loot in possibleLoot)
        {
            int roll = Random.Range(0, 100);

            if (roll <= loot.dropChance)
            {
                droppedItems.Add(loot.resource);
            }
        }
        return droppedItems;
    }
}
