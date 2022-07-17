using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private Transform statParent;
    [SerializeField] private GameObject statPrefab;
    [SerializeField] private Transform resourceParent;
    [SerializeField] private GameObject resourcePrefab;

    public void SetUp(Item item)
    {
        itemName.text = item.itemName;
        description.text = item.description;
        if (item.description == "")
            description.gameObject.SetActive(false);
        if (item.itemType == ItemType.Equipment)
        {
            Dragon dragon = GameController.Instance.currentDragon;
            Equipment eq = (Equipment)item;
            Equipment currentEquipment = null;
            if (dragon != null)
                currentEquipment = dragon.dragon.equipment[(int)eq.equipmentType];
            ItemStatInfo rarity = Instantiate(statPrefab, statParent).GetComponent<ItemStatInfo>();
            rarity.SetUp($"Rarity: {eq.rarity}");
            switch (eq.rarity)
            {
                case EquipmentRarity.Common:
                    rarity.SetColor(Color.white);
                    break;
                case EquipmentRarity.Uncommon:
                    rarity.SetColor(Color.green);
                    break;
                case EquipmentRarity.Rare:
                    rarity.SetColor(Color.yellow);
                    break;
                case EquipmentRarity.Epic:
                    rarity.SetColor(new Color(30, 115, 232, 255));
                    break;
                case EquipmentRarity.Legendary:
                    rarity.SetColor(new Color32(255, 165, 0, 255));
                    break;
            }
            ItemStatInfo reqLvl = Instantiate(statPrefab, statParent).GetComponent<ItemStatInfo>();
            reqLvl.SetUp($"Lvl Required: {eq.lvlRequired}");
            if (dragon != null)
            {
                if (dragon.dragon.level < eq.lvlRequired)
                {
                    reqLvl.SetColor(Color.red);
                }
                else
                {
                    reqLvl.SetColor(Color.green);
                }

            }

            foreach (ItemStat s in eq.stats)
            {
                ItemStatInfo stat = Instantiate(statPrefab, statParent).GetComponent<ItemStatInfo>();
                stat.SetUp($"{s.statType}: {s.value}");

                if (dragon == null || currentEquipment == null) continue;

                stat.SetColor(Color.green);

                for (int i = 0; i < currentEquipment.stats.Length; i++)
                {
                    if (s.statType == currentEquipment.stats[i].statType)
                    {
                        if (s.value > currentEquipment.stats[i].value)
                        {
                            stat.SetColor(Color.green);
                        }
                        else if (s.value < currentEquipment.stats[i].value)
                        {
                            stat.SetColor(Color.red);
                        }
                        else
                        {
                            stat.SetColor(Color.white);
                        }
                        break;
                    }
                }
            }
        }
        else if(item.itemType == ItemType.Blueprint)
        {
            Blueprint blueprint = (Blueprint)item;
            ItemStatInfo stat = Instantiate(statPrefab, statParent).GetComponent<ItemStatInfo>();
            stat.SetUp($"Dragon Type: {blueprint.type}");
            foreach (Resource r in blueprint.resources)
            {
                ItemStatInfo resource = Instantiate(resourcePrefab, resourceParent).GetComponent<ItemStatInfo>();
                resource.SetUp($"{r.amount}", Database.database.items[Database.database.GetIdByName(r.name)].icon);

                if (Inventory.Instance.resources.data[Inventory.Instance.GetIdByName(r.name)].amount > r.amount)
                {
                    resource.SetColor(Color.green);
                }
                else
                {
                    resource.SetColor(Color.red);
                }
            }
        }
    }
}