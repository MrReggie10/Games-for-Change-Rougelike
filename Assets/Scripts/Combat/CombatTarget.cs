using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatTargetStats
{
	public int maxHealth { get; }
	public int defense { get; }
	public bool invuln { get; }
	public CombatTargetType type { get; }
}

public struct DamageInfo
{
	public int attackPower;
	public float knockbackTime;
	public Vector2 knockbackForce;
}

[Flags]
public enum CombatTargetType { Player, Enemy }

[RequireComponent(typeof(ICombatTargetStats))]
public class CombatTarget : MonoBehaviour
{
	private int damageTaken;
	public int health => stats.maxHealth - damageTaken;
	public bool dead { get; private set; }
	public CombatTargetType type => stats.type;
	private ICombatTargetStats stats;
	private Movement movement;

	public event Action<DamageInfo> OnHit;
	public event Action OnDeath;

	void Awake()
	{
		stats = GetComponent<ICombatTargetStats>();
		movement = GetComponent<Movement>();
		damageTaken = 0;
		dead = false;
	}

	public void Damage(DamageInfo info)
	{
		if (stats.invuln)
			return;
		OnHit?.Invoke(info);
		damageTaken += Mathf.Min(info.attackPower - stats.defense, 1);
		if(movement != null)
		{
			movement.Knockback(info.knockbackForce, info.knockbackTime);
		}
		if (damageTaken >= stats.maxHealth)
		{
			dead = true;
			OnDeath?.Invoke();
		}
	}
}
