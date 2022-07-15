using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpeditionInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI expeditionName;

    [SerializeField] private Transform dropped;
    [SerializeField] private Transform possible;
    [SerializeField] private GameObject itemPrefab;

    public void SetUp(Expedition e)
    {
        expeditionName.text = e.placeName;
        foreach (LootResource r in e.currentData.resources)
        {
            ItemStatInfo rarity = Instantiate(itemPrefab, possible).GetComponent<ItemStatInfo>();
            rarity.SetUp($"{r.resource.amount} ({r.dropChance}%)", Database.database.items[Database.database.GetIdByName(r.resource.name)].icon);
        }
        foreach (LootItem i in e.currentData.loot)
        {
            ItemStatInfo rarity = Instantiate(itemPrefab, possible).GetComponent<ItemStatInfo>();
            rarity.SetUp($"1 ({i.dropChance}%)", i.item.icon);
        }
    }

    public void SetUp(Expedition e, List<Item> items, List<Resource> resources)
    {
        expeditionName.text = e.placeName;
        foreach (Resource r in resources)
        {
            ItemStatInfo rarity = Instantiate(itemPrefab, dropped).GetComponent<ItemStatInfo>();
            rarity.SetUp($"{r.amount}", Database.database.items[Database.database.GetIdByName(r.name)].icon);
        }
        foreach (Item i in items)
        {
            ItemStatInfo rarity = Instantiate(itemPrefab, dropped).GetComponent<ItemStatInfo>();
            rarity.SetUp($"1", i.icon);
        }
    }
}
