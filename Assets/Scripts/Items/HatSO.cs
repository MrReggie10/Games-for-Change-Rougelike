using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Hat", menuName = "Clothing Items/Hat")]
    public class HatSO : ItemSO
    {
        [SerializeField] string m_name;
        [TextArea]
        [SerializeField] string m_description;
        [SerializeField] Sprite m_sprite;

        public override ItemType itemType => ItemType.Hat;
        public override string itemName => m_name;
        public override string itemDescription => m_description;
        public override Sprite inventorySprite => m_sprite;

        public override void Use(PlayerStats player)
        {

        }

        public void OnEquip(PlayerStats player)
        {

        }

        public void OnUnequip(PlayerStats player)
        {

        }
    }
}
