using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blueprint", menuName = "Items/Blueprint")]
public class Blueprint : Item
{
    public DragonInfo dragon;
    public DragonType type;
    public Resource[] resources;
}

public enum DragonType
{
    Rare,
    Epic,
    Legendary,
    Special
}