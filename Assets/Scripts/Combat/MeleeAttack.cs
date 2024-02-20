using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeleeAttackStats
{
	public int attackPower { get; }
	public float knockbackTime { get; }
	public float knockbackPower { get; }

	public float activeTime { get; }

	public CombatTargetType targetType { get; }
}
[RequireComponent(typeof(IMeleeAttackStats))]
public class MeleeAttack : MonoBehaviour
{
	[SerializeField] private Transform attackHitboxCenter;
	[SerializeField] private GameObject meleeHitbox;

	private bool m_attacking = false;
	public bool attacking => m_attacking;
	private Vector2 aimDirection;

	private IMeleeAttackStats stats;

	private Coroutine attackCoroutine;

	private void Awake()
	{
		stats = GetComponent<IMeleeAttackStats>();
	}

	public void AimAt(Vector2 targetPos)
	{
		Vector2 objectPos = transform.position;
		Vector2 targetOffset = targetPos - objectPos;
		AimInDirection(targetOffset);
	}

	public void AimInDirection(Vector2 direction)
	{
		aimDirection = direction.normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		attackHitboxCenter.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	public void Attack()
	{
		CancelAttack();
		attackCoroutine = StartCoroutine(Attack_Internal());
	}

	public void CancelAttack()
	{
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		meleeHitbox.SetActive(false);
		m_attacking = false;
	}

	private IEnumerator Attack_Internal()
	{
		m_attacking = true;

		meleeHitbox.SetActive(true);
		List<Collider2D> overlapColliders = new List<Collider2D>();
		meleeHitbox.GetComponent<CapsuleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);
		
		foreach(Collider2D collider in overlapColliders)
		{
			CombatTarget target = collider.GetComponent<CombatTarget>();
			if (target != null && target.type == stats.targetType)
			{
				DamageInfo info = new DamageInfo
				{
					attackPower = stats.attackPower,
					knockbackForce = stats.knockbackPower * aimDirection,
					knockbackTime = stats.knockbackTime
				};
				collider.GetComponent<CombatTarget>().Damage(info);
			}
		}

		yield return new WaitForSeconds(stats.activeTime);

		m_attacking = false;
		meleeHitbox.SetActive(false);
	}
}
