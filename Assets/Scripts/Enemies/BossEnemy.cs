using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(MeleeAttack), typeof(CombatTarget))]
public class BossEnemy : MonoBehaviour, IMeleeAttackStats, IRangedAttackStats, IMovementStats, ICombatTargetStats, IEnemyRepelStats
{
	public enum State { Walking, Attacking, Stunned }

	int IMeleeAttackStats.attackPower => meleeDamage;
	float IMeleeAttackStats.knockbackTime => meleeKnockbackTime;
	float IMeleeAttackStats.knockbackPower => meleeKnockbackForce;
	float IMeleeAttackStats.activeTime => activeTime;
	CombatTargetType IMeleeAttackStats.targetType => CombatTargetType.Player;

	int IRangedAttackStats.attackPower => rangedDamage;
	float IRangedAttackStats.knockbackPower => rangedKnockbackForce;
	float IRangedAttackStats.knockbackTime => rangedKnockbackTime;
	int IRangedAttackStats.numVolleys => numVolleys;
	int IRangedAttackStats.projectilesPerVolley => projPerVolley;
	float IRangedAttackStats.volleySpread => volleySpread;
	float IRangedAttackStats.timeBetweenVolleys => timeBetweenVolleys;
	float IRangedAttackStats.projectileSpeed => projectileSpeed;
	bool IRangedAttackStats.kamikaze => false;
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


	//list of attacks
	//triple lunge			melee
	//circular sweep		melee
	//rapid-fire bombs		ranged
	//circular projectiles	ranged
	//6 line projectiles	ranged
	//summon enemies		half-health

    private MeleeAttack attackMelee;
	private RangedAttack attackRanged;
	private Movement movement;
	private CombatTarget combatTarget;
	private EnemyRepel repelHitbox;

	[SerializeField] private int maxHealth = 200;
	[SerializeField] private int defense = 0;
	[Space]
	[SerializeField] private float speed = 5;
	[SerializeField] private float acceleration = 100;
	[SerializeField] private float friction = 20;
	[SerializeField] private float knockbackResistanceTime = 0;
	[SerializeField] private float repelHitboxSize = 3;
	[Space]

	private int meleeDamage;
	private float meleeKnockbackTime;
	private float meleeKnockbackForce;
	private float activeTime;
	private int rangedDamage;
	private float rangedKnockbackForce;
	private float rangedKnockbackTime;
	private int numVolleys;
	private int projPerVolley;
	private float volleySpread;
	private float timeBetweenVolleys;
	private float projectileSpeed;
	private float lungeSpeedMult;
	private float initialLungeDistance;

	[Header("Triple Lunge Stats")]
	[SerializeField] private float tL_LungeSpeedMult = 4;
	[SerializeField] private float tL_initialLungeDistance = 0;
	[SerializeField] private int tL_meleeDamage = 50;
	[SerializeField] private float tL_knockbackTime = 0.4f;
	[SerializeField] private float tL_knockbackForce = 20;
	[SerializeField] private float tL_activeTime = 1;
	[SerializeField] private float tL_attackWindupTime = 0.5f;
	[SerializeField] private float tL_attackTime = 0.75f;
	[SerializeField] private float tL_attackCooldown = 3f;

	[Header("Circular Sweep Stats")]
	[SerializeField] private int cS_meleeDamage = 50;
	[SerializeField] private float cS_knockbackTime = 0.5f;
	[SerializeField] private float cS_knockbackForce = 30;
	[SerializeField] private float cS_activeTime = 2f;
	[SerializeField] private float cS_attackWindupTime = 1f;
	[SerializeField] private float cS_attackTime = 2f;
	[SerializeField] private float cS_attackCooldown = 3f;

	[Header("Rapid-fire Bombs Stats")]
	[SerializeField] private int rB_rangedDamage = 50;
	[SerializeField] private int rB_numVolleys = 10;
	[SerializeField] private int rB_projPerVolley = 1;
	[SerializeField] private float rB_spread = 0;
	[SerializeField] private float rB_timeBetweenVolleys = 0.2f;
	[SerializeField] private float rB_projectileSpeed = 7;
	[SerializeField] private float rB_attackWindupTime = 0.5f;
	[SerializeField] private float rB_attackTime = 2f;
	[SerializeField] private float rB_attackCooldown = 3f;
	[SerializeField] private float rB_knockbackForce = 30;
	[SerializeField] private float rB_knockbackDuration = 0.4f;
	[SerializeField] private GameObject rB_projectile;

