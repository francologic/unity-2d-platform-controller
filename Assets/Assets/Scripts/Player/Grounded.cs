using UnityEngine;

public class Grounded : PlayerState
{
    public Grounded(MovementController movementController) : base(movementController)
    {
        MovementController.remainingJumps = MovementController.extraJumps;
    }

    public override void Jump()
    {
        float force = MovementController.jumpForce;
        if (MovementController.rb.velocity.y < 0) force -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        MovementController.jumpCutTimer = MovementController.minimumTimeToJumpCut;
        MovementController.jumpBufferTimer = 0;
        MovementController.canJumpCut = true;
    }
}
