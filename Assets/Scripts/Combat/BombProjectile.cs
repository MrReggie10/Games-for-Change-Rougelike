using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : MonoBehaviour, IProjectile
{
	[SerializeField] private GameObject bombVisual;
	[SerializeField] private GameObject explosion;
	private Rigidbody2D rb;

	public float angle;
	[SerializeField] private float explosionTimer;

	public IRangedAttackStats stats;

	private List<Collider2D> overlapColliders = new List<Collider2D>();

    private void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		float xComp = Mathf.Cos(Mathf.Deg2Rad * angle) * stats.projectileSpeed;
		float yComp = Mathf.Sin(Mathf.Deg2Rad * angle) * stats.projectileSpeed;
		rb.velocity = new Vector2(xComp, yComp);
    }

    private void Update()
	{
		explosionTimer -= Time.deltaTime;

		if(explosionTimer < 0)
        {
			Explode();
			explosionTimer = 9999;
		}
	}

	private void Explode()
    {
		rb.velocity = Vector2.zero;
		explosion.SetActive(true);
		bombVisual.SetActive(false);
		CheckHitbox();
		StartCoroutine(DestroyTimer());
    }

	private void CheckHitbox()
    {
		explosion.GetComponent<CircleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);

		if (overlapColliders.Count > 0)
		{
			foreach (Collider2D collider in overlapColliders)
			{
				CombatTarget target = collider.GetComponent<CombatTarget>();
				if (target != null && target.type == stats.targetType)
				{
					DamageInfo info = new DamageInfo
					{
						attackPower = stats.attackPower,
						knockbackForce = (PlayerSingleton.player.transform.position - transform.position).normalized * stats.knockbackPower,
						knockbackTime = stats.knockbackTime
					};
					Debug.Log(info.knockbackTime);
					collider.GetComponent<CombatTarget>().Damage(info);
				}
			}
		}
	}

	private IEnumerator DestroyTimer()
    {
		yield return new WaitForSeconds(0.5f);

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
