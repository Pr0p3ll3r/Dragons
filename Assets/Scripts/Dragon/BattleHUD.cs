using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Image look;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI defenseText;
	public TextMeshProUGUI strengthText;
	public TextMeshProUGUI vitalityText;
	public TextMeshProUGUI luckText;
	public Slider currentHealthBar;
    public TextMeshProUGUI healthText;
    public Slider currentSpecialSkillBar;
    public TextMeshProUGUI specialSkillText;

	public void SetHUD(DragonInfo dragon)
	{
		look.sprite = dragon.look;
		nameText.text = dragon.dragonName;
		levelText.text = "Lvl: " + dragon.level.ToString();
		damageText.text = "Damage: " + dragon.stats[(int)StatType.Damage].ToString();
		defenseText.text = "Defense: " + dragon.stats[(int)StatType.Defense].ToString();
		strengthText.text = "Strength: " + dragon.stats[(int)StatType.Strength].ToString();
		vitalityText.text = "Vitality: " + dragon.stats[(int)StatType.Vitality].ToString();
		luckText.text = "Luck: " + dragon.stats[(int)StatType.Luck].ToString();
		UpdateGraphics(dragon);
	}

	public void UpdateHealthBar(DragonInfo dragon)
	{
		float ratio = (float)dragon.currentHealth / dragon.maxHealth;
		currentHealthBar.value = ratio;
		healthText.text = dragon.currentHealth.ToString("0") + "/" + dragon.maxHealth.ToString("0");
	}

	public void UpdateSkillBar(DragonInfo dragon)
    {
		float ratio = (float)dragon.specialSkill / 100;
		currentSpecialSkillBar.value = ratio;
		specialSkillText.text = dragon.specialSkill.ToString("0") + "/100";
	}

	private void UpdateGraphics(DragonInfo dragon)
	{
		UpdateHealthBar(dragon);
		UpdateSkillBar(dragon);
	}
}