	[Header("Circle Projectile Stats")]
	[SerializeField] private int cP_rangedDamage = 30;
	[SerializeField] private int cP_numVolleys = 3;
	[SerializeField] private int cP_projPerVolley = 5;
	[SerializeField] private float cP_spread = 90;
	[SerializeField] private float cP_timeBetweenVolleys = 0.5f;
	[SerializeField] private float cP_projectileSpeed = 7;
	[SerializeField] private float cP_attackWindupTime = 0.5f;
	[SerializeField] private float cP_attackTime = 1.5f;
	[SerializeField] private float cP_attackCooldown = 2f;
	[SerializeField] private float cP_knockbackForce = 0;
	[SerializeField] private float cP_knockbackDuration = 0;
	[SerializeField] private GameObject cP_projectile;

	[Header("6 Line Projectile Stats")]
	[SerializeField] private int lP_rangedDamage = 10;
	[SerializeField] private int lP_numVolleys = 30;
	[SerializeField] private int lP_projPerVolley = 6;
	[SerializeField] private float lP_spread = 360;
	[SerializeField] private float lP_timeBetweenVolleys = 0.1f;
	[SerializeField] private float lP_projectileSpeed = 7;
	[SerializeField] private float lP_attackWindupTime = 0.5f;
	[SerializeField] private float lP_attackTime = 3f;
	[SerializeField] private float lP_attackCooldown = 3f;
	[SerializeField] private float lP_knockbackForce = 0;
	[SerializeField] private float lP_knockbackDuration = 0;
	[SerializeField] private GameObject lP_projectile;

	private bool attackOnCooldown;

	public State state;

	private Coroutine stunCoroutine;
	private Coroutine attackCoroutine;
	private Coroutine cooldownCoroutine;

