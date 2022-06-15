using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> _inventoryContent;
    public bool HasPlayerRobot { get; set; }

    public Inventory()
    {
        _inventoryContent = new List<Item>();
    }

    public void AddItem(Item item)
    {
        _inventoryContent.Add(item);
    }

    public Item CycleRight(Item heldItem)
    {
        Item firstItem = _inventoryContent[0];

        _inventoryContent.Remove(firstItem);
        _inventoryContent.Add(heldItem);

        return firstItem;
    }

    public Item CycleLeft(Item heldItem)
    {
        Item lastItem = _inventoryContent[_inventoryContent.Count-1];
        _inventoryContent.Remove(lastItem);

        List<Item> newList = new List<Item> { heldItem };
        newList.AddRange(_inventoryContent);
        _inventoryContent = newList;

        return lastItem;
    }

    public int GetInventoryCount()
    {
        return _inventoryContent.Count;
    }
}
