using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D coll;
    protected SpriteRenderer sr;

    [Header("Health Details")]
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected int currentHealth;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageFeedbackDuration = 0.2f;
    private Coroutine damageFeedbackCoroutine; 

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;  
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;

    [Header("Collision Details")]
    [SerializeField] private float groundcheckdistance;
    [SerializeField] private LayerMask whatIsGround;
    protected bool isGrounded;

    //Direction Details
    protected float facingDirection = 1;
    protected bool canmove = true;  
    protected bool facingright = true;



    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        coll = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        HandleAnimation();
        HandleMovement();
        HandleFlip();
        HandleCollision();
    }

    protected virtual void HandleMovement()
    {
        // To be overridden in derived classes
    }

    public void DamageTargets()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        foreach (Collider2D enemy in enemyColliders)
        {
            Entity entityTarget = enemy.GetComponent<Entity>();
            entityTarget.TakeDamage();
        }
    }

    protected void TakeDamage()
    {
        currentHealth--;
        PlayDamageFeedback();

        if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    protected virtual void Die()
    {
        anim.enabled = false;
        coll.enabled = false;

        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 15);

        Destroy(gameObject, 5f);
    }


    protected void PlayDamageFeedback() 
    {
        if (damageFeedbackCoroutine != null)
        {
            StopCoroutine(damageFeedbackCoroutine);
        }
        damageFeedbackCoroutine = StartCoroutine(DamageFeedbackCO());
    }

    private IEnumerator DamageFeedbackCO()
    {
        Material originalMaterial = sr.material;
        sr.material = damageMaterial;
        yield return new WaitForSeconds(damageFeedbackDuration);
        sr.material = originalMaterial;
    }

    public virtual void EnableMovement(bool enable)
    {
        canmove = enable;
    }

    protected virtual void HandleAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

    

    protected virtual void HandleAttack()
    {
        if (isGrounded)
        {
            anim.SetTrigger("attack");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    protected virtual void HandleFlip()
    {
        if (rb.linearVelocity.x > 0 && facingright == false)
        {
            Flip();
        }
        else if (rb.linearVelocity.x < 0 && facingright == true)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingright = !facingright;
        facingDirection *= -1;
    }

    
    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundcheckdistance, whatIsGround);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundcheckdistance, 0));
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
