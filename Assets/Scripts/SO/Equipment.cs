using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment")]
public class Equipment : Item
{
    public EquipmentType equipmentType;
    public EquipmentRarity rarity;
    public ItemStat[] stats;
    public int lvlRequired = 1;

    public override Item GetCopy()
    {
        return Instantiate(this);
    }
}

public enum EquipmentType
{
    Head,
    Chest,
    Paw,
    Tail
}

public enum EquipmentRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}