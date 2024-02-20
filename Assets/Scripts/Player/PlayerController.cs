using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private MeleeAttack attack;
	private Movement movement;
	private PlayerStats stats;

	[SerializeField] InventoryMenuManager inventoryMenu;

	private bool attackOnCooldown;
	private bool dashOnCooldown;

	// Start is called before the first frame update
	void Awake()
	{
		attack = GetComponent<MeleeAttack>();
		movement = GetComponent<Movement>();
		stats = GetComponent<PlayerStats>();
		attackOnCooldown = false;
		dashOnCooldown = false;
	}

	// Update is called once per frame
	void Update()
	{
		if(!attack.attacking)
		{
			attack.AimAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
		if(Input.GetMouseButtonDown(0) && !attackOnCooldown)
		{
			attack.Attack();
			StartCoroutine(AttackCooldown());
		}
		movement.SetInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
		if (Input.GetKeyDown(KeyCode.Space) && !dashOnCooldown)
		{
			movement.Dash();
			StartCoroutine(DashCooldown());
		}
		if(Input.GetKey(KeyCode.LeftShift))
		{
			movement.sprinting = true;
		}
		else
		{
			movement.sprinting = false;
		}

		if(Input.GetKeyDown(KeyCode.E))
        {
			if(inventoryMenu.enabled)
            {
				inventoryMenu.Close();
				Time.timeScale = 1;
            }
			else
            {
				inventoryMenu.Open();
				Time.timeScale = 0;
            }				
        }
	}

	private IEnumerator AttackCooldown()
	{
		attackOnCooldown = true;
		yield return new WaitForSeconds(stats.attackCooldown);
		attackOnCooldown = false;
	}

	private IEnumerator DashCooldown()
	{
		dashOnCooldown = true;
		yield return new WaitForSeconds(stats.dashCooldown);
		dashOnCooldown = false;
	}
}
