using UnityEngine;

public class CopBehavior : MonoBehaviour
{
    [SerializeField] private float HP = 1;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isAlive = true;
    private Vector3 scale;
    private BoxCollider2D hitbox;
    private Transform sprite;
    private Grabber grabber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        grabber = GetComponent<Grabber>();
        sprite = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            scale = sprite.transform.localScale;
            if (HP <= 0)
            {
                if (grabber != null)
                    grabber.Throw(new Vector3(6, 3, 0));

                isAlive = false;
                sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                sprite.transform.Translate(new Vector3(-0.3f, 0, 0));
                //scale.y *= -1;
                //fieldOfVision.transform.localScale = Vector3.zero;

                if (TryGetComponent<GrabbableBehavior>(out var grabbableBehavior))
                {
                    grabbableBehavior.SetEnabled(true);

                    if (grabbableBehavior.IsGrabbable())
                        Debug.Log("Grabbing the cop enabled");
                }
            }
            else
            {
                if (isWallToLeft() || isWallToRight())
                    scale.x *= -1;

                transform.Translate(new Vector3(movementSpeed * scale.x * Time.deltaTime, 0, 0));
            }

            sprite.transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAlive)
            return;

        GameObject thing = collision.gameObject;
        GrabbableBehavior projectile = collision.gameObject.GetComponent<GrabbableBehavior>();

        if (projectile != null)
        {
            float damage = projectile.GetAttackPower();
            Debug.Log($"Called Get Attack Power and got {damage}");

            if (damage > 0)
            {
                HP -= damage;
            }
            else if (thing.name == "Bag" && grabber != null)
            {
                grabber.Grab(thing);
            }
            
            return;
        }

        IsKillable deathBehavior = thing.GetComponent<IsKillable>();
    }

    private bool isWallToLeft()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, Vector2.left, 0.1f, groundLayer);
        return raycastHit.collider != null && sprite.transform.localScale.x < 0;
    }

    private bool isWallToRight()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, Vector2.right, 0.1f, groundLayer);
        return raycastHit.collider != null && sprite.transform.localScale.x > 0;
    }
}
