using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Run")]
    [Range(0.0f, 10.0f)]
    public float moveSpeed = 5f;
    [Range(0.0f, 10.0f)]
    public float acceleration = 5f;
    [Range(0.0f, 10.0f)]
    public float deceleration = 5f;

    [Header("Jump")]
    [Range(0.0f, 10.0f)]
    public float jumpForce = 5f;
    [Range(0.0f, 10.0f)]
    public float fallForce = 2.5f;
    [Range(0.0f, 10.0f)]
    public float lowJumpForce = 2f;

    //TODO Force Physics
    //TODO Walljump

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new (x, y);
        Run(direction.x);
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        VariableJump();
    }

    private void Run(float xDirection)
    {
        float targetSpeed = xDirection * moveSpeed;
        float speedDifference = targetSpeed - _rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, 1) * Mathf.Sign(speedDifference);

        _rb.AddForce(movement* Vector2.right);
    }

    private void Jump()
    {
        _rb.velocity = Vector2.up * jumpForce;
    }

    private void VariableJump()
    {
        if(_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallForce - 1) * Time.deltaTime;
        } else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpForce - 1) * Time.deltaTime;
        } 
    }
}
