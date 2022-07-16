using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dragon : MonoBehaviour
{
    public DragonInfo dragon;
    [SerializeField] private Button[] dragonButtons;
    [SerializeField] private Button[] statsButtons;
    [SerializeField] private TextMeshProUGUI[] statsPoint;
    [SerializeField] private GameObject statsTab;
    [SerializeField] private GameObject meat;

    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    EquipmentSlot[] slots;

    [SerializeField] private Transform equipmentParent;
    private Inventory inventory;
    private GameObject graphics;

    void Start()
    {
        if(dragon.isAdult)
            graphics = Instantiate(dragon.prefabAdult, transform);
        else
            graphics = Instantiate(dragon.prefabBaby, transform);
        statsButtons[0].onClick.AddListener(delegate { AddStrength(); });
        statsButtons[1].onClick.AddListener(delegate { AddVitality(); });
        statsButtons[2].onClick.AddListener(delegate { AddLuck(); });
        dragonButtons[0].onClick.AddListener(delegate { OpenStats(); });
        dragonButtons[1].onClick.AddListener(delegate { StartExpedition(); });
        dragonButtons[2].onClick.AddListener(delegate { HideDragon(); });
        dragonButtons[3].onClick.AddListener(delegate { OpenArena(); });
        inventory = Inventory.Instance;
        CheckEating();

        slots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();

        RefreshDragonStats();
    }

    public void StartEating()
    {
        dragon.canEat = false;
        if(!dragon.isAdult)
        {
            dragon.LvlUp();
            if(dragon.isAdult)
            {
                Destroy(graphics);
                graphics = Instantiate(dragon.prefabAdult, transform);
            }
        }
        else
        {
            dragon.LvlUp();
        }      
        dragon.Heal();
        CheckAdult();
        RefreshDragonStats();
        GameController.Instance.StartEating(dragon.eatingTime, dragon);
    }

    public void CheckEating()
    {
        if(dragon.canEat)
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
        GameController.Instance.ShowExpeditionPanel(dragon);
    }

    void OpenStats()
    {
        RefreshDragonStats();
        statsTab.SetActive(!statsTab.activeSelf);
    }

    void HideDragon()
    {
        GameController.Instance.HideDragon();   
        GameController.Instance.RefreshDragonList();
    }

    void AddStrength()
    {
        dragon.AddStrength();
        RefreshDragonStats();
    }

    void AddVitality()
    {
        dragon.AddVitality();
        RefreshDragonStats();
    }

    void AddLuck()
    {
        dragon.AddLuck();
        RefreshDragonStats();
    }

    void CheckPoints()
    {
        statsPoint[0].text = dragon.stats[(int)StatType.Strength].GetValue().ToString();
        statsPoint[1].text = dragon.stats[(int)StatType.Vitality].GetValue().ToString();
        statsPoint[2].text = dragon.stats[(int)StatType.Luck].GetValue().ToString();

        if (dragon.remainPoints > 0)
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
        nameUI.text = dragon.dragonName;
        level.text = dragon.level.ToString();
        damage.text = dragon.stats[(int)StatType.Damage].GetValue().ToString();
        defense.text = dragon.stats[(int)StatType.Defense].GetValue().ToString();
        HealthBar();
        CheckPoints();
        CheckAdult();
        RefreshDragonEquipment();
    }

    void RefreshDragonEquipment()
    {
        for (int i = 0; i < dragon.equipment.Length; i++)
        {
            if(dragon.equipment[i] != null)
                slots[i].FillSlot(dragon.equipment[i]);
            else
                slots[i].ClearSlot();
        }
    }

    public void EquipItem(Equipment item, out Equipment previousItem)
    {
        for (int i = 0; i < dragon.equipment.Length; i++)
        {
            if (slots[i].equipmentType == item.equipmentType)
            {
                previousItem = slots[i].item;
                slots[i].item = item;
                dragon.equipment[i] = item;
                dragon.AddStats(item);
                Debug.Log("Equipped Item: " + item.itemName);
                return;
            }
        }
        previousItem = null;
    }

    public void UnequipItem(Equipment item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].item = null;
                dragon.equipment[i] = null;
                dragon.RemoveStats(item);
                Debug.Log("Unequipped Item: " + item.itemName);
            }
        }
    }

    public bool Equip(Equipment item, InventorySlot slot)
    {
        Equipment previousItem;
        if (item.lvlRequired <= dragon.level)
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
        healthText.text = $"{dragon.currentHealth}/{dragon.maxHealth}";
        float percentage = (float)dragon.currentHealth / dragon.maxHealth;
        healthBar.value = percentage;
    }

    public void CheckAdult()
    {
        if(!dragon.isAdult || dragon.currentHealth <= 0)
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
        GameController.Instance.ShowArena();
    }
}
