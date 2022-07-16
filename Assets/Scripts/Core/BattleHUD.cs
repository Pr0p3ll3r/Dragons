using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
	[SerializeField] private Image look;
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private TextMeshProUGUI damageText;
	[SerializeField] private TextMeshProUGUI defenseText;
	[SerializeField] private TextMeshProUGUI strengthText;
	[SerializeField] private TextMeshProUGUI vitalityText;
	[SerializeField] private TextMeshProUGUI luckText;
	[SerializeField] private Slider currentHealthBar;
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private Slider currentSpecialSkillBar;
    [SerializeField] private TextMeshProUGUI specialSkillText;

	public void SetHUD(DragonInfo dragon)
	{
		look.sprite = dragon.look;
		nameText.text = dragon.dragonName;
		levelText.text = "Lvl: " + dragon.level.ToString();
		damageText.text = "Damage: " + dragon.stats[(int)StatType.Damage].GetValue().ToString();
		defenseText.text = "Defense: " + dragon.stats[(int)StatType.Defense].GetValue().ToString();
		strengthText.text = "Strength: " + dragon.stats[(int)StatType.Strength].GetValue().ToString();
		vitalityText.text = "Vitality: " + dragon.stats[(int)StatType.Vitality].GetValue().ToString();
		luckText.text = "Luck: " + dragon.stats[(int)StatType.Luck].GetValue().ToString();
		UpdateGraphics(dragon);
	}

	public void UpdateHealthBar(DragonInfo dragon)
	{
		float ratio = (float)dragon.currentHealth / dragon.maxHealth;
		currentHealthBar.value = ratio;
		healthText.text = dragon.currentHealth.ToString("0") + "/" + dragon.maxHealth.ToString("0");
	}

	public void UpdateSkillBar(int specialSkill)
    {
		float ratio = (float)specialSkill / 100;
		currentSpecialSkillBar.value = ratio;
		specialSkillText.text = specialSkill.ToString("0") + "/100";
	}

	private void UpdateGraphics(DragonInfo dragon)
	{
		UpdateHealthBar(dragon);
		UpdateSkillBar(0);
	}
}
