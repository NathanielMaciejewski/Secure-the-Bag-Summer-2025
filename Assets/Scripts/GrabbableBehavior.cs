using UnityEngine;

public class GrabbableBehavior : MonoBehaviour
{

    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float xVelDamping = 1.0f;
    [SerializeField] private float xVelThrowMultiplier = 2.5f;
    [SerializeField] private float yVelThrowBoost = 2.5f;
    [SerializeField] private LayerMask groundLayer;

    private BoxCollider2D boxCollider;
    private bool resting = false;
    private bool isHeld = false;
    private Vector2 velocity = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld || resting)
            return;

        velocity.y += gravity * Time.deltaTime;
        velocity.x *= 1.0f / (1.0f + xVelDamping * Time.deltaTime);
        if (isWallToLeft() || isWallToRight())
            velocity.x *= -0.9f;

        if (isHeadBonk())
            velocity.y *= -1.5f;

        if (isGrounded())
        {
            velocity = Vector2.zero;
            resting = true;
        }

        transform.Translate(new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0));
    }

    // Call when picking up the grabbable object to disable physics
    public void Grab()
    {
        resting = false;
        isHeld = true;
    }

    // Call when releasing the grabbable object to initiate a throw with the provided velocity
    public void Release(Vector2 initialVelocity)
    {
        resting = false;
        isHeld = false;
        velocity.x = initialVelocity.x * xVelThrowMultiplier;
        if (Mathf.Abs(initialVelocity.x) > 0.1f)
            velocity.y = initialVelocity.y + yVelThrowBoost;
        else
            velocity.y = initialVelocity.y;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null && velocity.y < 0.01f;
    }

    private bool isHeadBonk()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.up, 0.1f, groundLayer);
        return raycastHit.collider != null && velocity.y > 0;
    }

    private bool isWallToLeft()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.left, 0.1f, groundLayer);
        return raycastHit.collider != null && velocity.x < 0.01f;
    }

    private bool isWallToRight()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.right, 0.1f, groundLayer);
        return raycastHit.collider != null && velocity.x > -0.01f;
    }
}
