using UnityEngine;

public class GrabbableBehavior : MonoBehaviour
{

    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float xVelDamping = 1.0f;
    [SerializeField] private float xVelThrowMultiplier = 2.5f;
    [SerializeField] private float yVelThrowBoost = 2.5f;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private LayerMask groundLayer;

    private BoxCollider2D boxCollider;
    private MovementState state = MovementState.FLYING;
    private Vector2 velocity = Vector2.zero;
    private SwitchWeight weight;

    private enum MovementState
    {
        RESTING,
        FLYING,
        HELD
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        weight = GetComponent<SwitchWeight>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (state != MovementState.FLYING || !isEnabled)
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
            state = MovementState.RESTING;
        }

        transform.Translate(new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0));
    }

    // Call when picking up the grabbable object to disable physics.
    public void Grab()
    {
        state = MovementState.HELD;
    }

    // Call when releasing the grabbable object to initiate a throw with the provided velocity
    public void Release(Vector2 initialVelocity)
    {
        state = MovementState.FLYING;
        velocity.x = initialVelocity.x * xVelThrowMultiplier;
        if (Mathf.Abs(initialVelocity.x) > 0.1f)
            velocity.y = initialVelocity.y + yVelThrowBoost;
        else
            velocity.y = initialVelocity.y;
    }

    public float GetAttackPower()
    {
        if (weight != null && state != MovementState.FLYING)
            return 0;
        return weight.GetTotalWeight();
    }

    public bool IsGrabbable()
    {
        return state != MovementState.HELD && isEnabled;
    }

    public void SetEnabled(bool input)
    {
        isEnabled = input;
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
