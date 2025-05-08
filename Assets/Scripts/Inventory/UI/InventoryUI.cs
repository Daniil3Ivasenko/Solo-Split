using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject SlotPrefab;
    public Transform InventoryPanel;
    public Transform HotBarPanel;
    public TMP_Text WeightText;

    List<SlotUI> SlotsUI = new List<SlotUI>();
    private void Start()
    {
        ActionManager.ItemChanged += UpdateUi;
        for (int i = 0; i < inventory.capacity; i++)
        {
            if(i < 10)
            {
                var slot = Instantiate(SlotPrefab, HotBarPanel).GetComponent<SlotUI>();
                slot.Init(this, i);
                SlotsUI.Add(slot);
            }
            else
            {
                var slot = Instantiate(SlotPrefab, InventoryPanel).GetComponent<SlotUI>();
                slot.Init(this, i);
                SlotsUI.Add(slot);
            }
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
        WeightText.text = $"{inventory.CurrentWeight}/{inventory.MaxWeight}\nKg";
        WeightText.color = inventory.overWeightColor;
    }
}
