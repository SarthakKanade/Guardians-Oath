using UnityEngine;

public class ObjectToProtect : Entity
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Update()
    {
        HandleFlip();
    }

    protected override void HandleFlip()
    {
        if (player.transform.position.x > transform.position.x && facingright == false)
        {
            Flip();
        }
        else if (player.transform.position.x < transform.position.x && facingright == true)
        {
            Flip();
        }
    }
    
    protected override void  Die()
    {
        base.Die();
        UI.instance.EnableGameOverUI();
    }
}
