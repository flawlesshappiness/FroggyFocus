using Godot;

public partial class WorldObject : ControlScript
{
    [Export]
    public Node3D Origin;

    private Node3D _current;

    public void Clear()
    {
        if (_current == null) return;

        _current.QueueFree();
        _current = null;
    }

    public void SetObject(Node3D obj)
    {
        Clear();

        _current = obj;
        _current.SetParent(Origin);
        _current.Transform = Origin.Transform;
    }
}
