using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Shirt", menuName = "Clothing Items/Shirt")]
    public class ShirtSO : ClothingSO
    {
        [SerializeField] string m_name;
        [TextArea]
        [SerializeField] string m_description;
        [SerializeField] Sprite m_sprite;
        [SerializeField] int m_defenseModifier;
        [SerializeField] List<PassiveAbility> m_passives;

        public override ItemType itemType => ItemType.Shirt;
        public override string itemName => m_name;
        public override string itemDescription => m_description;
        public override Sprite inventorySprite => m_sprite;
        public override IReadOnlyList<PassiveAbility> passives => m_passives;
        public int defenseModifier => m_defenseModifier;

        public override void Use(PlayerStats player)
        {
            player.AddShirt(this);
        }
    }
}
