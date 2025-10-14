using Godot;
using System;

public partial class Interactable : Area3D, IInteractable
{
    public event Action OnInteract;

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}
