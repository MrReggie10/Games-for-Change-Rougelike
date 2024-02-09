using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private MeleeAttack attack;
	private PlayerMovement movement;

	// Start is called before the first frame update
	void Start()
	{
		attack = GetComponent<MeleeAttack>();
		movement = GetComponent<PlayerMovement>();
	}

	// Update is called once per frame
	void Update()
	{
		if(!attack.attacking)
		{
			attack.Aim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
		if(Input.GetMouseButtonDown(0))
		{
			attack.Attack();
		}
		movement.SetInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
		if (Input.GetKeyDown(KeyCode.Space))
		{
			movement.Roll();
		}
	}
}
