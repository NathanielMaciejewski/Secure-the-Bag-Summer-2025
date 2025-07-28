using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    private CopBehavior copBehavior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        copBehavior = GetComponentInParent<CopBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject thing = collision.gameObject;

        if (thing.CompareTag("Player") || thing.name == "Bag")
            copBehavior.Aggro();
    }
}
