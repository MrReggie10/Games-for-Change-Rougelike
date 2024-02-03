using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;

    private float xInput;
    private float yInput;

    private float instantX;
    private float instantY;

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

    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void GetInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canRoll) { StartCoroutine(Roll()); }

        if (!rolling && (xInput != 0 || yInput != 0))
        {
            instantX = xInput;
            instantY = yInput;
        }
    }

    private void ApplyMovement()
    {
        if (!rolling)
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
        canRoll = false;
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
