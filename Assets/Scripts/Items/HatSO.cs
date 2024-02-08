using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    [CreateAssetMenu(fileName = "New Hat", menuName = "Clothing Items/Hat")]
    public class HatSO : ClothingSO
    {
        [SerializeField] string m_name;
        [TextArea]
        [SerializeField] string m_description;
        [SerializeField] Sprite m_sprite;
        [SerializeField] List<PassiveAbility> m_passives;

        public override ItemType type => ItemType.Hat;
        public override string name => m_name;
        public override string description => m_description;
        public override Sprite inventorySprite => m_sprite;
        public override IReadOnlyList<PassiveAbility> passives => m_passives;

        public override bool Use(PlayerStats player, PlayerInventory inventory)
        {
            HatSO oldHat = player.EquipHat(this);
            if (oldHat != null) inventory.AddItem(oldHat);
            return true;
        }
    }
}
