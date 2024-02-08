using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Shoes", menuName = "Clothing Items/Shoes")]
    public class ShoesSO : ClothingSO
    {
        [SerializeField] string m_name;
        [TextArea]
        [SerializeField] string m_description;
        [SerializeField] Sprite m_sprite;
        [SerializeField] int m_attackModifier;
        [SerializeField] List<PassiveAbility> m_passives;

        public override ItemType type => ItemType.Shoes;
        public override string name => m_name;
        public override string info => base.info + $"\nAttack: {attackModifier}";
        public override string description => m_description;
        public override Sprite inventorySprite => m_sprite;
        public override IReadOnlyList<PassiveAbility> passives => m_passives;
        public int attackModifier => m_attackModifier;

        public override bool Use(PlayerStats player, PlayerInventory inventory)
        {
            ShoesSO oldShoes = player.EquipShoes(this);
            if (oldShoes != null) inventory.AddItem(oldShoes);
            return true;
        }
    }
}
