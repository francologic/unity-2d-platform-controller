using UnityEngine;

public class WallRiding : PlayerState
{
    public WallRiding(MovementController movementController) : base(movementController)
    {

    }

    public override void Jump()
    {
        Vector2 force = MovementController.wallJumpForce;
        // if(MovementController.isRIghtWallRIding) force.x *= -1;

        if (Mathf.Sign(MovementController.rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= MovementController.rb.velocity.x;

        if (MovementController.rb.velocity.y < 0)
            force.y -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(force, ForceMode2D.Impulse);
    }
}
