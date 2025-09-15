using UnityEngine;

public class Player : Entity
{
    [Header("Movement Details")]
    [SerializeField] protected float movespeed = 8f;
    [SerializeField] private float jumpforce = 12.0f;
    private bool canjump = true;
    private float xInput;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth; // Ensure health is set to max on start
    }

    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleAttack();
        }
    }

    protected override void HandleMovement()
    {
        if (canmove)
        {
            rb.linearVelocity = new Vector2(xInput * movespeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        if (isGrounded && canjump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpforce);
        }
    }

    public override void EnableMovement(bool enable)
    {
        base.EnableMovement(enable);
        canjump = enable;
    }

    protected override void Die()
    {
        base.Die();
    }

     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}  
