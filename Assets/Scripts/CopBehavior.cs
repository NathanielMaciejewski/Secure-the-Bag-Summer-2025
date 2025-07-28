using FMODUnity;
using UnityEngine;

public class CopBehavior : MonoBehaviour
{
    [SerializeField] private float HP = 1;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float aggroSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;

    private bool isAlive = true;
    private Vector3 scale;
    private BoxCollider2D hitbox;
    private Transform sprite;
    private Transform lineOfSight;
    private Grabber grabber;
    private bool isAggroed = false;
    private float aggroTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = transform.GetChild(0);
        lineOfSight = transform.GetChild(1);
        hitbox = GetComponent<BoxCollider2D>();
        grabber = GetComponent<Grabber>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (isAggroed)
                isAggroed = Time.time - aggroTime <= 1.5f;

            scale = sprite.transform.localScale;
            if (HP <= 0)
            {
                if (grabber != null)
                    grabber.Throw(new Vector3(6, 3, 0));

                isAlive = false;
                sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                sprite.transform.Translate(new Vector3(-0.3f, 0, 0));

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
                {
                    scale.x *= -1;
                    isAggroed = false;
                }

                transform.Translate(new Vector3((isAggroed ? aggroSpeed : movementSpeed) * scale.x * Time.deltaTime, 0, 0));
            }

            sprite.transform.localScale = scale;
            lineOfSight.transform.localScale = scale;
        }
    }

    public void Aggro()
    {
        if (!isAlive)
            return;

        isAggroed = true;
        PlayCopAggro();
        aggroTime = Time.time;
        Debug.Log($"Cop was aggroed at time {aggroTime}!");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAlive)
            return;

        GameObject thing = collision.gameObject;
        GrabbableBehavior projectile = thing.GetComponent<GrabbableBehavior>();

        if (projectile != null)
        {
            float damage = projectile.GetAttackPower();
            Debug.Log($"Called Get Attack Power and got {damage}");

            if (damage > 0)
            {
                HP -= damage;
                PlayCopDamage();
            }
            else if (thing.name == "Bag" && grabber != null)
            {
                grabber.Grab(thing);
                isAggroed = false;
            }

            return;
        }

        if (thing.CompareTag("Player"))
            thing.GetComponent<IsKillable>()?.Kill();

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

    #region Audio Functions

    private void PlayCopAggro()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.copAggro, this.transform.position);
    }

    private void PlayCopDamage()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.copDamage, this.transform.position);
    }
    #endregion

}
