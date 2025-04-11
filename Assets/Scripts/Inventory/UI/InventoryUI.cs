using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject SlotPrefab;
    public Transform InventoryPanel;

    List<SlotUI> SlotsUI = new List<SlotUI>();
    private void Start()
    {
        ActionManager.ItemChanged += UpdateUi;
        for (int i = 0; i < inventory.capacity; i++)
        {
            var slot = Instantiate(SlotPrefab, InventoryPanel).GetComponent<SlotUI>();
            slot.Init(this, i);
            SlotsUI.Add(slot);
        }
        UpdateUi();
    }

    public void UpdateUi()
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
        var slot = inventory.inventory[i];
            SlotsUI[i].UpdateSlot(slot);
        }
    }
}
