using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Run")]
    [Range(0.0f, 10.0f)]
    public float speed = 5f;


    [Header("Jump")]
    [Range(0.0f, 10.0f)]
    public float jumpForce = 5f;
    [Range(0.0f, 10.0f)]
    public float fallForce = 2.5f;
    [Range(0.0f, 10.0f)]
    public float lowJumpForce = 2f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);
        Run(direction);
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        VariableJump();
    }

    private void Run(Vector2 direction)
    {
        _rb.velocity = (new Vector2(direction.x * speed, _rb.velocity.y));
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
