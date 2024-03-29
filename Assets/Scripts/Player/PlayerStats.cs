using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFC.Items;

public class PlayerStats : MonoBehaviour, IMeleeAttackStats, ICombatTargetStats, IMovementStats
{
    public delegate void PlayerEvent(PlayerStats player);
    public delegate void ClothingEvent(PlayerStats player, ClothingSO clothing);

    int IMeleeAttackStats.attackPower => attack;
    float IMeleeAttackStats.knockbackPower => effectiveShoes.knockbackForce;
    float IMeleeAttackStats.knockbackTime => effectiveShoes.knockbackTime;
    float IMeleeAttackStats.activeTime => effectiveShoes.activeTime;
    CombatTargetType IMeleeAttackStats.targetType => CombatTargetType.Enemy;
    int ICombatTargetStats.maxHealth => maxHealth;
    int ICombatTargetStats.defense => defense;
    bool ICombatTargetStats.invuln => false;
    float IMovementStats.baseSpeed => baseSpeed;
    float IMovementStats.baseAcceleration => baseAcceleration;
    float IMovementStats.friction => friction;
    float IMovementStats.knockbackResistanceDuration => knockbackResistanceTime;
    float IMovementStats.sprintSpeedMult => sprintSpeedMultiplier;
    float IMovementStats.sprintAccelMult => sprintAccelerationMultiplier;
    float IMovementStats.dashSpeedMult => rollSpeedMultiplier;
    float IMovementStats.dashDuration => rollDuration;
    CombatTargetType ICombatTargetStats.type => CombatTargetType.Player;
    public float attackCooldown => effectiveShoes.recoveryTime;
    public float dashCooldown => rollCooldown;

    [SerializeField] int maxHealth = 1000;
    [Space]
    [SerializeField] float baseSpeed = 7;
    [SerializeField] float baseAcceleration = 100;
    [SerializeField] float friction = 40;
    [SerializeField] float knockbackResistanceTime;
    [SerializeField] float sprintSpeedMultiplier = 2;
    [SerializeField] float sprintAccelerationMultiplier = 2;
    [SerializeField] float rollSpeedMultiplier = 3;
    [SerializeField] float rollDuration = 0.3f;
    [SerializeField] float rollCooldown = 0.5f;
    [Space]
    [SerializeField] ShoesSO defaultShoes;

    public int attack => effectiveShoes.attackModifier + miscAttackMod;
    private int m_miscAttack;
    public int miscAttackMod { get => m_miscAttack; 
        set 
        {
            if (m_miscAttack == value) return;
            m_miscAttack = value; 
            OnAttackChange?.Invoke(this); 
        } }

    public int defense => shirtsDefense + pantsDefense + miscDefenseMod;
    private int shirtsDefense;
    private int pantsDefense;
    private int m_miscDefense;
    public int miscDefenseMod { get => m_miscDefense; 
        set
        {
            if (m_miscDefense == value) return;
            m_miscDefense = value; 
            OnDefenseChange?.Invoke(this); 
        } }

    [Tooltip("If true, effectiveShirts/effectivePants is the max number of shirts/pants the player can wear.\n" +
        "If false, effectiveShirts/effectivePants is the number they can wear before defense gets scaled.")]
    [SerializeField] bool useHardLimit;
    [Space]
    private List<ShirtSO> m_equippedShirts;
    public IReadOnlyList<ShirtSO> equippedShirts => m_equippedShirts;
    [Tooltip("The max number of shirts the player can wear before their defense values get scaled down.")]
    [SerializeField] int effectiveShirts = 2;
    [Range(0,1)]
    [Tooltip("How much shirts' defense values get multiplied by after the effective limit.\n" +
        "First shirt gets scaled by this amount, second shirt gets scaled by this amount squared, etc.\n" +
        "Only used when useHardLimit = false.")]
    [SerializeField] float shirtDefenseMultiplier = 0.85f;
    [Space]
    private List<PantsSO> m_equippedPants;
    public IReadOnlyList<PantsSO> equippedPants => m_equippedPants;
    [Tooltip("The max number of pants the player can wear before their defense values get scaled down.")]
    [SerializeField] int effectivePants = 1;
    [Range(0,1)]
    [Tooltip("How much pants' defense values get multiplied by after the effective limit.\n" +
        "First pants get scaled by this amount, second pants get scaled by this amount squared, etc.\n" +
        "Only used when useHardLimit = false.")]
    [SerializeField] float pantsDefenseMultiplier = 0.7f;

    public HatSO equippedHat { get; private set; }
    public ShoesSO equippedShoes { get; private set; }
    public ShoesSO effectiveShoes => equippedShoes ? equippedShoes : defaultShoes;

