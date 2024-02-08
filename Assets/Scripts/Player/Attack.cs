using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Transform attackHitboxCenter;
	[SerializeField] private GameObject meleeHitbox;

	private bool kicking = false;
	private bool canKick = true;

	[SerializeField] private float kickTime;
	[SerializeField] private float kickRecoveryTime;

	[SerializeField] private int damage;
	[SerializeField] private float knockbackTime;
	[SerializeField] private float knockbackForce;

	void Update()
    {
		GetInput();
		UpdateHitbox();
	}

	private void GetInput()
    {
		if(Input.GetMouseButtonDown(0) && canKick) { StartCoroutine(Kick()); }
    }

	private void UpdateHitbox()
    {
		if(kicking)
        {
			meleeHitbox.SetActive(true);
        }
		else
        {
			meleeHitbox.SetActive(false);
			Aim();
		}
	}

	private void Aim()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 5.23f;

		Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;

		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		attackHitboxCenter.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	private IEnumerator Kick()
    {
		kicking = true;
		canKick = false;
		StartCoroutine(KickTimer());

		meleeHitbox.SetActive(true);
		List<Collider2D> overlapColliders = new List<Collider2D>();
		meleeHitbox.GetComponent<CapsuleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);
		
		foreach(Collider2D collider in overlapColliders)
        {
			if(collider.GetComponent<IEnemy>() != null)
            {
				collider.GetComponent<IEnemy>().Damage(damage, knockbackTime, knockbackForce);
            }
        }

		yield return new WaitForSeconds(kickTime);

		kicking = false;
	}

	private IEnumerator KickTimer()
	{
		yield return new WaitForSeconds(kickRecoveryTime);

		canKick = true;
	}
}