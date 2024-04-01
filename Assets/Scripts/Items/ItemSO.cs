using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    public enum ItemType { Hat, Shirt, Pants, Shoes, Misc }

    public abstract class ItemSO : ScriptableObject
    {
        public abstract new string name { get; }
        public virtual string info => $"Type: {type}";
        public abstract string description { get; }
        public abstract ItemType type { get; }
        public abstract Sprite inventorySprite { get; }
        //if we want to have the drop sprite be a simplified icon, we can override this
        public virtual Sprite dropSprite => inventorySprite;

        public abstract bool Use(PlayerStats player, PlayerInventory inventory);
    }
}
