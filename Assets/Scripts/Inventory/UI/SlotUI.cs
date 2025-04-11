using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amount;
    public int index;
    public InventoryUI inventoryUI;

    public void Init(InventoryUI ui, int slotindex)
    {
        inventoryUI = ui;
        index = slotindex;
    }

    public void UpdateSlot(Slot slot)
    {
        icon.sprite = slot.isEmpty ? null : slot.item.Icon;
        icon.gameObject.SetActive(!slot.isEmpty);
        amount.text = slot.isEmpty ? "" : slot.amount.ToString();
    }
}
