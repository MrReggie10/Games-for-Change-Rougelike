using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Shirt", menuName = "Clothing Items/Shirt")]
    public class ShirtSO : ItemSO
    {
        public override ItemType type => ItemType.Shirt;

        [SerializeField] new string name;
        [TextArea]
        [SerializeField] string description;
        [SerializeField] Sprite sprite;

        public override string itemName => name;
        public override string itemDescription => description;
        public override Sprite inventorySprite => sprite;
    }
}
