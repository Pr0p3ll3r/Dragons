using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public string name;
    public int amount;
}

[System.Serializable]
public class Resources
{
    public Resource[] data;
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    
    public InventorySlot[] slots;
    public Resources resources = new Resources();
    [SerializeField] private Item[] startingItems;
    [SerializeField] private GameObject itemInfoPrefab;
    [SerializeField] private Transform canvas;
    [SerializeField] private TextMeshProUGUI[] resourcesUI;

    private GameObject currentItemInfo;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
        if(Data.NewGame())
        {
            for (int i = 0; i < startingItems.Length; i++)
            {
                AddItem(startingItems[i], 1);
            }
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                slots[i].ClearSlot();
            else
                slots[i].FillSlot(slots[i].item);
        }

        resourcesUI[0].text = resources.data[0].amount.ToString();
        resourcesUI[1].text = resources.data[1].amount.ToString();
        resourcesUI[2].text = resources.data[2].amount.ToString();
        resourcesUI[3].text = resources.data[3].amount.ToString();
        resourcesUI[4].text = resources.data[4].amount.ToString();
        resourcesUI[5].text = resources.data[5].amount.ToString();
        resourcesUI[6].text = resources.data[6].amount.ToString();
    }

    public bool AddItem(Item item, int amount)
    {
        if (IsFull()) return false;

        if (item.stackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == item)
                {
                    slot.AddAmount(amount);
                    RefreshUI();
                    return true;
                }
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                RefreshUI();
                return true;
            }
        }
        return false;
    }

    public void AddItemAtSlot(Item item, InventorySlot slot)
    {
        slot.item = item;
        RefreshUI();
    }

    public void RemoveItem(Item _item, int amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item)
            {
                int itemAmount = slot.amount;
                itemAmount = Mathf.Min(itemAmount, amount);
                amount -= itemAmount;
                slot.amount -= itemAmount;

                if (slot.amount <= 0) RemoveItem(slot.item);
                if (amount == 0) break;
            }
        }
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == _item)
            {
                slots[i].item = null;
            }
        }
        RefreshUI();
    }

    public bool IsFull()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool EnoughSlots(int needed)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null)
            {
                needed--;
            }
        }

        if (needed <= 0) return true;
        else return false;
    }

    public bool HaveItem(Item item, int amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                amount -= slot.amount;
            }
        }

        if (amount <= 0) return true;
        else return false;
    }

    public void SwapSlot(InventorySlot slot1, InventorySlot slot2)
    {
        Item temp = slot1.item;
        AddItemAtSlot(slot2.item, slot1);
        AddItemAtSlot(temp, slot2);
    }

    public void DisplayItemInfo(Item item, Vector2 position)
    {
        DestroyItemInfo();

        if (item == null) return;

        currentItemInfo = Instantiate(itemInfoPrefab, position, Quaternion.identity, canvas);
        currentItemInfo.GetComponent<ItemInfo>().SetUp(item);
    }

    public void DestroyItemInfo()
    {
        if (currentItemInfo != null)
        {
            Destroy(currentItemInfo);
        }
    }

    public void AddResource(string name, int _amount)
    {
        resources.data[GetIdByName(name)].amount += _amount;
        RefreshUI();
    }

    public bool CheckGold(int neededGold)
    {
        if (resources.data[GetIdByName("Gold")].amount >= neededGold)
        {
            resources.data[GetIdByName("Gold")].amount -= neededGold;
            return true;
        }
        else return false;
    }

    public bool CheckMaterials(Blueprint b)
    {
        foreach(Resource r in b.resources)
        {
            if (resources.data[GetIdByName(r.name)].amount < r.amount)
                return false;
        }

        foreach (Resource r in b.resources)
        {           
            resources.data[GetIdByName(r.name)].amount -= r.amount;        
        }

        RefreshUI();

        return true;
    }

    public int GetIdByName(string _name)
    {
        for (int i = 0; i < resources.data.Length; i++)
        {
            if (resources.data[i].name == _name)
                return i;
        }
        return -1;
    }
}
