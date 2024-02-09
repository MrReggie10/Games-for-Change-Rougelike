using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthStats
{
	public int maxHealth { get; }
	public int defense { get; }
}

public class Health : MonoBehaviour
{
	private int damageTaken;
	public int health => stats.maxHealth - damageTaken;
	private IHealthStats stats;

	public event Action OnDeath;

	void Awake()
	{
		stats = GetComponent<IHealthStats>();
		damageTaken = 0;
	}

	public void Damage(int attackPower)
	{
		damageTaken += Mathf.Min(attackPower - stats.defense, 1);

		if (damageTaken >= stats.maxHealth)
		{
			OnDeath?.Invoke();
		}
	}
}
