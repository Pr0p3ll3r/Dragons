using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool inventory;
    [SerializeField] private Vector2 offset;

    private Item draggingItem;
    private InventorySlot invSlot;
    private EquipmentSlot eqSlot;
    private GameObject mouseObject;
    private GameController gameController;

    private void Start()
    {
        if (inventory)
            invSlot = GetComponentInParent<InventorySlot>();
        else
            eqSlot = GetComponentInParent<EquipmentSlot>();

        gameController = GameController.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) return;
        Inventory.Instance.DestroyItemInfo();
        mouseObject = new GameObject();
        RectTransform rt = mouseObject.AddComponent<RectTransform>();
        rt.localScale = new Vector2(75, 75);
        mouseObject.transform.SetParent(transform.parent);
        Image image = mouseObject.AddComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;
        if (invSlot != null)
        {
            image.sprite = Database.database.items[invSlot.item.ID].icon;
            draggingItem = invSlot.item;
        }
        else
        {
            image.sprite = Database.database.items[eqSlot.item.ID].icon;
            draggingItem = eqSlot.item;
        }

        mouseObject.AddComponent<Canvas>().overrideSorting = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mouseObject)
        {
            mouseObject.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        EquipmentSlot eqSlotTemp = null;
        InventorySlot invSlotTemp = null;
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.GetComponent<EquipmentSlot>() != null)
            {
                eqSlotTemp = results[i].gameObject.GetComponent<EquipmentSlot>();
            }

            if (results[i].gameObject.GetComponent<InventorySlot>() != null)
            {
                invSlotTemp = results[i].gameObject.GetComponent<InventorySlot>();
            }
        }

        if (inventory) //drag from inventory
        {
            if (gameController.currentDragon != null && eqSlotTemp != null) //drag from inventory to equipment
            {
                Equipment eq = (Equipment)draggingItem;
                if (eq.equipmentType == eqSlotTemp.equipmentType)
                {
                    gameController.currentDragon.Equip(eq, invSlot);
                }
                else
                {
                    GameController.Instance.ShowText("Wrong slot", new Color32(255, 65, 52, 255));
                }
            }
            else if (invSlotTemp != null) //switch inventory slots
            {
                //Debug.Log("Moved Item");
                Inventory.Instance.SwapSlot(invSlot, invSlotTemp);
                //show correct item info after swap slots
                Inventory.Instance.DisplayItemInfo(invSlotTemp.item, invSlotTemp.transform.position + new Vector3(offset.x, offset.y));
            }
        }
        else //drag from equipment
        {
            if (invSlotTemp != null)
            {
                //inventory slot is not empty and has the same type of equipment
                if (invSlotTemp.item != null && invSlotTemp.item.itemType == ItemType.Equipment)
                {
                    Equipment itemInInv = (Equipment)invSlotTemp.item;
                    if (itemInInv.equipmentType == eqSlot.item.equipmentType)
                        gameController.currentDragon.Equip(itemInInv, invSlotTemp);
                }
                else //inventory slot is empty
                {
                    gameController.currentDragon.Unequip((Equipment)draggingItem, invSlotTemp);
                }
            }
        }
        Destroy(mouseObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventory)
        {
            if (invSlot.item == null) return;
            Inventory.Instance.DisplayItemInfo(invSlot.item, transform.position + new Vector3(offset.x, offset.y));
        }
        else
        {
            if (eqSlot.item == null) return;
            Inventory.Instance.DisplayItemInfo(eqSlot.item, transform.position + new Vector3(offset.x, offset.y));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Inventory.Instance.DestroyItemInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inventory)
            {
                if (invSlot.item.itemType == ItemType.Equipment)
                {
                    Equipment eq = (Equipment)invSlot.item;
                    gameController.currentDragon.Equip(eq, invSlot);
                    Inventory.Instance.DisplayItemInfo(invSlot.item, invSlot.transform.position + new Vector3(offset.x, offset.y));
                }
            }
            else
            {
                if (gameController.currentDragon != null && gameController.currentDragon.Unequip(eqSlot.item, null))
                    Inventory.Instance.DestroyItemInfo();
            }
        }
    }
}
