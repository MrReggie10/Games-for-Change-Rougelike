using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementStats
{
	public float baseSpeed { get; }
	public float baseAcceleration { get; }
	public float friction { get; }
	public float knockbackResistanceDuration { get; }

	public float sprintSpeedMult { get; }
	public float sprintAccelMult { get; }

	public float dashSpeedMult { get; }
	public float dashDuration { get; }
}

[RequireComponent(typeof(IMovementStats))]
public class Movement : MonoBehaviour
{
	private Rigidbody2D rb;
	private IMovementStats stats;

	private Vector2 m_moveInput;
	public Vector2 moveInput { get => m_moveInput; set => SetInput(value); }
	private Vector2 dashDirection;

	[HideInInspector] public bool sprinting = false;
	public bool dashing { get; private set; } = false;
	private float stunTimer;
	public bool stunned => stunTimer > 0;
	private float knockbackResistanceTimer;
	public bool knockbackResistant => knockbackResistanceTimer > 0;

	public event Action<Vector2> OnDash;
	public event Action<Vector2, bool> OnKnockback;
	public event Action<float> OnStun;

	private Coroutine dashCoroutine;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		stats = GetComponent<IMovementStats>();
	}

	private void FixedUpdate()
	{
		ApplyMovement();
		if (stunTimer > 0)
			stunTimer -= Time.fixedDeltaTime;
		if (knockbackResistanceTimer > 0)
			knockbackResistanceTimer -= Time.fixedDeltaTime;
	}

	public void SetInput(Vector2 direction, bool clampMagnitude = true)
	{
		m_moveInput = direction;
		if (m_moveInput.magnitude > 1 && clampMagnitude)
			m_moveInput.Normalize();
	}

	private void ApplyMovement()
	{
		if (dashing)
		{
			//no acceleration
			rb.velocity = stats.baseSpeed * stats.dashSpeedMult * dashDirection;
		}
		else
		{
			Vector2 targetSpeed;
			float acceleration;
			if(stunned)
            {
				targetSpeed = Vector2.zero;
				acceleration = 0;
            }
			else if (sprinting)
			{
				targetSpeed = stats.baseSpeed * stats.sprintSpeedMult * m_moveInput;
				acceleration = stats.baseAcceleration * stats.sprintAccelMult;
			}
			else
			{
				targetSpeed = stats.baseSpeed * m_moveInput;
				acceleration = stats.baseAcceleration;
			}
			Vector2 speedDiff = targetSpeed - rb.velocity;
			if (Vector2.Dot(speedDiff, rb.velocity) < 0) //if speedDiff and rb.velocity are in opposite directions (decelerating)
				acceleration += stats.friction;
			if (speedDiff.magnitude < acceleration * Time.fixedDeltaTime)
				rb.velocity = targetSpeed;
			else
				rb.velocity += acceleration * Time.fixedDeltaTime * speedDiff.normalized;
		}
	}

	public bool Dash()
	{
		if (m_moveInput == Vector2.zero)
			return false;
		if (dashing)
			CancelDash();
		dashCoroutine = StartCoroutine(Dash_Internal());
		return true;
	}

	public void CancelDash()
	{
		if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
		dashing = false;
	}

	private IEnumerator Dash_Internal()
	{
		dashDirection = m_moveInput;
		dashing = true;
		OnDash?.Invoke(dashDirection);
		yield return new WaitForSeconds(stats.dashDuration);

		dashing = false;
	}

	public void Knockback(Vector2 force, float duration, bool additive = false)
	{
		if (!additive)
			rb.velocity = Vector2.zero;
		rb.velocity += force;
		OnKnockback?.Invoke(force, knockbackResistant);
		if(!knockbackResistant)
        {
			Stun(duration);
			knockbackResistanceTimer = stats.knockbackResistanceDuration;
        }
	}

	public void Stun(float duration)
    {
		stunTimer = duration;
		OnStun?.Invoke(duration);
    }
}
