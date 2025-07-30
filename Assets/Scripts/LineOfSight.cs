using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    private CopBehavior copBehavior;
    private BoxCollider2D hitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        copBehavior = GetComponentInParent<CopBehavior>();
        hitbox = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject thing = collision.gameObject;

        if (thing.CompareTag("Player") || (thing.name == "Bag" && !copBehavior.HasBag()))
            copBehavior.Aggro();
    }
}
