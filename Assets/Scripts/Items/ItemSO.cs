using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    public enum ItemType { Hat, Shirt, Pants, Shoes, Misc }

    public abstract class ItemSO : ScriptableObject
    {
        public abstract string itemName { get; }
        public abstract string itemDescription { get; }
        public abstract ItemType type { get; }
        public abstract Sprite inventorySprite { get; }
    }
}
