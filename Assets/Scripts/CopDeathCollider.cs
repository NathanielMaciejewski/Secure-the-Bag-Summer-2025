using UnityEngine;

public class CopDeathCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponentInParent<CopBehavior>()?.DeathColliderTrigger(collision);
    }
}
