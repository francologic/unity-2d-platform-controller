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
    public float extraJumpForce = 5f;
    public int extraJumps = 1;
    [Range(0.0f, 1.0f)]
    public float minimumTimeToJumpCut = .5f;
    [Range(0.0f, 1.0f)]
    public float jumpBuffer = .1f;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _canJumpCut = false;
    private float _jumpCutTimer = 0f;
    private int _remainingJumps;
    private float _jumpBufferTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _jumpBufferTimer -= Time.deltaTime;
        _jumpCutTimer -= Time.deltaTime;
        UpdatesCheck();

        #region Jump Button Verification
        if ((Input.GetButtonDown("Jump") || _jumpBufferTimer > 0) && _isGrounded && _rb.velocity.y <= 0)
        {
            _jumpBufferTimer = 0;
            Jump(jumpForce);
        }
        else if (Input.GetButtonDown("Jump") && _remainingJumps == 0)
        {
            _jumpBufferTimer = jumpBuffer;
        }
        if (Input.GetButtonDown("Jump") && !_isGrounded && _remainingJumps > 0)
        {
            _remainingJumps--;
            Jump(extraJumpForce);
        }
        if (!Input.GetButton("Jump") && _canJumpCut && _rb.velocity.y > 0 && _jumpCutTimer <= 0)
        {
            JumpCut();
        }
        #endregion

    }

    private void FixedUpdate()
    {
        Move(Input.GetAxis("Horizontal"));
    }

    private void OnDrawGizmos()
    {
        Vector3 wireframe = new(groundCheckPoint.position.x, groundCheckPoint.position.y, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wireframe, groundCheckSize);
    }

    private void UpdatesCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            _isGrounded = true;
            _remainingJumps = extraJumps;
        }
        else
        {
            _isGrounded = false;
        }
    }

    #region Move
    private void Move(float direction)
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

    #region Jump
    private void Jump(float initialForce)
    {
        float force = initialForce;
        if (_rb.velocity.y < 0) force -= _rb.velocity.y;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        _jumpCutTimer = minimumTimeToJumpCut;
        _canJumpCut = true;

    }

    private void JumpCut()
    {
        _rb.AddForce(Vector2.down * _rb.velocity.y, ForceMode2D.Impulse);
        _canJumpCut = false;
    }
    #endregion
}
