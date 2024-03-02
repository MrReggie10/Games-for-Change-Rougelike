using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private CircleCollider2D hitbox;

    public float angle;

	public IRangedAttackStats stats;

	private List<Collider2D> overlapColliders = new List<Collider2D>();

    private void Awake()
    {
        hitbox = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        MoveProjectile();

		hitbox.GetComponent<CircleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);

		if (overlapColliders.Count > 0)
		{
			CombatTarget target = overlapColliders[0].GetComponent<CombatTarget>();
			if (target != null && target.type == stats.targetType)
			{
				DamageInfo info = new DamageInfo
				{
					attackPower = stats.attackPower,
					knockbackForce = Vector2.zero,
					knockbackTime = 0f
				};
				overlapColliders[0].GetComponent<CombatTarget>().Damage(info);
			}
			if(target != null)
				if(target.gameObject.layer != 8)
					Destroy(gameObject);
		}
	}

    private void MoveProjectile()
    {
		float xComp = Mathf.Cos(Mathf.Deg2Rad * angle) * stats.projectileSpeed * Time.deltaTime;
		float yComp = Mathf.Sin(Mathf.Deg2Rad * angle) * stats.projectileSpeed * Time.deltaTime;
		transform.position += new Vector3(xComp, yComp);
    }
}