    public event ClothingEvent OnEquipClothing;
    public event ClothingEvent OnUnequipClothing;

    public event PlayerEvent OnAttackChange;
    public event PlayerEvent OnDefenseChange;

    private void Awake()
    {
        m_equippedShirts = new List<ShirtSO>();
        m_equippedPants = new List<PantsSO>();
        UnequipShoes();
    }

    #region Shirts
    public bool EquipShirt(ShirtSO shirt)
    {
        if (equippedShirts.Count >= effectiveShirts && useHardLimit)
            return false;

        int i = 0;
        for(; i < m_equippedShirts.Count; i++)
        {
            if(m_equippedShirts[i].defenseModifier < shirt.defenseModifier)
            {
                break;
            }
        }
        m_equippedShirts.Insert(i, shirt);
        RecalculateShirtsDefense();
        shirt.OnEquip(this);
        OnEquipClothing?.Invoke(this, shirt);
        return true;
    }

    public bool UnequipShirt(ShirtSO shirt)
    {
        if (m_equippedShirts.Remove(shirt))
        {
            RecalculateShirtsDefense();
            shirt.OnUnequip(this);
            OnUnequipClothing?.Invoke(this, shirt);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RecalculateShirtsDefense()
    {
        int oldShirtsDefense = shirtsDefense;
        shirtsDefense = 0;
        for(int i = 0; i < m_equippedShirts.Count; i++)
        {
            if(i < effectiveShirts) //no multiplier for first 2 shirts
            {
                shirtsDefense += m_equippedShirts[i].defenseModifier;
            }
            else
            {
                shirtsDefense += (int)(m_equippedShirts[i].defenseModifier * Mathf.Pow(shirtDefenseMultiplier, i-effectiveShirts+1));
            }
        }
        if (oldShirtsDefense != shirtsDefense)
            OnDefenseChange?.Invoke(this);
    }
    #endregion

    #region Pants
    public bool EquipPants(PantsSO pants)
    {
        if (equippedPants.Count >= effectivePants && useHardLimit)
            return false;

        int i = 0;
        for (; i < m_equippedPants.Count; i++)
        {
            if (m_equippedPants[i].defenseModifier < pants.defenseModifier)
            {
                break;
            }
        }
        m_equippedPants.Insert(i, pants);
        RecalculatePantsDefense();
        pants.OnEquip(this);
        OnEquipClothing?.Invoke(this, pants);
        return true;
    }

    public bool UnequipPants(PantsSO pants)
    {
        if (m_equippedPants.Remove(pants))
        {
            RecalculatePantsDefense();
            pants.OnUnequip(this);
            OnUnequipClothing?.Invoke(this, pants);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RecalculatePantsDefense()
    {
        int oldPantsDefense = pantsDefense;
        pantsDefense = 0;
        for (int i = 0; i < m_equippedPants.Count; i++)
        {
            if (i < effectivePants) //no multiplier for first pants
            {
                pantsDefense += m_equippedPants[i].defenseModifier;
            }
            else
            {
                pantsDefense += (int)(m_equippedPants[i].defenseModifier * Mathf.Pow(pantsDefenseMultiplier, i - effectivePants + 1));
            }
        }
        if (oldPantsDefense != pantsDefense)
            OnDefenseChange?.Invoke(this);
    }
    #endregion

    #region Hats
    public HatSO EquipHat(HatSO hat)
    {
        HatSO oldHat = UnequipHat();
        equippedHat = hat;
        hat.OnEquip(this);
        OnEquipClothing?.Invoke(this, hat);
        return oldHat;
    }

    public HatSO UnequipHat()
    {
        HatSO oldHat = equippedHat;
        equippedHat = null;
        if(oldHat != null)
        {
            oldHat.OnUnequip(this);
            OnUnequipClothing?.Invoke(this, oldHat);
        }
        return oldHat;
    }
    #endregion

    #region Shoes
    public ShoesSO EquipShoes(ShoesSO shoes)
    {
        int oldAttack = attack;
        ShoesSO oldShoes = UnequipShoes(false);
        equippedShoes = shoes;
        if (oldAttack != attack)
            OnAttackChange?.Invoke(this);
        shoes.OnEquip(this);
        OnEquipClothing?.Invoke(this, shoes);
        return oldShoes;
    }

    public ShoesSO UnequipShoes(bool triggerOnAttackChange = true)
    {
        ShoesSO oldShoes = equippedShoes;
        equippedShoes = null;
        if (oldShoes != null)
        {
            if (triggerOnAttackChange && oldShoes.attackModifier != defaultShoes.attackModifier)
                OnAttackChange(this);
            oldShoes.OnUnequip(this);
            OnUnequipClothing?.Invoke(this, oldShoes);
        }
        return oldShoes;
    }
    #endregion
}
