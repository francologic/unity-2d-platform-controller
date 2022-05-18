using UnityEngine;

public class WallRiding : PlayerState
{

    public string wallSide = "";
    public WallRiding(MovementController movementController, string side) : base(movementController)
    {
        MovementController.remainingJumps = MovementController.extraJumps;
        wallSide = side;
    }

    public override void Jump()
    {
        Vector2 force = MovementController.wallJumpForce;
        if (wallSide == "Right") force.x *= -1;

        if (Mathf.Sign(MovementController.rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= MovementController.rb.velocity.x;

        if (MovementController.rb.velocity.y < 0)
            force.y -= MovementController.rb.velocity.y;

        MovementController.rb.AddForce(force, ForceMode2D.Impulse);
    }
}
