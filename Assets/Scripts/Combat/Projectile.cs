using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
	public void SetAngle(float angle);
	public void SetStats(IRangedAttackStats stats);
}

public class Projectile : MonoBehaviour, IProjectile
{
    private CircleCollider2D hitbox;

	private float angle;

	public IRangedAttackStats stats;

	private List<Collider2D> overlapColliders = new List<Collider2D>();

    private void Awake()
    {
        hitbox = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        MoveProjectile();
	}

    private void MoveProjectile()
    {
		float xComp = Mathf.Cos(Mathf.Deg2Rad * angle) * stats.projectileSpeed * Time.deltaTime;
		float yComp = Mathf.Sin(Mathf.Deg2Rad * angle) * stats.projectileSpeed * Time.deltaTime;
		transform.position += new Vector3(xComp, yComp);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		CombatTarget target = collision.GetComponent<CombatTarget>();
		if (target != null && target.type == stats.targetType)
		{
			DamageInfo info = new DamageInfo
			{
				attackPower = stats.attackPower,
				knockbackForce = Vector2.zero,
				knockbackTime = 0f
			};
			collision.GetComponent<CombatTarget>().Damage(info);
		}
		if (target != null)
			if (target.gameObject.layer != 8 && target.gameObject.layer != 7)
				Destroy(gameObject);
	}

    public void SetAngle(float angle)
    {
		this.angle = angle;
    }

	public void SetStats(IRangedAttackStats stats)
	{
		this.stats = stats;
	}
}
