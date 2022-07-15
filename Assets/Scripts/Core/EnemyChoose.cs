using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyChoose
{
    public static DragonInfo[] Choose(int level)
    {
        DragonInfo[] enemies = new DragonInfo[3];

        DragonInfo enemy = ScriptableObject.CreateInstance<DragonInfo>();
        enemy.Initialize();

        enemies[0] = enemy;

        return enemies;
    }

    static void StatsChoose(DragonInfo dragon)
    {

    }
}
