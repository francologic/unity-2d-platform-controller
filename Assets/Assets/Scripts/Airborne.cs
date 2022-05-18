using UnityEngine;

public class Airborne : PlayerState
{
    public Airborne(MovementController movementController) : base(movementController)
    {

    }

    public override void Jump()
    {
        base.Jump();
        if (MovementController.remainingJumps <= 0){
            MovementController.jumpBufferTimer = MovementController.jumpBuffer;
            return;
        }
        MovementController.remainingJumps--;
        float force = MovementController.jumpForce;
        if (MovementController.rb.velocity.y < 0) force -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        MovementController.jumpCutTimer = MovementController.minimumTimeToJumpCut;
    }


    public override void JumpCut()
    {
        if(!MovementController.canJumpCut) return;
        MovementController.rb.AddForce(Vector2.down * MovementController.rb.velocity.y, ForceMode2D.Impulse);
        MovementController.canJumpCut = false;
    }
}
