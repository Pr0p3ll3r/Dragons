using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Transform resourceParent;
    [SerializeField] private GameObject resourcePrefab;

    public void SetUp(Blueprint b)
    {
        foreach (Resource r in b.resources)
        {
            ItemStatInfo stat = Instantiate(resourcePrefab, resourceParent).GetComponent<ItemStatInfo>();
            stat.SetUp($"{r.amount}", Database.database.items[Database.database.GetIdByName(r.name)].icon);

            if (Inventory.Instance.resources.data[Inventory.Instance.GetIdByName(r.name)].amount > r.amount)
            {
                stat.SetColor(Color.green);
            }
            else
            {
                stat.SetColor(Color.red);
            }
        }
    }
}
