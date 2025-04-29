using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerInteract : Area3D
{
    private List<GodotObject> _bodies = new();

    public override void _Ready()
    {
        base._Ready();
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    private void OnAreaEntered(GodotObject body)
    {
        _bodies.Add(body);
    }

    private void OnAreaExited(GodotObject body)
    {
        _bodies.Remove(body);
    }

    public bool TryInteract()
    {
        var nodes = _bodies.Select(x => x as Node3D)
            .Where(x => x != null)
            .OrderBy(x => x.GlobalPosition.DistanceTo(GlobalPosition));

        foreach (var node in nodes)
        {
            var interactable = node as IInteractable;
            if (interactable == null) continue;

            interactable.Interact();
            return true;
        }

        return false;
    }
}
