using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Shoes", menuName = "Clothing Items/Shoes")]
    public class ShoesSO : ItemSO
    {
        public override ItemType type => ItemType.Shoes;

        [SerializeField] new string name;
        [TextArea]
        [SerializeField] string description;
        [SerializeField] Sprite sprite;

        public override string itemName => name;
        public override string itemDescription => description;
        public override Sprite inventorySprite => sprite;
    }
}
