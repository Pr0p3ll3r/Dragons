using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dragon : MonoBehaviour
{
    public DragonInfo info;
    [SerializeField] private Button[] dragonButtons;
    [SerializeField] private Button[] statsButtons;
    [SerializeField] private TextMeshProUGUI[] statsPoint;
    [SerializeField] private GameObject statsTab;
    [SerializeField] private GameObject meat;
    private SpriteRenderer dragon;

    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    EquipmentSlot[] slots;

    [SerializeField] private Transform equipmentParent;
    private Inventory inventory;

    void Start()
    {
        dragon = GetComponent<SpriteRenderer>();
        dragon.sprite = info.look;
        statsButtons[0].onClick.AddListener(delegate { AddStrength(); });
        statsButtons[1].onClick.AddListener(delegate { AddVitality(); });
        statsButtons[2].onClick.AddListener(delegate { AddLuck(); });
        dragonButtons[0].onClick.AddListener(delegate { OpenStats(); });
        dragonButtons[1].onClick.AddListener(delegate { StartExpedition(); });
        dragonButtons[2].onClick.AddListener(delegate { HideDragon(); });
        dragonButtons[3].onClick.AddListener(delegate { OpenArena(); });
        dragon.enabled = true;
        inventory = Inventory.Instance;
        CheckEating();

        slots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();

        RefreshDragonStats();
    }

    public void StartEating()
    {
        info.canEat = false;
        info.LvlUp();
        info.Heal();
        CheckAdult();
        RefreshDragonStats();
        GameController.Instance.StartEating(info.eatingTime, info);
    }

    public void CheckEating()
    {
        if(info.canEat)
        {
            meat.GetComponent<SpriteRenderer>().enabled = true;
            meat.GetComponent<Collider2D>().enabled = true;
        }
        else
        {
            meat.GetComponent<SpriteRenderer>().enabled = false;
            meat.GetComponent<Collider2D>().enabled = false;
        }
    }

    void StartExpedition()
    {
        GameController.Instance.ShowExpeditionPanel(gameObject, info);
    }

    void OpenStats()
    {
        RefreshDragonStats();
        statsTab.SetActive(!statsTab.activeSelf);
    }

    void HideDragon()
    {
        info.shown = false;
        GameController.Instance.RefreshDragonList();
        Destroy(gameObject);
    }

    void AddStrength()
    {
        info.AddStrength();
        RefreshDragonStats();
    }

    void AddVitality()
    {
        info.AddVitality();
        RefreshDragonStats();
    }

    void AddLuck()
    {
        info.AddLuck();
        RefreshDragonStats();
    }

    void CheckPoints()
    {
        statsPoint[0].text = info.stats[(int)StatType.Strength].GetValue().ToString();
        statsPoint[1].text = info.stats[(int)StatType.Vitality].GetValue().ToString();
        statsPoint[2].text = info.stats[(int)StatType.Luck].GetValue().ToString();

        if (info.remainPoints > 0)
        {
            foreach(Button b in statsButtons)
            {
                b.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Button b in statsButtons)
            {
                b.gameObject.SetActive(false);
            }
        }
    }

    void RefreshDragonStats()
    {
        nameUI.text = info.dragonName;
        level.text = info.level.ToString();
        damage.text = info.stats[(int)StatType.Damage].GetValue().ToString();
        defense.text = info.stats[(int)StatType.Defense].GetValue().ToString();
        HealthBar();
        CheckPoints();
        CheckAdult();
        RefreshDragonEquipment();
    }

    void RefreshDragonEquipment()
    {
        for (int i = 0; i < info.equipment.Length; i++)
        {
            if(info.equipment[i] != null)
                slots[i].FillSlot(info.equipment[i]);
            else
                slots[i].ClearSlot();
        }
    }

    public void EquipItem(Equipment item, out Equipment previousItem)
    {
        for (int i = 0; i < info.equipment.Length; i++)
        {
            if (slots[i].equipmentType == item.equipmentType)
            {
                previousItem = slots[i].item;
                slots[i].item = item;
                info.equipment[i] = item;
                Debug.Log("Equipped Item: " + item.itemName);
                return;
            }
        }
        previousItem = null;
    }

    public void UnequipItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].item = null;
                info.equipment[i] = null;
                Debug.Log("Unequipped Item: " + item.itemName);
            }
        }
    }

    public bool Equip(Equipment item, InventorySlot slot)
    {
        Equipment previousItem;
        if (item.lvlRequired <= info.level)
        {
            Inventory.Instance.RemoveItem(item);
            EquipItem(item, out previousItem);
            if (previousItem != null)
            {
                inventory.AddItemAtSlot(previousItem, slot);
            }
        }
        else
        {
            GameController.Instance.ShowText("Level is too low", new Color32(255, 65, 52, 255));
            return false;
        }
        RefreshDragonStats();
        return true;
    }

    public bool Unequip(Equipment item, InventorySlot slot)
    {
        if (slot != null)
        {
            UnequipItem(item);
            inventory.AddItemAtSlot(item, slot);
        }
        else if (!inventory.IsFull())
        {
            UnequipItem(item);
            inventory.AddItem(item, 1);
        }
        else
        {
            GameController.Instance.ShowText("Inventory is full", new Color32(255, 65, 52, 255));
            return false;
        }
        RefreshDragonStats();
        return true;
    }

    void HealthBar()
    {
        healthText.text = $"{info.currentHealth}/{info.maxHealth}";
        float percentage = (float)info.currentHealth / info.maxHealth;
        healthBar.value = percentage;
    }

    public void CheckAdult()
    {
        if(!info.isAdult || info.currentHealth <= 0)
        {
            dragonButtons[3].interactable = false;
        }
        else
        {
            dragonButtons[3].interactable = true;
        }
    }

    void OpenArena()
    {
        GameController.Instance.ShowArena(gameObject);
    }
}
