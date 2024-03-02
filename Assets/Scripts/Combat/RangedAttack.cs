using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRangedAttackStats
{
	public int attackPower { get; }

	public int numVolleys { get; }
	public int projectilesPerVolley { get; }
	public float volleySpread { get; }
	public float timeBetweenVolleys { get; }
	public float projectileSpeed { get; }

	public CombatTargetType targetType { get; }
}

//TODO: Change hit detection to be active the entire time the attack is out, rather than only when it's first activated (make sure it doesn't rehit anyone)
[RequireComponent(typeof(IRangedAttackStats))]
public class RangedAttack : MonoBehaviour
{
	[SerializeField] private GameObject projectilePrefab;

	private bool m_attacking = false;
	public bool attacking => m_attacking;
	private Vector2 aimDirection;
	private float attackAngle;

	private IRangedAttackStats stats;

	private Coroutine attackCoroutine;

	private void Awake()
	{
		stats = GetComponent<IRangedAttackStats>();
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
		attackAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
		m_attacking = false;
	}

	private IEnumerator Attack_Internal()
	{
		m_attacking = true;

		for(int i = 0; i < stats.numVolleys; i++)
        {
			for (int j = 0; j < stats.projectilesPerVolley; j++)
			{
				//float xComp = Mathf.Cos(attackAngle + (j * stats.volleySpread / stats.projectilesPerVolley) - (stats.volleySpread / 2));
				//float yComp = Mathf.Sin(attackAngle + (j * stats.volleySpread / stats.projectilesPerVolley) - (stats.volleySpread / 2));
				float xComp = Mathf.Cos(Mathf.Deg2Rad * (attackAngle + (j * stats.volleySpread / stats.projectilesPerVolley) - (stats.volleySpread / 2)));
				float yComp = Mathf.Sin(Mathf.Deg2Rad * (attackAngle + (j * stats.volleySpread / stats.projectilesPerVolley) - (stats.volleySpread / 2)));
				Projectile projectile = Instantiate(projectilePrefab, transform.position + new Vector3(xComp, yComp), Quaternion.identity).GetComponent<Projectile>();
				projectile.angle = attackAngle + (j * stats.volleySpread / stats.projectilesPerVolley) - (stats.volleySpread / 2);
				projectile.stats = stats;
				Debug.Log("this should print 9 times");
			}

			yield return new WaitForSeconds(stats.timeBetweenVolleys);
			Debug.Log("this should print 3 times");
		}

		m_attacking = false;
	}
}
