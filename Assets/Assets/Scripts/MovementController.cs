using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Checks")]
    public LayerMask groundLayer;
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public Transform rightWallCheckPoint;
    public Vector2 rightWallCheckSize;
    public Transform leftWallCheckPoint;
    public Vector2 leftWallCheckSize;

    [Header("Run")]
    [Range(0.0f, 100.0f)]
    public float topSpeed = 5f;
    [Range(0.0f, 100.0f)]
    public float acceleration = 1f;
    [Range(0.0f, 100.0f)]
    public float handling = 1f;
    [Range(0.0f, 100.0f)]
    public float breaking = 1f;

    [Header("Jump")]
    [Range(0.0f, 100.0f)]
    public float jumpForce = 5f;
    [Range(0.0f, 100.0f)]
    public float extraJumpForce = 5f;
    public int extraJumps = 1;
    [Range(0.0f, 100.0f)]
    public float fallForce = 5f;
    [Range(0.0f, 1.0f)]
    public float minimumTimeToJumpCut = .5f;
    [Range(0.0f, 1.0f)]
    public float jumpBuffer = .1f;
    [Range(0.0f, 1.0f)]
    public float coyoteTime = .1f;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce = new(1, 1);

    private Rigidbody2D _rb;
    private float gravity;
    private bool _isGrounded;
    private bool _isRightWallRiding;
    private bool _isLeftWallRiding;
    private bool _canJumpCut = false;
    private float _jumpCutTimer = 0f;
    private int _remainingJumps;
    private float _jumpBufferTimer;
    private float _jumpCoyoteTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        gravity = _rb.gravityScale;
    }

    private void Update()
    {
        _jumpBufferTimer -= Time.deltaTime;
        _jumpCutTimer -= Time.deltaTime;
        _jumpCoyoteTimer -= Time.deltaTime;
        UpdatesCheck();

        if (_rb.velocity.y < 0)
        {
            _rb.gravityScale = gravity * fallForce;
        }
        else
        {
            _rb.gravityScale = gravity;
        }
        #region Jump Button Verification
        if (Input.GetButtonDown("Jump") && _isRightWallRiding)
        {
            WallJump(-1);
            return;
        }
        if (Input.GetButtonDown("Jump") && _isLeftWallRiding)
        {
            WallJump(1);
            return;
        }
        if ((Input.GetButtonDown("Jump") || _jumpBufferTimer > 0) && (_isGrounded || _jumpCoyoteTimer > 0) && _rb.velocity.y <= 0)
        {
            _jumpCoyoteTimer = 0;
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
        Vector3 groundWireframe = new(groundCheckPoint.position.x, groundCheckPoint.position.y, 0);
        Vector3 rightWallWireframe = new(rightWallCheckPoint.position.x, rightWallCheckPoint.position.y, 0);
        Vector3 leftWallWireframe = new(leftWallCheckPoint.position.x, leftWallCheckPoint.position.y, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundWireframe, groundCheckSize);
        Gizmos.DrawWireCube(rightWallWireframe, rightWallCheckSize);
        Gizmos.DrawWireCube(leftWallWireframe, leftWallCheckSize);
    }

    private void UpdatesCheck()
    {
        //Left Wall
        if (Physics2D.OverlapBox(leftWallCheckPoint.position, leftWallCheckSize, 0, groundLayer))
        {
            _isLeftWallRiding = true;
            _remainingJumps = extraJumps;
        }
        else
        {
            _isLeftWallRiding = false;
        }
        //Right Wall
        if (Physics2D.OverlapBox(rightWallCheckPoint.position, rightWallCheckSize, 0, groundLayer))
        {
            _isRightWallRiding = true;
            _remainingJumps = extraJumps;
        }
        else
        {
            _isRightWallRiding = false;
        }
        //Ground
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            _isGrounded = true;
            _remainingJumps = extraJumps;
        }
        else
        {
            if (_isGrounded) _jumpCoyoteTimer = coyoteTime;
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

    private void WallJump(int dir)
    {
        Vector2 force = wallJumpForce;
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= _rb.velocity.x;

        if (_rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= _rb.velocity.y;

        _rb.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion
}
