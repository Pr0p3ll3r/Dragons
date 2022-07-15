using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
	public int ID;
	public string itemName;
	public Sprite icon;
	public ItemType itemType;
	[TextArea(4, 6)] public string description;
	public int maxInStack = 999;
	public bool stackable;

	public virtual Item GetCopy()
	{
		return this;
	}
}

public enum ItemType
{
	Default,
	Equipment,
	Blueprint	
}

[System.Serializable]
public class LootItem
{
	public Item item;
	public float dropChance;
}

[System.Serializable]
public class ItemStat
{
	public StatType statType;
	public int value;
	public int min;
	public int max;
	public ItemStat(int _min, int _max)
	{
		min = _min;
		max = _max;
	}
	public void GenerateValue()
	{
		value = Random.Range(min, max);
	}
}
