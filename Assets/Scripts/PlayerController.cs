using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1;
    public float jumpVelocity = 5;

    private Rigidbody2D body;
    private Vector2 playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        body.linearVelocityX = playerInput.x * movementSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            body.linearVelocityY = jumpVelocity;
        }
    }
}
