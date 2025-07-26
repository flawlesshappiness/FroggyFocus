using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerInteract : Area3D
{
    public bool HasInteractables => _bodies.Count > 0;

    public event Action OnHasInteractable;
    public event Action OnNoInteractable;

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

        if (_bodies.Count == 1)
        {
            OnHasInteractable?.Invoke();
        }
    }

    private void OnAreaExited(GodotObject body)
    {
        _bodies.Remove(body);

        if (_bodies.Count == 0)
        {
            OnNoInteractable?.Invoke();
        }
    }

    public IInteractable GetInteractable()
    {
        var nodes = _bodies.Select(x => x as Node3D)
            .Where(x => x != null)
            .OrderBy(x => x.GlobalPosition.DistanceTo(GlobalPosition));

        foreach (var node in nodes)
        {
            var interactable = node as IInteractable;
            if (interactable == null) continue;

            return interactable;
        }

        return null;
    }

    public bool TryInteract()
    {
        var interactable = GetInteractable();
        if (interactable == null) return false;

        interactable.Interact();
        return true;
    }
}
