using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    private float xInput;
    private float yInput;

    float instantX;
    float instantY;

    private bool rolling;
    float rollingTimer = 0;

    [SerializeField] private float speed;

    [SerializeField] private float rollTime;
    [SerializeField] private float rollMultiplier;
    [SerializeField] private int totalRolls;
    [SerializeField] private float rollRecoveryTime;

    [SerializeField] private int rollsRemaining;
    [SerializeField] private int recoveringRolls = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rollsRemaining = totalRolls;
    }

    void Update()
    {
        GetInput();
        ApplyMovement();
    }

    private void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && rollsRemaining > 0) { StartCoroutine("Roll"); }

        if(!rolling && (xInput != 0 || yInput != 0))
        {
            instantX = xInput;
            instantY = yInput;
        }
    }

    private void ApplyMovement()
    {
        if(!rolling)
        {
            rb.velocity = new Vector2(xInput, yInput).normalized * speed;
        }
        else
        {
            rb.velocity = new Vector2(instantX, instantY).normalized * speed * rollMultiplier;
        }
    }

    private IEnumerator Roll()
    {
        rolling = true;
        rollsRemaining -= 1;
        StartCoroutine("RollTimer");

        yield return new WaitForSeconds(rollTime);

        if(recoveringRolls != 0)
        {
            rolling = false;
        }
    }

    private IEnumerator RollTimer()
    {
        recoveringRolls += 1;
        yield return new WaitForSeconds(rollRecoveryTime);

        if(recoveringRolls == 1)
        {
            rollsRemaining = totalRolls;
        }
        recoveringRolls -= 1;
    }
}