    private void Awake()
	{
		attackMelee = GetComponent<MeleeAttack>();
		attackRanged = GetComponent<RangedAttack>();
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
				movement.SetInput((PlayerSingleton.player.transform.position - transform.position).normalized + repelHitbox.GetRepelVector().normalized);
				if (!attackOnCooldown)
				{
					int rand = UnityEngine.Random.Range(1, 6);
					switch (rand)
                    {
						case 1:
							attackCoroutine = StartCoroutine(TL_Cycle());
							break;
						case 2:
							attackCoroutine = StartCoroutine(CS_Cycle());
							break;
						case 3:
							attackCoroutine = StartCoroutine(RB_Cycle());
							break;
						case 4:
							attackCoroutine = StartCoroutine(CP_Cycle());
							break;
						case 5:
							attackCoroutine = StartCoroutine(LP_Cycle());
							break;
					}
				}
				break;
		}
	}

	public IEnumerator ActivateStun(float secondsActive)
	{
		if (attackCoroutine != null)
			StopCoroutine(attackCoroutine);
		attackMelee.CancelAttack();
		movement.SetInput(Vector2.zero);
		state = State.Stunned;

		yield return new WaitForSeconds(secondsActive);

		state = State.Walking;
	}

	private IEnumerator TL_Cycle()
	{
		lungeSpeedMult = tL_LungeSpeedMult;
		initialLungeDistance = tL_initialLungeDistance;
		meleeDamage = tL_meleeDamage;
		meleeKnockbackTime = tL_knockbackTime;
		meleeKnockbackForce = tL_knockbackForce;
		activeTime = tL_activeTime;

		state = State.Attacking;
		movement.SetInput(Vector2.zero);
		Debug.Log("Wind-up");
		for(int i = 0; i < 3; i++)
        {
			Vector3 playerPos = PlayerSingleton.player.transform.position;
			tL_initialLungeDistance = (float) Math.Sqrt(Math.Pow(playerPos.x - transform.position.x, 2) + Math.Pow(playerPos.y - transform.position.y, 2));
			attackMelee.AimAt(playerPos);
			yield return new WaitForSeconds(tL_attackWindupTime);
			attackMelee.Attack();
			movement.SetInput((playerPos - transform.position).normalized);
			movement.Dash();
			movement.SetInput(Vector2.zero);

			yield return new WaitForSeconds(tL_attackTime);
		}
		state = State.Walking;

		cooldownCoroutine = StartCoroutine(AttackCooldown(tL_attackCooldown));
	}

	private IEnumerator CS_Cycle()
	{
		meleeDamage = cS_meleeDamage;
		meleeKnockbackTime = cS_knockbackTime;
		meleeKnockbackForce = cS_knockbackForce;
		activeTime = cS_activeTime;

		state = State.Attacking;
		movement.SetInput(Vector2.zero);

		yield return new WaitForSeconds(cS_attackWindupTime);
		attackMelee.Attack();

		for(float i = 0; i < cS_attackTime; i += Time.deltaTime)
		{
			Vector3 playerPos = PlayerSingleton.player.transform.position;
			attackMelee.AimAt(playerPos);
			yield return new WaitForEndOfFrame();
		}

		state = State.Walking;

		cooldownCoroutine = StartCoroutine(AttackCooldown(cS_attackCooldown));
	}

	private IEnumerator RB_Cycle()
    {
		rangedDamage = rB_rangedDamage;
		numVolleys = rB_numVolleys;
		projPerVolley = rB_projPerVolley;
		volleySpread = rB_spread;
		timeBetweenVolleys = rB_timeBetweenVolleys;
		projectileSpeed = rB_projectileSpeed;
		rangedKnockbackForce = rB_knockbackForce;
		rangedKnockbackTime = rB_knockbackDuration;
		attackRanged.projectilePrefab = rB_projectile;

		state = State.Attacking;
		movement.SetInput(Vector2.zero);
		yield return new WaitForSeconds(rB_attackWindupTime);

		attackRanged.Attack();

		yield return new WaitForSeconds(rB_attackTime);

		state = State.Walking;

		cooldownCoroutine = StartCoroutine(AttackCooldown(rB_attackCooldown));
	}

	private IEnumerator CP_Cycle()
	{
		rangedDamage = cP_rangedDamage;
		numVolleys = cP_numVolleys;
		projPerVolley = cP_projPerVolley;
		volleySpread = cP_spread;
		timeBetweenVolleys = cP_timeBetweenVolleys;
		projectileSpeed = cP_projectileSpeed;
		rangedKnockbackForce = cP_knockbackForce;
		rangedKnockbackTime = cP_knockbackDuration;
		attackRanged.projectilePrefab = cP_projectile;

		state = State.Attacking;
		movement.SetInput(Vector2.zero);
		yield return new WaitForSeconds(cP_attackWindupTime);

		attackRanged.Attack();

		yield return new WaitForSeconds(cP_attackTime);

		state = State.Walking;

		cooldownCoroutine = StartCoroutine(AttackCooldown(cP_attackCooldown));
	}

	private IEnumerator LP_Cycle()
	{
		rangedDamage = lP_rangedDamage;
		numVolleys = lP_numVolleys;
		projPerVolley = lP_projPerVolley;
		volleySpread = lP_spread;
		timeBetweenVolleys = lP_timeBetweenVolleys;
		projectileSpeed = lP_projectileSpeed;
		rangedKnockbackForce = lP_knockbackForce;
		rangedKnockbackTime = lP_knockbackDuration;
		attackRanged.projectilePrefab = lP_projectile;

		state = State.Attacking;
		movement.SetInput(Vector2.zero);
		yield return new WaitForSeconds(lP_attackWindupTime);

		attackRanged.Attack();

		yield return new WaitForSeconds(lP_attackTime);

		state = State.Walking;

		cooldownCoroutine = StartCoroutine(AttackCooldown(lP_attackCooldown));
	}

	private IEnumerator AttackCooldown(float sec)
	{
		attackOnCooldown = true;
		yield return new WaitForSeconds(sec);
		attackOnCooldown = false;
	}
}