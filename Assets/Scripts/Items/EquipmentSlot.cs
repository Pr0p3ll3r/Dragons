using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private GameObject itemIcon;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject placeholderIcon;

    public Equipment item;
    public EquipmentType equipmentType;

    public bool CanPlaceInSlot(Equipment _item)
    {
        if (_item.equipmentType == equipmentType)
            return true;
        return false;
    }

    public void FillSlot(Equipment _item)
    {
        item = _item;
        placeholderIcon.SetActive(false);
        icon.sprite = Database.database.items[_item.ID].icon;
        itemIcon.SetActive(true);
    }

    public void ClearSlot()
    {
        item = null;
        placeholderIcon.SetActive(true);
        icon.sprite = null;
        itemIcon.SetActive(false);
    }
}
