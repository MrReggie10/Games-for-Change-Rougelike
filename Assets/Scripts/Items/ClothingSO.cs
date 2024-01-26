using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFC.Items
{
    public abstract class ClothingSO : ItemSO
    {
        public abstract IReadOnlyList<PassiveAbility> passives { get; }

        public virtual void OnEquip(PlayerStats player)
        {
            foreach (PassiveAbility passive in passives)
                passive.InitEffect(player, this);
        }

        public virtual void OnUnequip(PlayerStats player)
        {
            foreach (PassiveAbility passive in passives)
                passive.RemoveEffect(player, this);
        }
    }
}
