using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitInventory
{
    private UnitData unitData;

    public UnitInventory(UnitData unit)
    {
        unitData = unit;
    }

    public void InitializeItemQuantities()
    {
        unitData.itemQuantities.Clear();
        if (unitData.itemsData != null)
        {
            foreach (var item in unitData.itemsData.items)
            {
                unitData.itemQuantities.Add(new ItemQuantity { itemIndex = unitData.itemsData.items.IndexOf(item), quantity = item.quantity });
            }
        }
        else
        {
            Debug.LogError("ItemsData is null");
        }
    }

    public List<ItemData> GetItems()
    {
        List<ItemData> selectedItems = new List<ItemData>();
        if (unitData.itemsData == null)
        {
            Debug.LogWarning("ItemsData is null, can't get items");
            return selectedItems;
        }
        foreach (var itemQuantity in unitData.itemQuantities)
        {
            if (itemQuantity.itemIndex >= 0 && itemQuantity.itemIndex < unitData.itemsData.items.Count && itemQuantity.quantity > 0)
            {
                selectedItems.Add(unitData.itemsData.items[itemQuantity.itemIndex]);
            }
        }
        return selectedItems;
    }

    public int GetItemQuantity(ItemData item)
    {
        if (unitData.itemsData == null || item == null) return 0;
        int itemIndex = unitData.itemsData.items.IndexOf(item);
        foreach (var itemQuantity in unitData.itemQuantities)
        {
            if (itemQuantity.itemIndex == itemIndex)
            {
                return itemQuantity.quantity;
            }
        }
        return 0;
    }

    public void SetItemQuantity(ItemData item, int quantity)
    {
        if (unitData.itemsData == null || item == null) return;
        int itemIndex = unitData.itemsData.items.IndexOf(item);
        foreach (var itemQuantity in unitData.itemQuantities)
        {
            if (itemQuantity.itemIndex == itemIndex)
            {
                itemQuantity.quantity = quantity;
                return;
            }
        }

        unitData.itemQuantities.Add(new ItemQuantity { itemIndex = itemIndex, quantity = quantity });
    }

    public void UseItem(ItemData item, UnitData playerUnit = null)
    {
        if (item != null)
        {
            if (playerUnit != null && unitData.gameObject.CompareTag("Companion"))
            {
                if (playerUnit.GetItemQuantity(item) > 0)
                {
                    int quantity = playerUnit.GetItemQuantity(item) - 1;
                    playerUnit.SetItemQuantity(item, quantity);
                    Debug.Log($"{unitData.unitName} used {item.itemName}. Remaining quantity: {quantity} on Player's inventory");
                    if (quantity <= 0)
                    {
                        Debug.Log($"{item.itemName} has 0 quantity, item removed from Player's inventory");
                    }
                }
                else
                {
                    Debug.LogWarning($"{unitData.unitName} cant use {item.itemName} because quantity is 0");
                }
            }
            else
            {
                if (GetItemQuantity(item) > 0)
                {
                    int quantity = GetItemQuantity(item) - 1;
                    SetItemQuantity(item, quantity);
                    Debug.Log($"{unitData.unitName} used {item.itemName}. Remaining quantity: {quantity}");
                    if (quantity <= 0)
                    {
                        Debug.Log($"{item.itemName} has 0 quantity, item removed from inventory");
                    }
                }
                else
                {
                    Debug.LogWarning($"{unitData.unitName} cant use {item.itemName} because quantity is {GetItemQuantity(item)}");
                }
            }
        }
        else
        {
            Debug.LogWarning("Item is null, can't use item");
        }
    }

    public void SetSelectedItems(List<ItemData> newItems)
    {
        unitData.itemQuantities.Clear();
        if (unitData.itemsData == null)
        {
            Debug.LogWarning("ItemsData is null, can't set items");
            return;
        }
        foreach (var item in newItems)
        {
            if (unitData.itemsData != null && unitData.itemsData.items.Contains(item))
            {
                unitData.itemQuantities.Add(new ItemQuantity { itemIndex = unitData.itemsData.items.IndexOf(item), quantity = item.quantity });
            }
            else
            {
                Debug.LogWarning($"Item '{item.itemName}' not found in ItemsData");
            }
        }
    }
}