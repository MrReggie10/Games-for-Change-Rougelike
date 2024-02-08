using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Pants", menuName = "Clothing Items/Pants")]
    public class PantsSO : ClothingSO
    {
        [SerializeField] string m_name;
        [TextArea]
        [SerializeField] string m_description;
        [SerializeField] Sprite m_sprite;
        [SerializeField] int m_defenseModifier;
        [SerializeField] List<PassiveAbility> m_passives;

        public override ItemType type => ItemType.Pants;
        public override string name => m_name;
        public override string info => base.info + $"\nDefense: {defenseModifier}";
        public override string description => m_description;
        public override Sprite inventorySprite => m_sprite;
        public override IReadOnlyList<PassiveAbility> passives => m_passives;
        public int defenseModifier => m_defenseModifier;

        public override bool Use(PlayerStats player, PlayerInventory inventory)
        {
            return player.EquipPants(this);
        }
    }
}
