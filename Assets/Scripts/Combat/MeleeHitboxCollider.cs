using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitboxCollider : MonoBehaviour
{
    [SerializeField] private MeleeAttack attackScript;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		attackScript.HitboxDamage(collision);
	}
}
