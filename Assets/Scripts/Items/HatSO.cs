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

        public override ItemType itemType => ItemType.Hat;
        public override string itemName => m_name;
        public override string itemDescription => m_description;
        public override Sprite inventorySprite => m_sprite;
        public override IReadOnlyList<PassiveAbility> passives => m_passives;

        public override void Use(PlayerStats player)
        {
            player.EquipHat(this);
        }
    }
}
