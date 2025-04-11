using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToDragUI : MonoBehaviour, IPointerClickHandler
{
    private ItemMenu itemMenu;
    private PickupItem _item;
    private RectTransform rectTransform;
    private Transform inventoryWindow;
    private bool isDragging = false;
    private SlotUI slotUi;
    private Transform originalParent;
    private Image itemImage;

    void Awake()
    {
        itemMenu = FindFirstObjectByType<ItemMenu>(FindObjectsInactive.Include);
        _item = FindFirstObjectByType<PickupItem>();
        ActionManager.inventoryClosed += OnInventoryClosed;
        rectTransform = GetComponent<RectTransform>();
        inventoryWindow = transform.parent.parent.parent.parent;
        slotUi = GetComponentInParent<SlotUI>();
        originalParent = transform.parent;
        itemImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            LKM(eventData);
            itemMenu.gameObject.SetActive(false);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            PKM(eventData);
        } else itemMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDragging)
        {
            // Обновляем позицию объекта
            rectTransform.position = Input.mousePosition;
        }
    }

    private GameObject FindSlotUnderCursor()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name == "Panel")
            {
                return result.gameObject;
            }
        }
        return null;
    }

    private bool FindInventoryUnderCursor()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name == "InventoryWindow")
            {
                return true;
            }
        }
        return false;
    }
    private void OnInventoryClosed()
    {
        isDragging = false;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
        itemMenu.gameObject.SetActive(false);
    }

    private void LKM(PointerEventData eventData)
    {
        if (!slotUi.inventoryUI.inventory.inventory[slotUi.index].isEmpty)
        {
            if (!isDragging)
            {
                // Первый клик — начинаем перетаскивание
                isDragging = true;
                itemImage.raycastTarget = false; // Отключаем блокировку кликов
                transform.SetParent(inventoryWindow.transform); // Поднимаем на верхний уровень
                rectTransform.position = eventData.position; // Центрируем на курсоре
            }
            else
            {
                // Второй клик — завершаем перетаскивание
                isDragging = false;
                itemImage.raycastTarget = true; // Включаем Raycast обратно

                // Ищем ячейку под курсором
                GameObject targetSlot = FindSlotUnderCursor();
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = Vector2.zero;

                if (targetSlot != null)
                {
                    // Прикрепляем к ячейке
                    SlotUI OtherItem = targetSlot.GetComponentInParent<SlotUI>();
                    slotUi.inventoryUI.inventory.SwapItems(slotUi.index, OtherItem.index);
                }
                else if (!FindInventoryUnderCursor())
                {
                    _item.ThrowItem(slotUi.index);
                }
            }
        }
    }

    private void PKM(PointerEventData eventData)
    {
        if (!isDragging) {
            GameObject targetSlot = FindSlotUnderCursor();
            if (!targetSlot.GetComponentInParent<SlotUI>().inventoryUI.inventory.inventory[targetSlot.GetComponentInParent<SlotUI>().index].isEmpty)
            {
                Debug.Log(targetSlot.GetComponentInParent<SlotUI>().index);
                itemMenu.gameObject.SetActive(true);
                itemMenu.transform.position = new Vector2(eventData.position.x + 50, eventData.position.y);
                Debug.Log(slotUi.index);
                itemMenu.ChangeIndex(slotUi.index);
            }
        }
    }
}