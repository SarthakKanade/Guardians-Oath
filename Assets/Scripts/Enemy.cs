using UnityEngine;
using UnityEngine.XR;

public class Enemy : Entity
{
    [Header("Movement Details")]
    [SerializeField] protected float movespeed = 3.5f;

    private bool playerDetected;
    protected override void Update()
    {
        base.Update(); 
        HandleAttack();
    }

    protected override void HandleMovement()
    {
        if (canmove)
        {
            rb.linearVelocity = new Vector2(facingDirection * movespeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    protected override void HandleAttack()
    {
        if (playerDetected)
        {
            anim.SetTrigger("attack");
        }
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.UpdateKillCount();
        
        // Notify WaveManager that an enemy was killed
        if (WaveManager.instance != null)
        {
            WaveManager.instance.OnEnemyKilled();
        }
    }
}
