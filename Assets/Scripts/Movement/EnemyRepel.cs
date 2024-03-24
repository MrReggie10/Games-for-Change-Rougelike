using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyRepelStats
{
	public float repelDist { get; }
}

//TODO: Change hit detection to be active the entire time the attack is out, rather than only when it's first activated (make sure it doesn't rehit anyone)
[RequireComponent(typeof(IEnemyRepelStats))]
public class EnemyRepel : MonoBehaviour
{
	[SerializeField] private Transform enemyRepelHitbox;

	private List<Collider2D> overlapColliders = new List<Collider2D>();

	private IEnemyRepelStats stats;

	//private Coroutine attackCoroutine;

	private void Awake()
	{
		stats = GetComponent<IEnemyRepelStats>();
		enemyRepelHitbox.localScale = new Vector3(stats.repelDist, stats.repelDist);
	}

	public Vector3 GetRepelVector()
    {
		if(enemyRepelHitbox.GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders) != 1)
        {
			Vector3 resultingVector = Vector2.zero;
			for(int i = 0; i < overlapColliders.Count; i++)
            {
				if(overlapColliders[i].gameObject.layer == 9)
					resultingVector += transform.position - overlapColliders[i].transform.position; 
            }
			return resultingVector;
		}
		return Vector3.zero;
    }
}
