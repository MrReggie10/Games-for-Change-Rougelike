using GFC.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Ability", menuName = "Passive Ability/Stat Change")]
public class StatChangeAbility : PassiveAbility
{
    [SerializeField] int attackMod;
    [SerializeField] int defenseMod;

    public override void InitEffect(PlayerStats player, ClothingSO attachedClothing)
    {
        player.miscAttackMod += attackMod;
        player.miscDefenseMod += defenseMod;
    }

    public override void RemoveEffect(PlayerStats player, ClothingSO attachedClothing)
    {
        player.miscAttackMod -= attackMod;
        player.miscDefenseMod -= defenseMod;
    }
}
