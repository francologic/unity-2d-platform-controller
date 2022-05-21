public abstract class PlayerState
{
    protected MovementController MovementController;

    public PlayerState(MovementController movementController)
    {
        MovementController = movementController;
    }
    public virtual void  Jump()
    {
        return;
    }

    public virtual void JumpCut()
    {
        return;
    }
}
