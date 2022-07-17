using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaDragon : MonoBehaviour
{
    [SerializeField] private Image look;
    [SerializeField] private TextMeshProUGUI dragonName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI vitality;
    [SerializeField] private TextMeshProUGUI luck;

    public void SetUp(int i)
    {
        DragonInfo enemy = Database.database.enemyDragons[i];
        look.sprite = enemy.lookAdult;
        dragonName.text = enemy.dragonName;
        level.text = enemy.level.ToString();
        health.text = enemy.maxHealth.ToString();
        damage.text = enemy.stats[(int)StatType.Damage].GetValue().ToString();
        defense.text = enemy.stats[(int)StatType.Defense].GetValue().ToString();
        strength.text = enemy.stats[(int)StatType.Strength].GetValue().ToString();
        vitality.text = enemy.stats[(int)StatType.Vitality].GetValue().ToString();
        luck.text = enemy.stats[(int)StatType.Luck].GetValue().ToString();
    }
}
