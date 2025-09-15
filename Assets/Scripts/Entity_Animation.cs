using UnityEngine;

public class Entity_Animation : MonoBehaviour
{
   private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void EnableMovement()
    {
        entity.EnableMovement(true);
    }

    private void DisableMovement()
    {
        entity.EnableMovement(false);
    }

    private void DamageTargets()
    {
        entity.DamageTargets();
    }

    
     
}
 