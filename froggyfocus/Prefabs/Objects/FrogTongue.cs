using Godot;

public partial class FrogTongue : Node3D
{
    public virtual Coroutine AnimateTongueTowards(Vector3 position)
    {
        return null;
    }

    public virtual Coroutine AnimateTongueBack()
    {
        return null;
    }

    public virtual void AttachToTongue(Node3D target)
    {

    }

    public virtual void ClearTongueAttachement()
    {

    }
}
