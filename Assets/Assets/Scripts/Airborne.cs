using UnityEngine;

public class Airborne : PlayerState
{
    public Airborne(MovementController movementController) : base(movementController)
    {
        MovementController.jumpCoyoteTimer = MovementController.coyoteTime;
    }

    public override void Jump()
    {
        if (MovementController.jumpCoyoteTimer > 0)
        {
            CoyoteJump();
            return;
        }
        if (MovementController.remainingJumps <= 0)
        {
            MovementController.jumpBufferTimer = MovementController.jumpBuffer;
            return;
        }
        AirJump();
    }


    public override void JumpCut()
    {
        if (!MovementController.canJumpCut || MovementController.rb.velocity.y < 0) return;
        MovementController.rb.AddForce(Vector2.down * MovementController.rb.velocity.y, ForceMode2D.Impulse);
        MovementController.canJumpCut = false;
    }

    private void CoyoteJump()
    {
        float force = MovementController.jumpForce;
        if (MovementController.rb.velocity.y < 0) force -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        MovementController.jumpCutTimer = MovementController.minimumTimeToJumpCut;
        MovementController.jumpBufferTimer = 0;
        MovementController.canJumpCut = true;
    }
    private void AirJump()
    {
        MovementController.remainingJumps--;
        float force = MovementController.extraJumpForce;
        if (MovementController.rb.velocity.y < 0) force -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        MovementController.jumpCutTimer = MovementController.minimumTimeToJumpCut;
        MovementController.canJumpCut = true;
    }
}
