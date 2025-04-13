using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Slot> inventory = new List<Slot>();
    public int capacity = 20;
    public int selectedSlot = 0;
    public float MaxWeight = 1;
    public float CurrentWeight;

    public void Awake()
    {
        for (int i = 0; i < capacity; i++) 
        {
            inventory.Add(new Slot());
        }
        ActionManager.ItemChanged += recalculationWeight;
    }

    private void recalculationWeight()
    {
        float a = 0;
        for (int i = 0; i < capacity; i++)
        {
            if(!inventory[i].isEmpty)
            a += inventory[i].item.Weight * inventory[i].amount;
        }
        CurrentWeight = a;
    }

    public bool AddItem(Item NewItem, int amount = 1, bool onlyEmptySlots = false)
    {
        if (!onlyEmptySlots)
        {
            foreach (var slot in inventory)
            {
                if (slot.isEmpty)
                    continue;

                if (slot.item.Id == NewItem.Id && slot.amount + amount <= slot.item.MaxStack && CurrentWeight + (NewItem.Weight * amount) <= MaxWeight)
                {
                    slot.amount += amount;
                    ActionManager.ItemChanged?.Invoke();
                    return true;
                }
            }
        }
        foreach(var slot in inventory)
        {
            if (slot.isEmpty && slot.amount <= NewItem.MaxStack && CurrentWeight + (NewItem.Weight * amount) <= MaxWeight)
            {
                slot.item = NewItem;
                slot.amount = amount;
                ActionManager.ItemChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
    public void RemoveItem(int slotIndex, int amount) 
    {
        if(slotIndex <= inventory.Count)
        {
            inventory[slotIndex].amount -= amount;
            if(inventory[slotIndex].amount <= 0)
            {
                inventory[slotIndex].ClearSlot();
            }
            ActionManager.ItemChanged?.Invoke();
        }
    }

    public void SwapItems(int index1, int index2)
    {
        if (index1 != index2)
        {
            if (inventory[index1]?.item?.Id == inventory[index2]?.item?.Id && !inventory[index1].isEmpty && !inventory[index2].isEmpty)
            {
                if (inventory[index1].amount + inventory[index2].amount <= inventory[index1].item.MaxStack)
                {
                    inventory[index2].amount += inventory[index1].amount;
                    inventory[index1].ClearSlot();
                    UnityEngine.Debug.Log($"предметы полностью сложились {index1} и {index2}");
                }
                else
                {
                    if (inventory[index1].amount > inventory[index2].amount)

                    {
                        inventory[index1].amount += inventory[index2].amount - inventory[index1].item.MaxStack;
                        inventory[index2].amount = inventory[index2].item.MaxStack;
                        UnityEngine.Debug.Log($"предметы не полностью сложились {index1} колво {inventory[index1].amount} и {index2}колво {inventory[index2].amount}");
                    } else
                    if (inventory[index1].amount < inventory[index2].amount)
                    {
                        inventory[index2].amount += inventory[index1].amount - inventory[index2].item.MaxStack;
                        inventory[index1].amount = inventory[index1].item.MaxStack;
                        UnityEngine.Debug.Log($"предметы не полностью сложились {index1} колво {inventory[index1].amount} и {index2}колво {inventory[index2].amount}");
                    }
                }
            }
            else
            {
                Slot item1 = new Slot();
                item1.amount = inventory[index1].amount;
                item1.item = inventory[index1].item;
                inventory[index1] = inventory[index2];
                inventory[index2] = item1;
                UnityEngine.Debug.Log($"поменялись местами {index1} и {index2}");
            }
            ActionManager.ItemChanged?.Invoke();
        }
    }
}
