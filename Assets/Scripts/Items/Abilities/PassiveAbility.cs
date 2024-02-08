using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public abstract class PassiveAbility : ScriptableObject
{
    public abstract void InitEffect(PlayerStats player, ClothingSO attachedClothing);

    public abstract void RemoveEffect(PlayerStats player, ClothingSO attachedClothing);
}
