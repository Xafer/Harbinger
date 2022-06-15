using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { HandHeldController, Key, PuzzlePiece, PlayerRobot }
    public ItemType Type;

    [SerializeField] private ItemUse _itemUse;
    
    public void Use()
    {
        if (_itemUse != null)
            _itemUse.UseItem();
    }
}
