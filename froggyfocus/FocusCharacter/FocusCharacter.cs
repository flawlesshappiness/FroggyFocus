public partial class FocusCharacter : Character
{
    public void SetMoving(bool moving)
    {
        if (moving)
        {
            MoveStarted();
        }
        else
        {
            MoveStopped();
        }
    }

    protected virtual void MoveStarted()
    {

    }

    protected virtual void MoveStopped()
    {

    }
}
