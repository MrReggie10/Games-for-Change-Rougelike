using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeleeAttackStats
{
	public int baseDamage { get; }
	public float knockbackTime { get; }
	public float knockbackForce { get; }

	public float activeTime { get; }
	public float recoveryTime { get; }
}

public class MeleeAttack : MonoBehaviour
{
	[SerializeField] private Transform attackHitboxCenter;
	[SerializeField] private GameObject meleeHitbox;

	private bool m_attacking = false;
	public bool attacking => m_attacking;
	private bool m_canAttack = true;
	public bool canAttack => m_canAttack;

	private IMeleeAttackStats stats;

	private void Awake()
	{
		stats = GetComponent<IMeleeAttackStats>();
	}

	public void Aim(Vector2 targetPos)
	{
		Vector2 objectPos = transform.position;
		Vector2 targetOffset = targetPos - objectPos;

		float angle = Mathf.Atan2(targetOffset.y, targetOffset.x) * Mathf.Rad2Deg;
		attackHitboxCenter.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	public void Attack(bool ignoreCooldown = false)
	{
		if (m_canAttack || ignoreCooldown)
		{
			StopAllCoroutines();
			StartCoroutine(Attack_Internal());
		}
	}

	private IEnumerator Attack_Internal()
	{
		m_attacking = true;
		m_canAttack = false;
		StartCoroutine(KickTimer());

		meleeHitbox.SetActive(true);
		List<Collider2D> overlapColliders = new List<Collider2D>();
		meleeHitbox.GetComponent<CapsuleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);
		
		foreach(Collider2D collider in overlapColliders)
		{
			if(collider.GetComponent<IEnemy>() != null)
			{
				collider.GetComponent<IEnemy>().Damage(stats.baseDamage, stats.knockbackTime, stats.knockbackForce);
			}
		}

		yield return new WaitForSeconds(stats.activeTime);

		m_attacking = false;
	}

	private IEnumerator KickTimer()
	{
		yield return new WaitForSeconds(stats.recoveryTime);

		m_canAttack = true;
	}
}
