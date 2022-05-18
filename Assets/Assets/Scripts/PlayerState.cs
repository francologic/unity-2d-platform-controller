public abstract class PlayerState
{
    protected MovementController MovementController;

    public PlayerState(MovementController movementController)
    {
        MovementController = movementController;
    }
    public virtual void  Jump()
    {
        MovementController.canJumpCut = true;
        return;
    }

    public virtual void JumpCut()
    {
        return;
    }
}
