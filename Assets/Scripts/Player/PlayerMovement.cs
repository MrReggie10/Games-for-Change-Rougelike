using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Rigidbody2D rb;

	private Vector2 moveInput;

	private Vector2 instantMoveInput;

	private bool rolling = false;
	private bool canRoll = true;

	[SerializeField] private float speed;

	[SerializeField] private float rollTime;
	[SerializeField] private float rollMultiplier;
	[SerializeField] private float rollRecoveryTime;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		ApplyMovement();
	}

	public void SetInput(Vector2 direction)
	{
		moveInput = direction;
		if (moveInput.magnitude > 1)
			moveInput.Normalize();
	}

	private void ApplyMovement()
	{
		if (!rolling)
		{
			rb.velocity = moveInput * speed;
		}
		else
		{
			rb.velocity = instantMoveInput * speed * rollMultiplier;
		}
	}

	public void Roll(bool ignoreCooldown = false)
	{
		if(canRoll || ignoreCooldown)
		{
			StopAllCoroutines();
			StartCoroutine(Roll_Internal());
		}
	}

	private IEnumerator Roll_Internal()
	{
		rolling = true;
		canRoll = false;
		instantMoveInput = moveInput;
		StartCoroutine(RollTimer());

		yield return new WaitForSeconds(rollTime);

		rolling = false;
	}

	private IEnumerator RollTimer()
	{
		yield return new WaitForSeconds(rollRecoveryTime);

		canRoll = true;
	}
}
