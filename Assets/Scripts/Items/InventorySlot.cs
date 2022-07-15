using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantity;

    public Item item;
    public int amount;

    public void AddAmount(int _amount)
    {
        amount += _amount;
    }

    public void FillSlot(Item _item)
    {
        item = _item;
        quantity.text = amount.ToString();
        icon.sprite = Database.database.items[item.ID].icon;
        icon.enabled = true;

        if (amount <= 1) quantity.gameObject.SetActive(false);
        else quantity.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        item = null;
        quantity.text = "";
        icon.sprite = null;
        icon.enabled = false;
    }
}
