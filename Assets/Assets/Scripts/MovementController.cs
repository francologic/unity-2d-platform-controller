using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Checks")]
    public LayerMask groundLayer;
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;

    [Header("Run")]
    [Range(0.0f, 20.0f)]
    public float topSpeed = 5f;
    [Range(0.0f, 5.0f)]
    public float acceleration = 1f;
    [Range(0.0f, 5.0f)]
    public float handling = 1f;
    [Range(0.0f, 5.0f)]
    public float breaking = 1f;

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
    private bool isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Run(Input.GetAxis("Horizontal"));
        Check();
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        VariableJump();
    }

    #region Run
    private void Run(float direction)
    {
        float targetSpeed = direction * topSpeed;
        float speedDifference = targetSpeed - _rb.velocity.x;
        float power = GetControlPower(targetSpeed);

        _rb.AddForce(speedDifference * power * Vector2.right);
    }

    private float GetControlPower(float targetSpeed)
    {
        if (Mathf.Abs(targetSpeed) < .01f) return breaking;
        if (Mathf.Abs(_rb.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(_rb.velocity.x))) return handling;
        return acceleration;
    }
    #endregion

    private void Check()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {

        _rb.velocity = Vector2.up * jumpForce;
    }

    private void VariableJump()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallForce - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpForce - 1) * Time.deltaTime;
        }
    }
}
