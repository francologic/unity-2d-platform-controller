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
    [Range(0.0f, 1.0f)]
    public float minimumTimeToJumpCut = .5f;

    //TODO Force Physics
    //TODO Walljump

    private Rigidbody2D _rb;
    private bool isGrounded;
    private bool canJumpCut = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdatesCheck();
        Run(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        if(Input.GetButtonUp("Jump") && canJumpCut && _rb.velocity.y > 0)
        {
            JumpCut();  
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 wireframe = new(groundCheckPoint.position.x, groundCheckPoint.position.y, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wireframe, groundCheckSize);
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

    private void UpdatesCheck()
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
        float force = jumpForce;
        if (_rb.velocity.y < 0) force -= _rb.velocity.y;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        canJumpCut = true;
    }

    private void JumpCut()
    {
        _rb.AddForce(Vector2.down * _rb.velocity.y, ForceMode2D.Impulse);
        canJumpCut = false;
    }
}
