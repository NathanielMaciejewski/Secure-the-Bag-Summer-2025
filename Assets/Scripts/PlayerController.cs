using UnityEngine;
using FMOD.Studio;

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

    //public float fallSpeedCap = 1;

    public float relativeScale = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private GameObject heldItem;
    private GameObject closeGrabbableItem;
    private Vector2 playerInput;
    private Vector2 velocity;
    private Vector3 scale = new Vector3(1, 1, 1);
    private MovementState movementState = MovementState.GROUNDED;
    private float jumpInitialY = 0;
    private bool hasJumpTimedOut = false;

    #region Audio
    private EventInstance playerFootsteps;
    #endregion

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
        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.defaultFootsteps);
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
            scale.x = 1.0f;
        else if (velocity.x < -0.01)
            scale.x = -1.0f;

        if (isGrounded())
        {
            if (movementState != MovementState.GROUNDED)
            {
                // Sfx code for landing goes here
                AudioManager.instance.PlayOneShot(FMODEvents.instance.land, this.transform.position);
            }

            movementState = MovementState.GROUNDED;
            //if (Input.GetKey(KeyCode.S))
            //scale.y = 0.5f;
        }
        else
        {
            scale.y = 1.0f;
        }

        switch (movementState)
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
                    PlayJumpSound();
                    if (Input.GetKey(KeyCode.S) && heldItem == null)
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
                    velocity.y += normalJumpGravity * Time.deltaTime;
                    if (velocity.y < normalJumpFallSpeedCap)
                        velocity.y = normalJumpFallSpeedCap;
                }
                break;

            case MovementState.HIGH_JUMP:

                // Check if hitting a ceiling
                if (isHeadBonk() && velocity.y > 0)
                    velocity.y *= -0.7f;

                // Horizontal movement
                doLeftRightMovement(highJumpAccelerationX, highJumpSpeedCap);

                velocity.y += highJumpGravity * Time.deltaTime;
                if (velocity.y < highJumpFallSpeedCap)
                    velocity.y = highJumpFallSpeedCap;
                break;
            case MovementState.LONG_JUMP:

                // Check if hitting a ceiling
                if (isHeadBonk() && velocity.y > 0)
                {
                    movementState = MovementState.NORMAL_JUMP;
                    velocity.y *= -0.7f;
                }

                // Check if hitting a wall
                if (isWallToLeft() || isWallToRight())
                {
                    movementState = MovementState.NORMAL_JUMP;
                    velocity.x *= -0.5f;
                }

                // Horizontal movement
                doLeftRightMovement(longJumpAccelerationX, longJumpSpeedCap);

                velocity.y += longJumpGravity * Time.deltaTime;
                if (velocity.y < longJumpFallSpeedCap)
                    velocity.y = longJumpFallSpeedCap;
                break;
        }

        transform.Translate(new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0));
        transform.localScale = scale * relativeScale;
        UpdateSound();

        // If player presses X and isn't holding something, grab object
        if (heldItem == null
            && closeGrabbableItem != null
            && Vector3.SqrMagnitude(transform.position - closeGrabbableItem.transform.position) <= 1
            && Input.GetKey(KeyCode.X))
        {
            heldItem = closeGrabbableItem;
            PlayGrabSound();
            heldItem.transform.SetParent(transform);
            heldItem.transform.localPosition = new Vector3(0, 8, 0);
        }

        if (heldItem != null && Input.GetKey(KeyCode.C))
        {
            PlayReleaseSound();
            heldItem.transform.SetParent(null);
            heldItem = null;
        }

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
        // Only apply no-input friction when the player is grounded
        else if (isGrounded())
        {
            velocity.x *= 1.0f / (1.0f + 5 * acceleration * Time.deltaTime);
            if (Mathf.Abs(velocity.x) < 0.01f)
                velocity.x = 0;
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
        hasJumpTimedOut = initialVelocity < 0.01f || heldItem != null;
    }

    private void jumpHigh()
    {
        //body.linearVelocityY = jumpVelocity * 1.5f;
        movementState = MovementState.HIGH_JUMP;
        jumpInitialY = transform.localPosition.y;
        velocity.y = highJumpAccelerationY;
        hasJumpTimedOut = true;
    }

    private void jumpLong()
    {
        //body.linearVelocityY = jumpVelocity * 0.7f;
        //body.linearVelocityX *= 1.5f;
        movementState = MovementState.LONG_JUMP;
        jumpInitialY = transform.localPosition.y;
        velocity.x *= 1.5f;
        velocity.y = longJumpAccelerationY;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (heldItem == null && collision.gameObject.CompareTag("Grabbable"))
            closeGrabbableItem = collision.gameObject;
    }

    #region Audio Functions
    private void UpdateSound()
    {
        // Check if footsteps are already playing
        PLAYBACK_STATE playbackState;
        playerFootsteps.getPlaybackState(out playbackState);

        // Start footsteps event if player is moving and grounded
        if (Mathf.Abs(velocity.x) > 0.01f && isGrounded())
        {
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        // Otherwsie, stop the footstep event
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void PlayJumpSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.jump, this.transform.position);
    }

    private void PlayGrabSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bagGrab, this.transform.position);
    }
    private void PlayReleaseSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.bagRelease, this.transform.position);
    }

    #endregion

}