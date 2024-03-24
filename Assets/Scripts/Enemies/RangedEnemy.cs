using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(RangedAttack), typeof(CombatTarget))]
public class RangedEnemy : MonoBehaviour, IRangedAttackStats, IMovementStats, ICombatTargetStats, IEnemyRepelStats
{
	public enum State { Walking, Attacking, Stunned }

	int IRangedAttackStats.attackPower => damage;
	int IRangedAttackStats.numVolleys => volleys;
	int IRangedAttackStats.projectilesPerVolley => projPerVolley;
	float IRangedAttackStats.volleySpread => spread;
	float IRangedAttackStats.timeBetweenVolleys => timeBetweenVolleys;
	float IRangedAttackStats.projectileSpeed => projectileSpeed;
	float IRangedAttackStats.knockbackPower => knockbackForce;
	float IRangedAttackStats.knockbackTime => knockbackDuration;
	bool IRangedAttackStats.kamikaze => isKamikaze;
	CombatTargetType IRangedAttackStats.targetType => CombatTargetType.Player;
	float IMovementStats.baseSpeed => speed;
	float IMovementStats.baseAcceleration => acceleration;
	float IMovementStats.friction => friction;
	float IMovementStats.knockbackResistanceDuration => knockbackResistanceTime;
	float IMovementStats.sprintSpeedMult => 1;
	float IMovementStats.sprintAccelMult => 1;
	float IMovementStats.dashSpeedMult => lungeSpeedMult;
	float IMovementStats.dashDuration => initialLungeDistance / (speed * lungeSpeedMult);
	int ICombatTargetStats.maxHealth => maxHealth;
	int ICombatTargetStats.defense => defense;
	bool ICombatTargetStats.invuln => false;
	CombatTargetType ICombatTargetStats.type => CombatTargetType.Enemy;
    float IEnemyRepelStats.repelDist => repelHitboxSize;


    private RangedAttack attack;
	private Movement movement;
	private CombatTarget combatTarget;
	private EnemyRepel repelHitbox;

	[SerializeField] private int maxHealth = 200;
	[SerializeField] private int defense = 0;
	[Space]
	[SerializeField] private float speed = 5;
	[SerializeField] private float acceleration = 100;
	[SerializeField] private float friction = 20;
	[SerializeField] private float lungeSpeedMult = 4;
	[SerializeField] private float initialLungeDistance = 3;
	[SerializeField] private float knockbackResistanceTime = 0.5f;
	[SerializeField] private float repelHitboxSize = 3f;
	[Space]
	[SerializeField] private int damage = 50;
	[SerializeField] private int volleys = 1;
	[SerializeField] private int projPerVolley;
	[SerializeField] private float spread = 0;
	[SerializeField] private float timeBetweenVolleys = 0.5f;
	[SerializeField] private float projectileSpeed = 7;
	[SerializeField] private float farAttackDistance = 8;
	[SerializeField] private float closeAttackDistance = 4;
	[SerializeField] private float attackWindupTime = 0.5f;
	[SerializeField] private float attackTime = 0.75f;
	[SerializeField] private float attackInterval = 2.5f;
	[SerializeField] private bool isKamikaze = false;
	[Space]
	[SerializeField] private float knockbackForce = 0;
	[SerializeField] private float knockbackDuration = 0f;
	private bool attackOnCooldown;

	public State state;

	private Coroutine stunCoroutine;
	private Coroutine attackCoroutine;
	private Coroutine cooldownCoroutine;

	private void Awake()
	{
		attack = GetComponent<RangedAttack>();
		movement = GetComponent<Movement>();
		combatTarget = GetComponent<CombatTarget>();
		repelHitbox = GetComponent<EnemyRepel>();
		movement.OnStun += time => { if (stunCoroutine != null) StopCoroutine(stunCoroutine); stunCoroutine = StartCoroutine(ActivateStun(time)); };
		state = State.Walking;
	}

	void Update()
	{
		switch (state)
		{
			case State.Walking:
				if (Vector2.Distance(transform.position, PlayerSingleton.player.transform.position) > farAttackDistance)
					movement.SetInput((PlayerSingleton.player.transform.position - transform.position).normalized + repelHitbox.GetRepelVector().normalized);
				else if (Vector2.Distance(transform.position, PlayerSingleton.player.transform.position) < closeAttackDistance && attackOnCooldown)
					movement.SetInput((- PlayerSingleton.player.transform.position + transform.position).normalized + repelHitbox.GetRepelVector().normalized);
				else if (!attackOnCooldown)
					attackCoroutine = StartCoroutine(AttackCycle());
				break;
		}
	}

	public IEnumerator ActivateStun(float secondsActive)
	{
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attack.CancelAttack();
		movement.SetInput(Vector2.zero);
		state = State.Stunned;

		yield return new WaitForSeconds(secondsActive);

		state = State.Walking;
	}

	private IEnumerator AttackCycle()
	{
		cooldownCoroutine = StartCoroutine(AttackCooldown());
		state = State.Attacking;
		movement.SetInput(Vector2.zero);
		//Debug.Log("Wind-up");
		yield return new WaitForSeconds(attackWindupTime);

		Vector3 playerPos = PlayerSingleton.player.transform.position;
		attack.Attack();
		//Debug.Log("Attack");

		yield return new WaitForSeconds(attackTime);

		//Debug.Log("Attack finished");
		state = State.Walking;
	}

	private IEnumerator AttackCooldown()
	{
		attackOnCooldown = true;
		yield return new WaitForSeconds(attackInterval);
		attackOnCooldown = false;
	}
}
