using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float groundSpeedCap = 3;
    public float groundAcceleration = 0.1f;
    public float normalJumpSpeedCap = 1;
    public float normalJumpAccelerationX = 0.1f;
    public float normalJumpAccelerationY = 4f;
    public float normalJumpMaxHeight = 3.5f;
    public float normalJumpGravity = -0.1f;
    public float normalJumpFallSpeedCap = -1;
    public float highJumpSpeedCap = 1;
    public float highJumpAccelerationX = 0.1f;
    public float highJumpAccelerationY = 0.1f;
    public float highJumpGravity = -0.1f;
    public float highJumpFallSpeedCap = -1;
    public float longJumpSpeedCap = 1;
    public float longJumpAccelerationX = 0.1f;
    public float longJumpAccelerationY = 0.1f;
    public float longJumpGravity = -0.1f;
    public float longJumpFallSpeedCap = -1;

    public float fallSpeedCap = 1;

    public float relativeScale = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Vector2 playerInput;
    private Vector2 velocity;
    private MovementState movementState = MovementState.GROUNDED;
    private float jumpInitialY = 0;
    private bool hasJumpTimedOut = false;

    private enum MovementState
    {
        GROUNDED,
        NORMAL_JUMP,
        HIGH_JUMP,
        LONG_JUMP
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input
        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Set player facing direction
        if (velocity.x > 0.01)
            transform.localScale = new Vector3(1, 1, 1) * relativeScale;
        else if (velocity.x < -0.01)
            transform.localScale = new Vector3(-1, 1, 1) * relativeScale;

        if (isGrounded())
            movementState = MovementState.GROUNDED;

        switch(movementState)
        {
            case MovementState.GROUNDED:

                // If the player isn't grounded, switch to a falling state
                if (!isGrounded())
                {
                    jumpNormal(0);
                    break;
                }

                // Horizontal movement
                doLeftRightMovement(groundAcceleration, groundSpeedCap);

                // No vertical movement
                velocity.y = 0;

                // Determine whether to jump
                if (Input.GetKey(KeyCode.Space) && isGrounded())
                {
                    if (Input.GetKey(KeyCode.S))
                        if (Mathf.Abs(velocity.x) > 0.5 * groundSpeedCap)
                            jumpLong();
                        else
                            jumpHigh();
                    else
                        jumpNormal(normalJumpAccelerationY);
                }
                break;

            case MovementState.NORMAL_JUMP:

                // Check if hitting a ceiling
                if (isHeadBonk() && velocity.y > 0)
                    velocity.y *= -0.7f;

                // Check if the player's jump has timed out, or they stopped holding jump
                if (transform.localPosition.y - jumpInitialY > normalJumpMaxHeight || !Input.GetKey(KeyCode.Space) || velocity.y < 0.01f)
                    hasJumpTimedOut = true;

                // Horizontal movement
                doLeftRightMovement(normalJumpAccelerationX, normalJumpSpeedCap);

                // Vertical movement
                if (hasJumpTimedOut)
                {
                    velocity.y += normalJumpGravity;
                    if (velocity.y < normalJumpFallSpeedCap)
                        velocity.y = normalJumpFallSpeedCap;
                }
                break;

            case MovementState.HIGH_JUMP:
                break;
            case MovementState.LONG_JUMP:
                break;
        }

        transform.Translate(new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0));

    }

    private void doLeftRightMovement(float acceleration, float speedCap)
    {
        // Determine horizontal velocity
        if (Mathf.Abs(playerInput.x) > 0.01f)
        {
            velocity.x += playerInput.x * acceleration * Time.deltaTime;

            // Check to see if player exceeds speed cap
            if (velocity.x > speedCap)
                velocity.x = speedCap;
            if (velocity.x < -1 * speedCap)
                velocity.x = -1 * speedCap;
        }
        /*else if (Mathf.Abs(velocity.x) < 0.01f * Time.deltaTime)
        {
            velocity.x = 0;
        }*/
        else
        {
            //velocity.x += (velocity.x > 0 ? -0.5f : 0.5f) * acceleration * Time.deltaTime;
            velocity.x *= 1.0f / (1.0f + 5 * acceleration * Time.deltaTime);
        }

        // Check for wall collisions and bounce the player back if so
        if (isWallToLeft())
            velocity.x *= -0.5f;
        if (isWallToRight())
            velocity.x *= -0.5f;
    }

    private void jumpNormal(float initialVelocity)
    {
        movementState = MovementState.NORMAL_JUMP;
        jumpInitialY = transform.localPosition.y;
        velocity.y = initialVelocity;
        hasJumpTimedOut = initialVelocity < 0.01f;
    }

    private void jumpHigh()
    {
        //body.linearVelocityY = jumpVelocity * 1.5f;
        movementState = MovementState.HIGH_JUMP;
        jumpInitialY = transform.localPosition.y;
        hasJumpTimedOut = true;
    }

    private void jumpLong()
    {
        //body.linearVelocityY = jumpVelocity * 0.7f;
        //body.linearVelocityX *= 1.5f;
        movementState = MovementState.LONG_JUMP;
        jumpInitialY = transform.localPosition.y;
        hasJumpTimedOut = false;
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
