using UnityEngine;

public class MovementController : StateMachine
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

    [HideInInspector]
    public Rigidbody2D rb;
    private float gravity;
    private bool _isGrounded;
    public bool _isRightWallRiding;
    private bool _isLeftWallRiding;
    private bool _canJumpCut = false;
    public float jumpCutTimer = 0f;
    public int remainingJumps;
    private float _jumpBufferTimer;
    private float _jumpCoyoteTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    private void Update()
    {
        _jumpBufferTimer -= Time.deltaTime;
        jumpCutTimer -= Time.deltaTime;
        _jumpCoyoteTimer -= Time.deltaTime;
        ChangeState();

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravity * fallForce;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if(Input.GetButtonDown("Jump")){
            State.Jump();
        }

        if(!Input.GetButton("Jump")){
            State.JumpCut();
        }

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

    private void ChangeState()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            if (State is not Grounded) SetPlayerState(new Grounded(this));
            return;
        }
        if (Physics2D.OverlapBox(leftWallCheckPoint.position, leftWallCheckSize, 0, groundLayer))
        {
            if (State is not WallRiding) SetPlayerState(new WallRiding(this));
            return;
        }
        if (Physics2D.OverlapBox(rightWallCheckPoint.position, rightWallCheckSize, 0, groundLayer))
        {
            if (State is not WallRiding) SetPlayerState(new WallRiding(this));
            return;
        }
        if (State is not Airborne) SetPlayerState(new Airborne(this));
        return;
    }

    private void Move(float direction)
    {
        float targetSpeed = direction * topSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float power = GetControlPower(targetSpeed);

        rb.AddForce(speedDifference * power * Vector2.right);
    }

    private float GetControlPower(float targetSpeed)
    {
        if (Mathf.Abs(targetSpeed) < .01f) return breaking;
        if (Mathf.Abs(rb.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(rb.velocity.x))) return handling;
        return acceleration;
    }
}
